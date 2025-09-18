using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class BaseMapColorController : MonoBehaviour
{
    [Header("Target renderer / slot")]
    [Tooltip("Renderer whose current material color will be changed (no material switching here).")]
    public Renderer targetRenderer;

    [Tooltip("Which material slot on the renderer to recolor (0-based).")]
    public int materialSlotIndex = 0;

    [Tooltip("If true, write to sharedMaterials (global asset). If false, write to instance materials.")]
    public bool useSharedMaterial = false;

    [Header("Shader property")]
    [Tooltip("URP usually uses _BaseColor; Built-in often uses _Color. The script tries fallback automatically.")]
    public string baseColorPropertyName = "_BaseColor";

    [Header("Color buttons")]
    [Tooltip("Buttons whose Image.color will be applied to the target material.")]
    public List<Button> colorButtons = new List<Button>();

    private void Awake()
    {
        if (!targetRenderer)
        {
            targetRenderer = GetComponent<Renderer>();
            if (!targetRenderer)
                Debug.LogWarning("[BaseMapColorController] No Renderer assigned/found.");
        }

        // Wire up all buttons once. Each button applies its own Image.color
        foreach (var btn in colorButtons)
        {
            if (!btn) continue;
            var captured = btn; // avoid closure issue
            captured.onClick.AddListener(() =>
            {
                var img = captured.GetComponent<Image>();
                if (!img)
                {
                    Debug.LogWarning($"[BaseMapColorController] Button '{captured.name}' has no Image component.");
                    return;
                }
                ApplyBaseMapColor(img.color);
            });
        }
    }

    /// <summary>
    /// Public API: apply a specific color (you can also call this from other UI like a Color Picker).
    /// </summary>
    public void ApplyBaseMapColor(Color color)
    {
        if (!targetRenderer) return;

        // Get materials array according to write mode
        var mats = useSharedMaterial ? targetRenderer.sharedMaterials : targetRenderer.materials;
        if (mats == null || materialSlotIndex < 0 || materialSlotIndex >= mats.Length)
        {
            Debug.LogWarning($"[BaseMapColorController] Slot {materialSlotIndex} out of range (len={mats?.Length ?? 0}).");
            return;
        }

        var m = mats[materialSlotIndex];
        if (!m)
        {
            Debug.LogWarning($"[BaseMapColorController] Null material in slot {materialSlotIndex}.");
            return;
        }

        // Try the chosen property; if absent, fallback to _Color
        if (!string.IsNullOrEmpty(baseColorPropertyName) && m.HasProperty(baseColorPropertyName))
        {
            m.SetColor(baseColorPropertyName, color);
        }
        else if (m.HasProperty("_Color"))
        {
            m.SetColor("_Color", color);
        }
        else
        {
            Debug.LogWarning($"[BaseMapColorController] Material '{m.name}' has no '{baseColorPropertyName}' nor '_Color'.");
        }

        // Reassign back for instance mode (explicit, keeps things clear)
        if (!useSharedMaterial)
        {
            mats[materialSlotIndex] = m;
            targetRenderer.materials = mats;
        }
        else
        {
            // For sharedMaterials, just setting the color is enough
            targetRenderer.sharedMaterials = mats;
        }
    }
}

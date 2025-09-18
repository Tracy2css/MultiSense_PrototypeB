using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class GlobalMaterialController : MonoBehaviour
{
    [Serializable]
    public class MaterialOption
    {
        [Tooltip("Unique, human-readable key used by buttons (e.g., 'dyson_01').")]
        public string displayName;

        [Tooltip("The material asset to apply.")]
        public Material material;
    }

    [Header("Targets (all will be set to the SAME material)")]
    [Tooltip("Drag all Renderers that should switch together (MeshRenderer / SkinnedMeshRenderer).")]
    public Renderer[] targetRenderers;

    [Header("Apply Mode")]
    [Tooltip("If true, replace ALL material slots for every renderer; otherwise only one slot.")]
    public bool applyToAllSlots = true;

    [Tooltip("When applyToAllSlots is false, which slot to replace (0-based).")]
    public int materialSlotIndex = 0;

    [Tooltip("Use sharedMaterial(s) instead of instanced material(s). Changes affect all users of the asset.")]
    public bool useSharedMaterial = false;

    [Header("Default / Snapshot")]
    [Tooltip("Auto-capture current materials for ALL targets on Awake, so you can Reset later.")]
    public bool autoCaptureDefaultOnAwake = true;

    // One snapshot per renderer (so Reset restores each one’s own original array)
    [SerializeField] private List<Material[]> _defaultsPerRenderer = new List<Material[]>();

    [Header("Candidate Materials (named)")]
    [Tooltip("Add/remove freely. Each option has a name for button triggering.")]
    public List<MaterialOption> options = new List<MaterialOption>();

    // Fast lookup by name
    private Dictionary<string, Material> _byName;

    void Awake()
    {
        BuildLookup();

        if (autoCaptureDefaultOnAwake)
            CaptureCurrentAsDefault();
    }

    private void BuildLookup()
    {
        _byName = new Dictionary<string, Material>(StringComparer.OrdinalIgnoreCase);
        foreach (var opt in options)
        {
            if (opt == null || opt.material == null) continue;
            if (string.IsNullOrWhiteSpace(opt.displayName)) continue;

            if (!_byName.ContainsKey(opt.displayName))
                _byName.Add(opt.displayName, opt.material);
            else
                Debug.LogWarning($"[GlobalMaterialController] Duplicate displayName: {opt.displayName}. Only first is used.");
        }
    }

    /// <summary>
    /// Call this from a UI Button. All target renderers switch to the material with that displayName.
    /// </summary>
    public void ApplyByName(string displayName)
    {
        if (_byName == null || !_byName.TryGetValue(displayName, out var mat) || mat == null)
        {
            Debug.LogWarning($"[GlobalMaterialController] Material name not found: {displayName}");
            return;
        }
        ApplyMaterialToAll(mat);
    }

    /// <summary>
    /// Apply a specific material to ALL target renderers (same result everywhere).
    /// </summary>
    public void ApplyMaterialToAll(Material mat)
    {
        if (mat == null) return;

        foreach (var r in targetRenderers)
        {
            if (r == null) continue;

            if (applyToAllSlots)
            {
                if (useSharedMaterial)
                {
                    var arr = r.sharedMaterials;
                    for (int i = 0; i < arr.Length; i++) arr[i] = mat;
                    r.sharedMaterials = arr;
                }
                else
                {
                    var arr = r.materials;
                    for (int i = 0; i < arr.Length; i++) arr[i] = mat;
                    r.materials = arr;
                }
            }
            else
            {
                if (materialSlotIndex < 0)
                {
                    Debug.LogWarning("[GlobalMaterialController] materialSlotIndex must be >= 0.");
                    continue;
                }

                if (useSharedMaterial)
                {
                    var arr = r.sharedMaterials;
                    if (materialSlotIndex >= arr.Length)
                    {
                        Debug.LogWarning($"[GlobalMaterialController] Slot {materialSlotIndex} out of range for {r.name} (len={arr.Length}).");
                        continue;
                    }
                    arr[materialSlotIndex] = mat;
                    r.sharedMaterials = arr;
                }
                else
                {
                    var arr = r.materials;
                    if (materialSlotIndex >= arr.Length)
                    {
                        Debug.LogWarning($"[GlobalMaterialController] Slot {materialSlotIndex} out of range for {r.name} (len={arr.Length}).");
                        continue;
                    }
                    arr[materialSlotIndex] = mat;
                    r.materials = arr;
                }
            }
        }
    }

    /// <summary>
    /// Snapshot CURRENT materials of all targets as defaults (for Reset).
    /// </summary>
    [ContextMenu("Capture CURRENT as Default (All Targets)")]
    public void CaptureCurrentAsDefault()
    {
        _defaultsPerRenderer.Clear();
        if (targetRenderers == null) return;

        foreach (var r in targetRenderers)
        {
            if (r == null) { _defaultsPerRenderer.Add(null); continue; }
            var arr = useSharedMaterial ? r.sharedMaterials : r.materials;
            _defaultsPerRenderer.Add(arr != null ? arr.ToArray() : null);
        }
        Debug.Log("[GlobalMaterialController] Captured defaults for all targets.");
    }

    /// <summary>
    /// Reset ALL targets back to their captured defaults.
    /// </summary>
    public void ResetAllToDefault()
    {
        if (targetRenderers == null || _defaultsPerRenderer == null) return;

        int n = Mathf.Min(targetRenderers.Length, _defaultsPerRenderer.Count);
        for (int i = 0; i < n; i++)
        {
            var r = targetRenderers[i];
            var snapshot = _defaultsPerRenderer[i];
            if (r == null || snapshot == null) continue;

            if (useSharedMaterial) r.sharedMaterials = snapshot.ToArray();
            else r.materials = snapshot.ToArray();
        }
    }

    /// <summary>
    /// Utility: cycle to next named option for quick testing.
    /// </summary>
    public void NextOption()
    {
        if (options == null || options.Count == 0) return;
        // Find first option that differs from the first renderer’s current slot (rough heuristic)
        var next = options[0].material;
        ApplyMaterialToAll(next);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VRUIP
{
    public class WallInteractionController : MonoBehaviour
    {
        [Header("Wall Materials")]
        public Material wallAMaterial;     // Material for wall option A Oak
        public Material wallBMaterial;     // Material for wall option B Bamboo
        public Material wallCMaterial;     // Material for wall option C Stained Oak

        [Header("Material Change Event")]
        public UnityEvent<Material> OnMaterialChanged = new UnityEvent<Material>();

        private Renderer wallRenderer;
        public Material currentWallMaterial;
        public int selectedWallMaterialIndex; // Index of selected base material
        private Material[] materials;
        private Color originalColor;

        [SerializeField]
        private MaterialSoundAbsorptionManager absorptionManager;

        void Start()
        {
            wallRenderer = GetComponent<Renderer>();

            if (wallRenderer != null && wallRenderer.materials.Length > 0)
            {
                materials = wallRenderer.materials;

                // Cache original color from element 0's BaseColor before instantiation
                originalColor = materials[0].GetColor("_BaseColor");

                // Work on a runtime instance so changes won't affect the shared asset
                currentWallMaterial = Instantiate(materials[0]);
                currentWallMaterial.name = materials[0].name; // keep clean name (no "(Clone)")
                materials[0] = currentWallMaterial;
                wallRenderer.materials = materials;

                // Register the object with the absorption manager
                absorptionManager?.RegisterObject(gameObject, currentWallMaterial);
            }
            else
            {
                DebugManager.Instance?.LogError("Renderer materials not found or Element 0 is missing.");
            }
        }

        private void OnDisable()
        {
            if (currentWallMaterial != null)
            {
                // Revert BaseColor to the original when disabled (optional safeguard)
                currentWallMaterial.SetColor("_BaseColor", originalColor);
                DebugManager.Instance?.Log($"Reverted {currentWallMaterial.name} to original color: {originalColor}");
            }
        }

        // Method to change the material of the wall (A/B/C selector)
        public void OnMaterialSelected(int materialIndex)
        {
            Material newMaterial = null;

            switch (materialIndex)
            {
                case 0:
                    newMaterial = wallAMaterial;
                    break;
                case 1:
                    newMaterial = wallBMaterial;
                    break;
                case 2:
                    newMaterial = wallCMaterial;
                    break;
                default:
                    DebugManager.Instance?.LogWarning("Invalid material index.");
                    return;
            }

            // Replace element 0 with a fresh runtime instance of the selected material
            originalColor = newMaterial.GetColor("_BaseColor");
            selectedWallMaterialIndex = materialIndex;
            currentWallMaterial = Instantiate(newMaterial);
            currentWallMaterial.name = newMaterial.name;
            materials[0] = currentWallMaterial;
            wallRenderer.materials = materials;

            // Notify listeners
            OnMaterialChanged.Invoke(currentWallMaterial);
            absorptionManager?.UpdateMaterial(gameObject, currentWallMaterial);
        }

        // ---------- NEW: Color update from UI Button / Graphic ----------

        /// <summary>
        /// Call this from a Button's OnClick, passing the button's Image (or any UI Graphic).
        /// It reads the Graphic's color and applies it to the wall material's _BaseColor.
        /// </summary>
        public void ApplyColorFromGraphic(Graphic colorSource)
        {
            if (colorSource == null)
            {
                DebugManager.Instance?.LogWarning("ApplyColorFromGraphic: colorSource is null.");
                return;
            }
            ApplyColor(colorSource.color);
        }

        /// <summary>
        /// Directly apply a given color to the current wall material.
        /// </summary>
        public void ApplyColor(Color newColor)
        {
            if (currentWallMaterial == null)
            {
                DebugManager.Instance?.LogWarning("ApplyColor: currentWallMaterial is null.");
                return;
            }

            // Update Base Map color (_BaseColor is the HDRP/URP Lit property name)
            currentWallMaterial.SetColor("_BaseColor", newColor);
            DebugManager.Instance?.Log($"Updated wall material color to: R={newColor.r:F2}, G={newColor.g:F2}, B={newColor.b:F2}");

            // If your acoustic model depends on color/material, keep manager in sync as needed
            absorptionManager?.UpdateMaterial(gameObject, currentWallMaterial);
        }

        // Tiling control kept as-is
        public void UpdateTiling(float tilingValue)
        {
            if (currentWallMaterial != null)
            {
                currentWallMaterial.SetTextureScale("_BaseMap", new Vector2(tilingValue, tilingValue));
                DebugManager.Instance?.Log($"Updated wall material tiling to: X={tilingValue}, Y={tilingValue}");
            }
        }
    }
}

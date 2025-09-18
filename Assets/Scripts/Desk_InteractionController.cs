using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace VRUIP
{
    public class DeskInteractionController : MonoBehaviour
    {
        [Header("Desk Materials")]
        public Material deskAMaterial;     // Material for desk option A Oak
        public Material deskBMaterial;     // Material for desk option B Bamboo
        public Material deskCMaterial;     // Material for desk option C Stained Oak

        [Header("Material Change Event")]
        public UnityEvent<Material> OnMaterialChanged = new UnityEvent<Material>();

        private Renderer deskRenderer;
        public Material currentDeskMaterial;          // Runtime instance being edited
        public int selectedDeskMaterialIndex;         // Selected base material index
        private Material[] materials;
        private Color originalColor;

        [SerializeField]
        private MaterialSoundAbsorptionManager absorptionManager;

        void Start()
        {
            deskRenderer = GetComponent<Renderer>();

            if (deskRenderer != null && deskRenderer.materials.Length > 0)
            {
                materials = deskRenderer.materials;

                // Cache original color from element 0's BaseColor before instantiation
                originalColor = materials[0].GetColor("_BaseColor");

                // Work on a runtime instance so changes won't affect the shared asset
                currentDeskMaterial = Instantiate(materials[0]);
                currentDeskMaterial.name = materials[0].name; // keep clean name
                materials[0] = currentDeskMaterial;
                deskRenderer.materials = materials;

                // Register with absorption manager if used
                absorptionManager?.RegisterObject(gameObject, currentDeskMaterial);
            }
            else
            {
                DebugManager.Instance?.LogError("Renderer materials not found or Element 0 is missing.");
            }
        }

        private void OnDisable()
        {
            if (currentDeskMaterial != null)
            {
                // Revert BaseColor when disabled (optional safeguard)
                currentDeskMaterial.SetColor("_BaseColor", originalColor);
                DebugManager.Instance?.Log($"Reverted {currentDeskMaterial.name} to original color: {originalColor}");
            }
        }

        // ---------------- Base material (A/B/C) selection ----------------

        /// <summary>
        /// Swap the desk's base material (0=A, 1=B, 2=C). Keeps working on a runtime instance.
        /// </summary>
        public void OnMaterialSelected(int materialIndex)
        {
            Material newMaterial = null;

            switch (materialIndex)
            {
                case 0: newMaterial = deskAMaterial; break;
                case 1: newMaterial = deskBMaterial; break;
                case 2: newMaterial = deskCMaterial; break;
                default:
                    DebugManager.Instance?.LogWarning("Invalid material index.");
                    return;
            }

            // Replace element 0 with a fresh runtime instance of the selected material
            originalColor = newMaterial.GetColor("_BaseColor");
            selectedDeskMaterialIndex = materialIndex;

            currentDeskMaterial = Instantiate(newMaterial);
            currentDeskMaterial.name = newMaterial.name;
            materials[0] = currentDeskMaterial;
            deskRenderer.materials = materials;

            // Notify listeners and absorption manager
            OnMaterialChanged.Invoke(currentDeskMaterial);
            absorptionManager?.UpdateMaterial(gameObject, currentDeskMaterial);
        }

        // ---------------- Color application from UI buttons ----------------

        /// <summary>
        /// Call this from a Button's OnClick, passing the button's Image (or any UI Graphic).
        /// It reads the Graphic's color and applies it to _BaseColor.
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
        /// Directly apply a given color to the current desk material's _BaseColor.
        /// </summary>
        public void ApplyColor(Color newColor)
        {
            if (currentDeskMaterial == null)
            {
                DebugManager.Instance?.LogWarning("ApplyColor: currentDeskMaterial is null.");
                return;
            }

            currentDeskMaterial.SetColor("_BaseColor", newColor);
            DebugManager.Instance?.Log(
                $"Updated desk material color to: R={newColor.r:F2}, G={newColor.g:F2}, B={newColor.b:F2}"
            );

            // Keep absorption manager in sync if needed
            absorptionManager?.UpdateMaterial(gameObject, currentDeskMaterial);
        }

        // ---------------- Optional: texture tiling control ----------------

        /// <summary>
        /// Uniformly update _BaseMap tiling (X=Y=tilingValue).
        /// </summary>
        public void UpdateTiling(float tilingValue)
        {
            if (currentDeskMaterial != null)
            {
                currentDeskMaterial.SetTextureScale("_BaseMap", new Vector2(tilingValue, tilingValue));
                DebugManager.Instance?.Log($"Updated desk material tiling to: X={tilingValue}, Y={tilingValue}");
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace VRUIP
{
    public class FloorInteractionController : MonoBehaviour
    {
        [Header("Floor Materials")]
        public Material floorAMaterial;     // Material for floor option A
        public Material floorBMaterial;     // Material for floor option B
        public Material floorCMaterial;     // Material for floor option C

        [Header("Material Change Event")]
        public UnityEvent<Material> OnMaterialChanged = new UnityEvent<Material>();

        private Renderer floorRenderer;
        public Material currentFloorMaterial;         // Runtime instance being edited
        public int selectedFloorMaterialIndex;        // Selected base material index
        private Material[] materials;
        private Color originalColor;

        [SerializeField]
        private MaterialSoundAbsorptionManager absorptionManager;

        void Start()
        {
            floorRenderer = GetComponent<Renderer>();

            if (floorRenderer != null && floorRenderer.materials.Length > 0)
            {
                materials = floorRenderer.materials;

                // Cache original color
                originalColor = materials[0].GetColor("_BaseColor");

                // Work on a runtime instance so changes won't affect the shared asset
                currentFloorMaterial = Instantiate(materials[0]);
                currentFloorMaterial.name = materials[0].name;
                materials[0] = currentFloorMaterial;
                floorRenderer.materials = materials;

                // Register with absorption manager if used
                absorptionManager?.RegisterObject(gameObject, currentFloorMaterial);
            }
            else
            {
                DebugManager.Instance?.LogError("Renderer materials not found or Element 0 is missing.");
            }
        }

        private void OnDisable()
        {
            if (currentFloorMaterial != null)
            {
                // Revert BaseColor when disabled
                currentFloorMaterial.SetColor("_BaseColor", originalColor);
                DebugManager.Instance?.Log($"Reverted {currentFloorMaterial.name} to original color: {originalColor}");
            }
        }

        // ---------------- Base material (A/B/C) selection ----------------

        public void OnMaterialSelected(int materialIndex)
        {
            Material newMaterial = null;

            switch (materialIndex)
            {
                case 0: newMaterial = floorAMaterial; break;
                case 1: newMaterial = floorBMaterial; break;
                case 2: newMaterial = floorCMaterial; break;
                default:
                    DebugManager.Instance?.LogWarning("Invalid material index.");
                    return;
            }

            // Replace with a fresh runtime instance
            originalColor = newMaterial.GetColor("_BaseColor");
            selectedFloorMaterialIndex = materialIndex;

            currentFloorMaterial = Instantiate(newMaterial);
            currentFloorMaterial.name = newMaterial.name;
            materials[0] = currentFloorMaterial;
            floorRenderer.materials = materials;

            // Notify listeners / manager
            OnMaterialChanged.Invoke(currentFloorMaterial);
            absorptionManager?.UpdateMaterial(gameObject, currentFloorMaterial);
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
        /// Directly apply a given color to the current floor material's _BaseColor.
        /// </summary>
        public void ApplyColor(Color newColor)
        {
            if (currentFloorMaterial == null)
            {
                DebugManager.Instance?.LogWarning("ApplyColor: currentFloorMaterial is null.");
                return;
            }

            currentFloorMaterial.SetColor("_BaseColor", newColor);
            DebugManager.Instance?.Log(
                $"Updated floor material color to: R={newColor.r:F2}, G={newColor.g:F2}, B={newColor.b:F2}"
            );

            absorptionManager?.UpdateMaterial(gameObject, currentFloorMaterial);
        }

        // ---------------- Optional: tiling control ----------------

        public void UpdateTiling(float tilingValue)
        {
            if (currentFloorMaterial != null)
            {
                currentFloorMaterial.SetTextureScale("_BaseMap", new Vector2(tilingValue, tilingValue));
                DebugManager.Instance?.Log($"Updated floor material tiling to: X={tilingValue}, Y={tilingValue}");
            }
        }
    }
}

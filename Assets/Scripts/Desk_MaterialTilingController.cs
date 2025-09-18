using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VRUIP
{
    public class DeskMaterialTilingController : MonoBehaviour
    {
        [Header("Target Object")]
        public DeskInteractionController deskInteractionController; // Reference to deskInteractionController

        [Header("UI Components")]
        public Slider tilingSlider;       // Slider to adjust tiling
        public Button increaseButton;     // "+" button
        public Button decreaseButton;     // "-" button
        public TMP_Text tilingValueText;  // Text to display current tiling value

        [Header("Tiling Range")]
        public float minTiling = 0.1f;    // Minimum tiling value
        public float maxTiling = 5f;      // Maximum tiling value
        public float step = 0.1f;         // Step value for buttons

        private Material currentdeskMaterial;

        private void Start()
        {
            if (deskInteractionController == null)
            {
                DebugManager.Instance?.LogError("deskInteractionController reference is missing.");
                return;
            }

            // Subscribe to material change event
            deskInteractionController.OnMaterialChanged.AddListener(OnMaterialChanged);

            // Initialize UI with the current material
            InitializeUI(deskInteractionController.currentDeskMaterial);
        }

        private void OnDestroy()
        {
            // Unsubscribe from material change event
            if (deskInteractionController != null)
            {
                deskInteractionController.OnMaterialChanged.RemoveListener(OnMaterialChanged);
            }
        }

        private void OnMaterialChanged(Material newMaterial)
        {
            currentdeskMaterial = newMaterial;
            InitializeUI(currentdeskMaterial);
        }

        private void InitializeUI(Material material)
        {
            if (material != null)
            {
                currentdeskMaterial = material;
                Vector2 initialTiling = currentdeskMaterial.GetTextureScale("_BaseMap");

                if (tilingSlider != null)
                {
                    tilingSlider.minValue = minTiling;
                    tilingSlider.maxValue = maxTiling;
                    tilingSlider.value = initialTiling.x;
                    tilingSlider.onValueChanged.RemoveAllListeners();
                    tilingSlider.onValueChanged.AddListener(OnSliderValueChanged);
                }

                if (increaseButton != null)
                {
                    increaseButton.onClick.RemoveAllListeners();
                    increaseButton.onClick.AddListener(() => AdjustTiling(step));
                }

                if (decreaseButton != null)
                {
                    decreaseButton.onClick.RemoveAllListeners();
                    decreaseButton.onClick.AddListener(() => AdjustTiling(-step));
                }

                UpdateTilingValueText(initialTiling.x);
            }
        }

        private void OnSliderValueChanged(float value)
        {
            SetTiling(value);
        }

        private void AdjustTiling(float amount)
        {
            if (tilingSlider != null)
            {
                float newValue = Mathf.Clamp(tilingSlider.value + amount, minTiling, maxTiling);
                tilingSlider.value = newValue;
            }
        }

        private void SetTiling(float value)
        {
            if (currentdeskMaterial != null)
            {
                currentdeskMaterial.SetTextureScale("_BaseMap", new Vector2(value, value));
                DebugManager.Instance?.Log($"Tiling updated: X={value}, Y={value}");

                // Update the tiling in deskInteractionController
                deskInteractionController.UpdateTiling(value);
            }
            else
            {
                DebugManager.Instance?.LogError("Current material is null. Cannot set tiling.");
            }

            UpdateTilingValueText(value);
        }

        private void UpdateTilingValueText(float value)
        {
            if (tilingValueText != null)
            {
                tilingValueText.text = value.ToString("0.0");
            }
        }
    }
}
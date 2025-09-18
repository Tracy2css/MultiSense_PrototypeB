using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{

    public class AuditoryAdjustmentController : MonoBehaviour
    {
        public SharedMaterialState sharedState; // Reference to shared state for materials

        public Button[] materialButtons; // Buttons for material selection
        public Toggle soundAbsorptionToggle; // Toggle for activating sound absorption

        // private int totalWallMaterials = 3; // Hardcoded for wall materials (A, B, C)

        void Start()
        {
            // Initialize buttons based on shared state
            InitializeButtons();

            // Add listener for sound absorption toggle
            soundAbsorptionToggle.onValueChanged.AddListener(ToggleSoundAbsorption);

            // Listen to shared state updates
            sharedState.OnMaterialUpdated += UpdateAuditoryUI;
        }

        private void InitializeButtons()
        {
            for (int i = 0; i < materialButtons.Length; i++)
            {
                int index = i; // Capture current index for listener
                materialButtons[i].onClick.AddListener(() => OnMaterialSelected(index));
            }
        }

        private void OnMaterialSelected(int materialIndex)
        {
            string materialName = materialIndex switch
            {
                0 => "A",
                1 => "B",
                2 => "C",
                _ => null
            };

            if (materialName != null)
            {
                Material newMaterial = materialName switch
                {
                    "A" => sharedState.currentMaterial,
                    "B" => sharedState.currentMaterial,
                    "C" => sharedState.currentMaterial,
                    _ => null
                };

                Color newColor = sharedState.currentColor;
                sharedState.UpdateMaterial(newMaterial, newColor);
                Debug.Log($"Auditory Adjustment - Material {materialName} selected.");
            }
        }

        private void UpdateAuditoryUI(Material newMaterial, Color newColor)
        {
            Debug.Log($"Auditory tab updated: Material = {newMaterial?.name}, Color = {newColor}");
        }

        private void ToggleSoundAbsorption(bool isOn)
        {
            if (isOn)
            {
                Debug.Log("Sound absorption activated.");
            }
            else
            {
                Debug.Log("Sound absorption deactivated.");
            }
        }
    }
}

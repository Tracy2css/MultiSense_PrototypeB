using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Events;

namespace VRUIP
{

    public class winddowInteractionController : MonoBehaviour
    {
        [Header("winddow Materials")]
        public Material winddowAMaterial;     // Material for winddow option A
        public Material winddowBMaterial;     // Material for winddow option B
        public Material winddowCMaterial;     // Material for winddow option C

        [Header("Color Picker")]
        public ColorPickerController colorPicker; // Reference to the ColorPickerController

        [Header("Material Change Event")]
        public UnityEvent<Material> OnMaterialChanged; // Event to notify material change

        private Renderer winddowRenderer;
        public Material currentwinddowMaterial { get; private set; }
        public int selectedwinddowMaterialIndex { get; private set; } // Index of selected base material
        private Material[] materials;
        private Color originalColor;


        void Start()
        {
            winddowRenderer = GetComponent<Renderer>();
            if (winddowRenderer != null && winddowRenderer.materials.Length > 0)
            {
                materials = winddowRenderer.materials;
                originalColor = materials[0].GetColor("_BaseColor"); // Get original color before instantiation
                currentwinddowMaterial = Instantiate(materials[0]);
                materials[0] = currentwinddowMaterial;
                winddowRenderer.materials = materials;

                colorPicker?.onColorChanged.AddListener(OnColorChanged);
            }
            else
            {
                Debug.LogError("Renderer materials not found or Element 0 is missing.");
            }
        }

        private void OnDisable()
        {
            if (currentwinddowMaterial != null)
            {
                // Revert the material's Base Map color to the original
                currentwinddowMaterial.SetColor("_BaseColor", originalColor);
                Debug.Log($"Reverted {currentwinddowMaterial.name} to original color: {originalColor}");
            }
        }

        void OnDestroy()
        {
            // Unsubscribe from the ColorPicker's onColorChanged event
            if (colorPicker != null)
            {
                colorPicker.onColorChanged.RemoveListener(OnColorChanged);
            }
        }

        // Method to change the material of the winddow
        public void OnMaterialSelected(int materialIndex)
        {

            Material newMaterial = null;

            switch (materialIndex)
            {
                case 0:
                    newMaterial = winddowAMaterial;
                    break;
                case 1:
                    newMaterial = winddowBMaterial;
                    break;
                case 2:
                    newMaterial = winddowCMaterial;
                    break;
                default:
                    Debug.LogWarning("Invalid material index.");
                    return;
            }

            // Replace the material at Element 0 with a runtime instance of the new material
            originalColor = newMaterial.GetColor("_BaseColor"); // Get original color before instantiation
                    // Save the selected material index
            selectedwinddowMaterialIndex = materialIndex;
            currentwinddowMaterial = Instantiate(newMaterial);
            materials[0] = currentwinddowMaterial;
            winddowRenderer.materials = materials; // Update the Renderer with the new materials array

            // Update the Color Picker with the new material's Base Map color
            if (colorPicker != null)
            {
                colorPicker.SetInitialColor(originalColor);

                // Trigger the event to notify about the material change
                OnMaterialChanged?.Invoke(currentwinddowMaterial);
            }

            // Save the material selection to CSV with the updated Base Map color
            Color colorToSave = currentwinddowMaterial.GetColor("_BaseColor");
            // SaveMaterialChoiceToCSV(currentwinddowMaterial.name, colorToSave);
        }

        // Method to update the color of the current material
        private void OnColorChanged(Color newColor)
        {
            if (currentwinddowMaterial != null)
            {
                currentwinddowMaterial.SetColor("_BaseColor", newColor);
                Debug.Log($"Updated winddow material color to: R={newColor.r:F2}, G={newColor.g:F2}, B={newColor.b:F2}");
                // Save immediately after color change if needed
                // SaveMaterialChoiceToCSV(currentwinddowMaterial.name, newColor);
            }
        }
        // Method to save the material choice to a CSV file
        //     private void SaveMaterialChoiceToCSV(string materialName, Color currentColor)
        //     {
        //         // Directory path where the files are stored
        //         string directoryPath = "F:/00/Test00/";
        //         string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmm");
        //         string filePath = $"{directoryPath}MaterialChoices_{timestamp}.csv";

        //         // Ensure the directory exists
        //         CreateDirectoryIfNotExists(directoryPath);

        //         // Delete all existing files in the directory before creating a new one
        //         DeleteOldFiles(directoryPath);

        //         // Format the color as a string
        //         string colorString = $"{currentColor.r:F2},{currentColor.g:F2},{currentColor.b:F2}";
        //         Debug.Log($"Color being saved: R={currentColor.r:F2}, G={currentColor.g:F2}, B={currentColor.b:F2}");

        //         // Write the latest selected material, current timestamp, and color to the CSV file
        //         string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //         string csvContent = "Timestamp,Selected Material,Color (R,G,B)\n";
        //         csvContent += $"{timeStamp},{materialName},{colorString}\n";
        //         File.WriteAllText(filePath, csvContent);

        //         Debug.Log($"Saved material choice: {materialName}, color: {colorString} at {timeStamp}");
        //     }

        // // Method to delete old files in the directory
        //     private void DeleteOldFiles(string directoryPath)
        //     {
        //         if (Directory.Exists(directoryPath))
        //         {
        //             string[] files = Directory.GetFiles(directoryPath, "MaterialwinddowChoices_*.csv");
        //             foreach (string file in files)
        //             {
        //                 File.Delete(file);
        //             }
        //             Debug.Log($"Deleted {files.Length} old files in {directoryPath}");
        //         }
        //         else
        //         {
        //             Debug.LogWarning($"Directory not found: {directoryPath}");
        //         }
        //     }

        //     // Method to create the directory if it does not exist
        //     private void CreateDirectoryIfNotExists(string directoryPath)
        //     {
        //         if (!Directory.Exists(directoryPath))
        //         {
        //             Directory.CreateDirectory(directoryPath);
        //             Debug.Log($"Directory created: {directoryPath}");
        //         }
        //     }
    }
}
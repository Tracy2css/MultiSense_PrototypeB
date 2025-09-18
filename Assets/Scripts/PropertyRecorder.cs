using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class RuntimePropertyRecorder : MonoBehaviour
{
    [System.Serializable]
    public class ObjectPropertySettings
    {
        public GameObject targetObject; // The object to record
        public bool recordActiveState;
        public bool recordMaterial;
        public bool recordPosition;
        public bool recordRotation;
        public bool recordScale;
        public bool recordLightState; // NEW: Record Point Light activation state
    }

    [Header("Object Settings")]
    public List<ObjectPropertySettings> monitoredObjects = new List<ObjectPropertySettings>(); // List of object settings

    [Header("Output Settings")]
    public string outputFileName = "RuntimeProperties.csv"; // Output CSV file name
    public string outputFilePath = "Assets/Logs/";          // Directory for output file

    private void Start()
    {
        // Ensure the output directory exists
        if (!Directory.Exists(outputFilePath))
        {
            Directory.CreateDirectory(outputFilePath);
        }
    }

    private void OnApplicationQuit()
    {
        ExportPropertiesToCsv();
    }

    public void ExportPropertiesToCsv()
    {
        string fullPath = Path.Combine(outputFilePath, outputFileName);

        using (StreamWriter writer = new StreamWriter(fullPath))
        {
            // Write CSV header
            writer.WriteLine("ObjectName,PropertyName,PropertyValue");

            // Iterate over each monitored object
            foreach (var settings in monitoredObjects)
            {
                if (settings.targetObject == null) continue;

                string objectName = settings.targetObject.name;

                // Record active state
                if (settings.recordActiveState)
                {
                    writer.WriteLine($"{objectName},ActiveState,{settings.targetObject.activeSelf}");
                }

                // Record material name
                if (settings.recordMaterial)
                {
                    Renderer renderer = settings.targetObject.GetComponent<Renderer>();
                    if (renderer != null && renderer.material != null)
                    {
                        string materialName = renderer.material.name.Replace("(Clone)", "").Trim();
                        writer.WriteLine($"{objectName},Material,{materialName}");
                    }
                }

                // Record position
                if (settings.recordPosition)
                {
                    writer.WriteLine($"{objectName},Position,{settings.targetObject.transform.position}");
                }

                // Record rotation
                if (settings.recordRotation)
                {
                    writer.WriteLine($"{objectName},Rotation,{settings.targetObject.transform.rotation.eulerAngles}");
                }

                // Record scale
                if (settings.recordScale)
                {
                    writer.WriteLine($"{objectName},Scale,{settings.targetObject.transform.localScale}");
                }

                // Record Light state (NEW)
                if (settings.recordLightState)
                {
                    Light light = settings.targetObject.GetComponent<Light>();
                    if (light != null)
                    {
                        writer.WriteLine($"{objectName},LightState,{light.enabled}");
                    }
                }
            }
        }

        Debug.Log($"Runtime properties exported to: {fullPath}");
    }
}

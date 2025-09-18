using UnityEngine;
using System.Collections.Generic;

public class MaterialSoundAbsorptionManager : MonoBehaviour
{
    [Header("Audio Components")]
    public List<AudioSource> audioSources;

    [Header("Material Absorption Settings")]
    public List<MaterialAbsorptionSetting> materialAbsorptionSettings = new List<MaterialAbsorptionSetting>();

    private Dictionary<GameObject, Material> objectMaterialMap = new Dictionary<GameObject, Material>();
    private Dictionary<string, float> materialAbsorptionByName = new Dictionary<string, float>();

    private void Awake()
    {
        foreach (var setting in materialAbsorptionSettings)
        {
            if (setting.material != null)
            {
                string materialName = NormalizeMaterialName(setting.material.name);
                materialAbsorptionByName[materialName] = setting.absorptionCoefficient;
                DebugManager.Instance?.Log($"Configured material: {materialName}, Coefficient: {setting.absorptionCoefficient}");
            }
        }
    }

    public void RegisterObject(GameObject obj, Material material)
    {
        string materialName = NormalizeMaterialName(material.name);
        DebugManager.Instance?.Log($"Registering object: {obj.name} with material: {materialName}");

        if (!objectMaterialMap.ContainsKey(obj))
        {
            objectMaterialMap[obj] = material;
            UpdateSoundAbsorption();
        }
        else
        {
            DebugManager.Instance?.LogWarning($"Object {obj.name} is already registered.");
        }
    }

    public void UpdateMaterial(GameObject obj, Material newMaterial)
    {
        string materialName = NormalizeMaterialName(newMaterial.name);
        DebugManager.Instance?.Log($"Updating material for object: {obj.name} to {materialName}");

        if (objectMaterialMap.ContainsKey(obj))
        {
            objectMaterialMap[obj] = newMaterial;

            foreach (var entry in materialAbsorptionByName)
            {
                DebugManager.Instance?.Log($"Dictionary Entry: {entry.Key}, Coefficient: {entry.Value}");
            }

            UpdateSoundAbsorption();
        }
        else
        {
            DebugManager.Instance?.LogWarning($"Object {obj.name} is not registered. Cannot update material.");
        }
    }

    private void UpdateSoundAbsorption()
    {
        float totalAbsorption = 1f;
        List<string> contributingMaterials = new List<string>();

        foreach (var kvp in objectMaterialMap)
        {
            Material material = kvp.Value;
            string materialName = NormalizeMaterialName(material.name);

            DebugManager.Instance?.Log($"Checking material: {materialName}");
            if (materialAbsorptionByName.TryGetValue(materialName, out float coefficient))
            {
                totalAbsorption *= (1 - coefficient);
                if (!contributingMaterials.Contains(materialName))
                {
                    contributingMaterials.Add(materialName);
                }
            }
            else
            {
                DebugManager.Instance?.LogWarning($"Material {materialName} does not match any known absorption settings.");
            }
        }

        totalAbsorption = 1 - totalAbsorption;

        foreach (var source in audioSources)
        {
            if (source != null)
            {
                source.volume = Mathf.Clamp01(1 - totalAbsorption);
            }
        }

        string materialNames = string.Join(", ", contributingMaterials);
        DebugManager.Instance?.Log($"Total absorption (non-linear): {totalAbsorption}");
        DebugManager.Instance?.Log($"Contributing materials: {materialNames}");
    }

    private string NormalizeMaterialName(string materialName)
    {
        return materialName.Replace("(Clone)", "").Replace("(Instance)", "").Trim();
    }
}

[System.Serializable]
public class MaterialAbsorptionSetting
{
    public Material material;
    [Range(0f, 1f)]
    public float absorptionCoefficient;
}

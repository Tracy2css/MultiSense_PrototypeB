using UnityEngine;

public class SharedMaterialState : MonoBehaviour
{
    public Material currentMaterial; 
    public Color currentColor;       

    public delegate void MaterialUpdated(Material newMaterial, Color newColor);
    public event MaterialUpdated OnMaterialUpdated;

    public void UpdateMaterial(Material newMaterial, Color newColor)
    {
        currentMaterial = newMaterial;
        currentColor = newColor;

        // Update the material of the object
        GetComponent<Renderer>().material = newMaterial;
        GetComponent<Renderer>().material.color = newColor;

        OnMaterialUpdated?.Invoke(newMaterial, newColor);
    }
}
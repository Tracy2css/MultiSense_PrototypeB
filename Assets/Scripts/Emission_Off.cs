using UnityEngine;

public class DisableEmissionOnStart : MonoBehaviour
{
    public Material targetMaterial; // Drag the material that needs Emission to be turned off

    void Start()
    {
        if (targetMaterial != null)
        {
            // Disable the Emission keyword
            targetMaterial.DisableKeyword("_EMISSION");

            // Set the Emission color to black
            targetMaterial.SetColor("_EmissionColor", Color.white);

            // Ensure the material is updated
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = targetMaterial;
            }
        }
        else
        {
            Debug.LogWarning("No material specified, please drag the target material in the Inspector panel!");
        }
    }
}

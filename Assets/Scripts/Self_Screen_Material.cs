using UnityEngine;

public class ScreenMaterialSwitcher : MonoBehaviour
{
    public Material materialInitial; // Initial material for the object
    public Material materialB;    
    public MeshRenderer targetMeshRenderer;

    public void ScreenSwitchMaterial()
    {
        if (targetMeshRenderer != null && materialB != null)
        {
            targetMeshRenderer.material = materialB;
        }
    }
}

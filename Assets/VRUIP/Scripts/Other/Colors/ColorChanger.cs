using UnityEngine;

namespace VRUIP
{
    /// <summary>
    /// This class is for changing the color of an object with a MeshRenderer.
    /// </summary>
    public class ColorChanger : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MeshRenderer meshRenderer;
        
        public void ChangeColor(Color color)
        {
            meshRenderer.material.color = color;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRUIP
{
    /// <summary>
    /// This class is for changing the color of an object with a SpriteRenderer.
    /// </summary>
    public class ColorChangerSprite : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        public void ChangeColorSprite(Color color)
        {
            spriteRenderer.color = color;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class ImageController : A_ColorController
    {
        [SerializeField] private Image image;
        protected override void SetColors(ColorTheme theme)
        {
            image.color = theme.secondaryColor;
        }
    }
}

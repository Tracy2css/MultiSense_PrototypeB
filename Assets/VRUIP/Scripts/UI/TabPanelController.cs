using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class TabPanelController : A_ColorController
    {
        [Header("Components")]
        [SerializeField] private Image background;

        protected override void SetColors(ColorTheme theme)
        {
            background.color = theme.primaryColor;
        }
        
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}

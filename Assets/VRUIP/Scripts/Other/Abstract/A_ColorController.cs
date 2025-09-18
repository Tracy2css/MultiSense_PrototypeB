using UnityEngine;

namespace VRUIP
{
    public abstract class A_ColorController : MonoBehaviour
    {
        public bool followsTheme = true;

        protected virtual void Start()
        {
            RegisterElement();
            SetupElement(VRUIPManager.instance.CurrentColorTheme);
        }

        /// <summary>
        /// Setup the colors of this element based on a color theme.
        /// </summary>
        protected abstract void SetColors(ColorTheme theme);

        public void SetupElement(ColorTheme theme)
        {
            if (followsTheme)
            {
                SetColors(theme);
            }
        }

        /// <summary>
        /// Register this element with the VRUIPManager.
        /// </summary>
        private void RegisterElement()
        {
            VRUIPManager.instance.RegisterElement(this);
        }
    }
}

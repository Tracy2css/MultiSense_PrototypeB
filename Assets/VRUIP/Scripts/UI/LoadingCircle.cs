using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class LoadingCircle : A_ColorController
    {
        [Header("Properties")]
        [SerializeField] private LoadingCircleAnimation animationType;

        [Header("Components")]
        [SerializeField] private Image loadingImage1;
        [SerializeField] private Image loadingImage2;

        public bool Loading { get; set; } = true;
        
        private int speed = 1;
        private bool expanding = true;
        private bool shrinking = false;

        private void Awake()
        {
            SetupLoading();
        }

        private void Update()
        {
            if (!Loading) return;
            switch (animationType)
            {
                case LoadingCircleAnimation.Type1:
                    loadingImage1.transform.Rotate(Vector3.forward, -1f * speed);
                    loadingImage2.transform.Rotate(Vector3.forward, -1f * speed);
                    break;
                case LoadingCircleAnimation.Type2:
                    loadingImage1.transform.Rotate(Vector3.forward, -1f * speed);
                    break;
                case LoadingCircleAnimation.Type3:
                    if (expanding)
                    {
                        loadingImage1.fillAmount += 0.002f * speed;
                        loadingImage1.transform.Rotate(Vector3.forward, -0.36f * speed);
                        if (loadingImage1.fillAmount >= 0.9f)
                        {
                            expanding = false;
                            shrinking = true;
                        }
                    }
                    else if (shrinking)
                    {
                        loadingImage1.transform.Rotate(Vector3.forward, -0.36f * 4 * speed);
                        loadingImage1.fillAmount -= 0.002f * speed;
                        if (loadingImage1.fillAmount < 0.1f)
                        {
                            shrinking = false;
                            expanding = true;
                        }
                    }
                    break;
            }
        }

        private void SetupLoading()
        {
            speed = VRUIPManager.instance.IsVR ? 4 : 1;
            switch (animationType)
            {
                case LoadingCircleAnimation.Type1:
                    loadingImage1.fillAmount = 0.25f;
                    loadingImage2.fillAmount = 0.25f;
                    break;
                case LoadingCircleAnimation.Type2:
                    loadingImage2.gameObject.SetActive(false);
                    loadingImage1.fillAmount = 0.75f;
                    break;
                case LoadingCircleAnimation.Type3:
                    loadingImage2.gameObject.SetActive(false);
                    loadingImage1.fillAmount = 0.1f;
                    break;
            }
        }

        protected override void SetColors(ColorTheme theme)
        {
            loadingImage1.color = theme.fourthColor;
            loadingImage2.color = theme.fourthColor;
        }

        private enum LoadingCircleAnimation
        {
            Type1,
            Type2,
            Type3
        }
    }
}

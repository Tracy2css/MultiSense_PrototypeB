using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VRUIP
{
    public class SliderController : A_ColorController
    {
        [Header("Customize")]
        [SerializeField] private bool showPercentage;
        [SerializeField] private bool showText;
        [SerializeField] private bool showIcon;
        [SerializeField] [Tooltip("If clickable, clicking logo can set slider to 0 or 100%")]
        private bool iconClickable;
        [SerializeField] private bool showDecimals = true;
        [SerializeField] private string sliderTextValue;

        [Header("Colors")] 
        [SerializeField] private Color emptyColor;
        [SerializeField] private Color fillColor;
        [SerializeField] private Color pressedColor;
        [SerializeField] private Color textColor;

        [Header("Components")]
        [SerializeField] private Slider slider;
        [SerializeField] private IconController sliderIcon;
        [SerializeField] private TextMeshProUGUI percentageText;
        [SerializeField] private TextMeshProUGUI sliderText;
        [SerializeField] private Image emptyImage;
        [SerializeField] private Image fillImage;

        private void Awake()
        {
            SetupSlider();
        }

        protected override void SetColors(ColorTheme theme)
        {
            emptyImage.color = theme.thirdColor;
            fillImage.color = theme.fourthColor;
            sliderText.color = theme.secondaryColor;
            percentageText.color = theme.secondaryColor;
        }

        [ContextMenu("Setup Slider (VRUIP)")]
        private void SetupSlider()
        {
            sliderText.gameObject.SetActive(showText);
            sliderText.color = textColor;
            sliderText.text = sliderTextValue;
            sliderIcon.gameObject.SetActive(showIcon);
            sliderIcon.button.interactable = iconClickable;
            sliderIcon.interactable = iconClickable;
            sliderIcon.button.onClick.AddListener(LogoClicked);
            percentageText.gameObject.SetActive(showPercentage);
            percentageText.color = textColor;
            SetPercentageText(slider.value);
            slider.onValueChanged.AddListener(SetPercentageText);
            emptyImage.color = emptyColor;
            fillImage.color = fillColor;
        }

        private void SetPercentageText(float percentage)
        {
            var formattedNumber = percentage == 0 || Math.Abs(percentage - 1f) < 0.0001f  || !showDecimals ? ((int)(percentage * 100)).ToString() : (percentage * 100).ToString("N1");
            percentageText.text = formattedNumber + "%";
        }

        private void LogoClicked()
        {
            slider.value = slider.value != 0 ? 0 : 1;
        }

        /// <summary>
        /// Set the value of this slider.
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(float value, bool notify = true)
        {
            if (notify) slider.value = value;
            else slider.SetValueWithoutNotify(value);
            SetPercentageText(value);
        }

        public void RegisterOnChanged(UnityAction<float> action)
        {
            slider.onValueChanged.AddListener(action);
        }
    }
}

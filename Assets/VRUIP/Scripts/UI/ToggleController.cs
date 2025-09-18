using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VRUIP
{
    public class ToggleController : A_ColorController
    {
        [Header("Normal Colors")]
        [SerializeField] private Color textNormalColor = Color.black;
        [SerializeField] private Color boxNormalColor = Color.white;

        [Header("Selected Colors")]
        [SerializeField] private Color textSelectedColor = Color.green;
        [SerializeField] private Color boxSelectedColor = Color.gray;
        [SerializeField] private Color checkmarkColor = Color.black;

        [Header("Properties")] 
        [SerializeField] private bool crossOutSelected;
        [SerializeField] private AudioClip toggleSound;
        
        [Header("Components")]
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image boxImage;
        [SerializeField] private Image checkmarkImage;
        [SerializeField] private TextMeshProUGUI toggleText;
        [SerializeField] private AudioSource audioSource;
        
        public bool IsOn => toggle.isOn;

        private void Awake()
        {
            SetupToggle();
        }

        /// <summary>
        /// Setup this toggle.
        /// </summary>
        [ContextMenu("Setup Toggle (VRUIP)")]
        private void SetupToggle()
        {
            boxImage.color = boxNormalColor;
            toggleText.color = textNormalColor;
            checkmarkImage.color = checkmarkColor;
            toggle.onValueChanged.AddListener(HandleToggleChanged);
            audioSource.clip = toggleSound;
        }

        // Handle toggle value changed.
        private void HandleToggleChanged(bool on)
        {
            audioSource.Play();
            if (on)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }

        // Turn on toggle.
        private void TurnOn()
        {
            boxImage.color = boxSelectedColor;
            toggleText.color = textSelectedColor;
            if (crossOutSelected) toggleText.fontStyle = FontStyles.Strikethrough;
        }

        // Turn off toggle.
        private void TurnOff()
        {
            boxImage.color = boxNormalColor;
            toggleText.color = textNormalColor;
            if (crossOutSelected) toggleText.fontStyle = FontStyles.Normal;
        }

        protected override void SetColors(ColorTheme theme)
        {
            textNormalColor = theme.secondaryColor;
            textSelectedColor = theme.fourthColor;
            boxNormalColor = theme.secondaryColor;
            boxSelectedColor = theme.secondaryColor;
            checkmarkColor = theme.primaryColor;
            boxImage.color = IsOn ? boxSelectedColor : boxNormalColor;
            toggleText.color = IsOn ? textSelectedColor : textNormalColor;
            checkmarkImage.color = checkmarkColor;
        }

        /// <summary>
        /// Register a listener for when the toggle value changes.
        /// </summary>
        /// <param name="action"></param>
        public void RegisterOnToggleChanged(UnityAction<bool> action)
        {
            toggle.onValueChanged.AddListener(action);
        }
    }
}

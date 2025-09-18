using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VRUIP
{
    public class SwitchController : A_UIIntercations
    {
        [Header("Colors On")]
        [SerializeField] private Color backgroundOnColor = Color.green;
        [SerializeField] private Color circleOnColor = Color.white;
        
        [Header("Colors Off")]
        [SerializeField] private Color backgroundOffColor = Color.gray;
        [SerializeField] private Color circleOffColor = Color.white;

        [Header("Properties")]
        [SerializeField] [Tooltip("Enable text that tells you whether this switch is on or off.")]
        private bool enableStateText;
        [SerializeField] private bool customStateText;
        [SerializeField] private string onStateText = "ON";
        [SerializeField] private string offStateText = "OFF";
        [SerializeField] private AudioClip switchSound;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onTurnOn;
        [SerializeField] private UnityEvent onTurnOff;
        
        [Header("Components")]
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image background;
        [SerializeField] private Image circleImage;
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private AudioSource audioSource;

        public bool IsOn => toggle.isOn;

        private void Awake()
        {
            SetupSwitch();
        }

        // Setup this switch.
        [ContextMenu("Setup Switch (VRUIP)")]
        private void SetupSwitch()
        {
            circleImage.color = toggle.isOn ? circleOnColor : circleOffColor;
            background.color = toggle.isOn ? backgroundOnColor : backgroundOffColor;
            stateText.color = toggle.isOn ? backgroundOnColor : backgroundOffColor;
            stateText.text = customStateText ? (toggle.isOn ? onStateText : offStateText) : (toggle.isOn ? "ON" : "OFF");
            stateText.gameObject.SetActive(enableStateText);
            toggle.onValueChanged.AddListener(HandleToggleChanged);
            audioSource.clip = switchSound;
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
            circleImage.rectTransform.anchoredPosition = new Vector3(24f, 0, 0);
            background.color = backgroundOnColor;
            circleImage.color = circleOnColor;
            stateText.color = backgroundOnColor;
            stateText.text = customStateText ? onStateText : "ON";
            onTurnOn.Invoke();
        }

        // Turn off toggle.
        private void TurnOff()
        {
            circleImage.rectTransform.anchoredPosition = new Vector3(-24f, 0, 0);
            background.color = backgroundOffColor;
            circleImage.color = circleOffColor;
            stateText.color = backgroundOffColor;
            stateText.text = customStateText ? offStateText : "OFF";
            onTurnOff.Invoke();
        }

        protected override void SetColors(ColorTheme theme)
        {
            stateText.color = theme.secondaryColor;
            backgroundOffColor = theme.secondaryColor;
            backgroundOnColor = theme.fourthColor;
            background.color = toggle.isOn ? backgroundOnColor : backgroundOffColor;
        }
        
        /// <summary>
        /// Register a function to be called when the toggle value changes.
        /// </summary>
        /// <param name="action"></param>
        public void RegisterOnValueChanged(UnityAction<bool> action)
        {
            toggle.onValueChanged.AddListener(action);
        }
    }
}

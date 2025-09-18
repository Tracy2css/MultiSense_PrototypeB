using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace VRUIP
{
    public class IconController : A_UIIntercations
    {
        // COLOR PROPERTIES
        public Color iconNormalColor = Color.gray;
        public Color iconHoverColor = Color.white;
        public Color iconClickColor = Color.cyan;

        // ICON PROPERTIES
        [SerializeField] private Sprite iconSprite = null;
        [SerializeField] private bool clickSoundEnabled = true;
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip hoverSound;
        [SerializeField] private UnityEvent onClick = new();
        
        // ICON COMPONENTS
        public Image iconImage;
        public Button button;

        public Color IconColor
        {
            get => iconImage.color;
            set => iconImage.color = value;
        }
        
        private UnityEvent _animationHoverEvent = new();
        private UnityEvent _animationExitEvent = new();
        private AudioSource _audioSource;

        private void Awake()
        {
            SetupIcon();
        }

        /// <summary>
        /// Setup the button including all properties and functionalities from this script.
        /// </summary>
        [ContextMenu("Setup Icon (VRUIP)")]
        private void SetupIcon()
        {
            SetupButtonColors();
            RegisterPointerEvents();
            _audioSource = VRUIPManager.instance.AudioSource;
            if (clickSoundEnabled) button.onClick.AddListener(() => _audioSource.PlayOneShot(clickSound));
            button.onClick.AddListener(onClick.Invoke);
            if (iconSprite != null) iconImage.sprite = iconSprite;
        }

        /// <summary>
        /// Set up the button colors based on the colors assigned to the properties of this script.
        /// </summary>
        private void SetupButtonColors()
        {
            button.transition = Selectable.Transition.None;
            iconImage.color = iconNormalColor;
        }

        protected override void SetColors(ColorTheme theme)
        {
            iconNormalColor = theme.secondaryColor;
            iconHoverColor = theme.fourthColor;
            iconClickColor = theme.thirdColor;
            SetupButtonColors();
        }


        // ANIMATION FUNCTIONS ----------
        private void HoverAnimation()
        {
            _animationHoverEvent.Invoke();
        }

        private void ExitAnimation()
        {
            _animationExitEvent.Invoke();
        }

        // CUSTOM EVENTS ----------
        private void OnEnter()
        {
            iconImage.color = iconHoverColor;
            if (hoverSound != null) _audioSource.PlayOneShot(hoverSound, 0.5f);
        }

        private void OnExit()
        {
            iconImage.color = iconNormalColor;
        }

        private void OnDown()
        {
            iconImage.color = iconClickColor;
        }

        private void OnUp()
        {
            iconImage.color = iconHoverColor;
        }
        // ----------
        
        // POINTER EVENTS ----------
        
        private void RegisterPointerEvents()
        {
            RegisterOnEnter(OnEnter);
            RegisterOnExit(OnExit);
            RegisterOnDown(OnDown);
            RegisterOnUp(OnUp);
            RegisterOnOver(HoverAnimation);
            RegisterOnOff(ExitAnimation);
        }
        
        // ----------
        
        // REGISTER BUTTON CLICK EVENTS ----------

        /// <summary>
        /// Register a function to be called when this button is clicked.
        /// </summary>
        /// <param name="action">The function to be called.</param>
        public void RegisterOnClick(UnityAction action)
        {
            button.onClick.AddListener(action);
        }
        
        /// <summary>
        /// Clear all listeners from the button.
        /// </summary>
        public void ClearListeners()
        {
            button.onClick.RemoveAllListeners();
            if (clickSoundEnabled) button.onClick.AddListener(() => _audioSource.PlayOneShot(clickSound));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!followsTheme) iconImage.color = iconNormalColor;
            if (iconSprite != null) iconImage.sprite = iconSprite;
        }
#endif
    }
}

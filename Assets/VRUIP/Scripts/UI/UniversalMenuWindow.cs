using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VRUIP
{
    public class UniversalMenuWindow : A_ColorController
    {
        [Header("Window Properties")] 
        [SerializeField] private string title;
        
        [Header("Events")]
        public UnityEvent onClose;

        [Header("Components")]
         public Image background;
        public Image headerBackground;
        public IconController closeButton;
        public TextController titleText;
        
        private CanvasGroup _canvasGroup;
        private bool _isTransitioning;
        
        public string Title
        {
            get => titleText.Text;
            set => titleText.Text = value;
        }

        public bool IsOpen => gameObject.activeInHierarchy;

        public void Awake()
        {
            closeButton.RegisterOnClick(() => SetOpen(false));
            titleText.Text = title;
            SetupCanvasGroup();
        }

        // Open or close this window.
        public void SetOpen(bool open, Action callback = null)
        {
            if (!open && gameObject.activeInHierarchy == false) return;
            if (open)
            {
                //gameObject.SetActive(true);
                FadeInWindow(callback);
            }
            else
            {
                //Close();
                FadeOutWindow(callback);
                onClose.Invoke();
            }
        }

        protected override void SetColors(ColorTheme theme)
        {
            background.color = theme.primaryColor;
            headerBackground.color = theme.thirdColor;
        }
        
        /// <summary>
        /// Fade out this canvas and do something when fading finishes.
        /// </summary>
        private void FadeOutWindow(Action callback = null)
        {
            if (_isTransitioning) return;
            _isTransitioning = true;
            StartCoroutine(_canvasGroup.TransitionAlpha(0, 6, () =>
            {
                callback?.Invoke();
                _isTransitioning = false;
                gameObject.SetActive(false);
            }));
        }

        /// <summary>
        /// Fade in this canvas and do something when fading finishes.
        /// </summary>
        private void FadeInWindow(Action callback = null)
        {
            if (_isTransitioning) return;
            _isTransitioning = true;
            
            // Make sure the window is active.
            gameObject.SetActive(true);
            
            // Make sure the canvas group's starting alpha is zero for fade in.
            if (_canvasGroup.alpha != 0) _canvasGroup.alpha = 0;
            
            // Fade in the canvas group.
            StartCoroutine(_canvasGroup.TransitionAlpha(1, 6, () =>
            {
                callback?.Invoke();
                _isTransitioning = false;
            }));
        }
        
        /// <summary>
        /// Setup the canvas group for this window.
        /// </summary>
        private void SetupCanvasGroup()
        {
            if (_canvasGroup != null) return;
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            titleText.Text = title;
        }
        #endif
    }
}


using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace VRUIP
{
    public class ButtonController : A_UIIntercations
    {
        // NORMAL COLORS
        [SerializeField] private Color buttonNormalColor = Color.gray;
        [SerializeField] private Color borderNormalColor = Color.gray;
        [SerializeField] private Color textNormalColor = Color.black;

        // HOVER COLORS
        [SerializeField] private Color buttonHoverColor = Color.gray;
        [SerializeField] private Color borderHoverColor = Color.gray;
        [SerializeField] private Color textHoverColor = Color.white;
    
        // CLICK COLORS
        [SerializeField] private Color buttonClickColor = Color.gray;
        [SerializeField] private Color borderClickColor = Color.gray;
        [SerializeField] private Color textClickColor = Color.black;

        // PROPERTIES
        [SerializeField] private string text = "Button";
        [SerializeField] private int borderWidth = 2;
        [Range(0.01f,2f)]
        [SerializeField] private float roundness = 0.01f;
        [SerializeField] private bool hasBorder = true;
        [SerializeField] private bool clickSoundEnabled = true;
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip hoverSound;

        // ANIMATIONS
        [SerializeField] private ButtonAnimationType animationType;
        
        // COMPONENTS
        [SerializeField] private Button button;
        [SerializeField] private Image buttonImage;
        [SerializeField] private Image buttonBorder;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Image expandBorder;
        [SerializeField] private Image buttonShine;
        [SerializeField] private Image buttonSlide;

        public Color ButtonColor
        {
            get => buttonImage.color;
            set => buttonImage.color = value;
        }

        public Color BorderColor
        {
            get => buttonBorder.color;
            set => buttonBorder.color = value;
        }

        private float Roundness
        {
            get => 1 / buttonImage.pixelsPerUnitMultiplier;
            set
            {
                buttonImage.pixelsPerUnitMultiplier = 1 / value;
                buttonBorder.pixelsPerUnitMultiplier = 1 / value;
            }
        }
        
        // PRIVATE VARIABLES
        private Vector3 _adjustedExpandBorderScale;
        private Vector3 _shineStartPosition;
        private Vector3 _shineEndPosition;
        private UnityEvent _animationHoverEvent = new();
        private UnityEvent _animationExitEvent = new();
        private AudioSource _audioSource;

        private void Awake()
        {
            SetupButton();
        }

        /// <summary>
        /// Setup the button including all properties and functionalities from this script.
        /// </summary>
        [ContextMenu("Setup Button (VRUIP)")]
        private void SetupButton()
        {
            SetupVariables();
            SetupButtonColors();
            SetupButtonProperties();
            RegisterPointerEvents();
            SetupAnimations();
        }

        /// <summary>
        /// Set up the button colors based on the colors assigned to the properties of this script.
        /// </summary>
        private void SetupButtonColors()
        {
            button.transition = Selectable.Transition.None;
            buttonImage.color = buttonNormalColor;
            buttonBorder.color = borderNormalColor;
            buttonText.color = textNormalColor;
            expandBorder.color = buttonHoverColor;
            var shineColor = textHoverColor;
            shineColor.a = 60 / 255f;
            buttonShine.color = shineColor;
        }

        protected override void SetColors(ColorTheme theme)
        {
            buttonNormalColor = theme.thirdColor;
            buttonHoverColor = theme.secondaryColor;
            buttonClickColor = theme.fourthColor;
            borderNormalColor = theme.thirdColor;
            borderHoverColor = theme.secondaryColor;
            borderClickColor = theme.fourthColor;
            textNormalColor = theme.secondaryColor;
            textHoverColor = theme.primaryColor;
            textClickColor = theme.secondaryColor;
            buttonSlide.color = theme.secondaryColor;
            if (animationType == ButtonAnimationType.Slide)
            {
                buttonHoverColor = buttonNormalColor;
                borderHoverColor = borderNormalColor;
            }
            SetupButtonColors();
        }

        /// <summary>
        /// Setup the button properties based on the values of the properties in this script.
        /// </summary>
        private void SetupButtonProperties()
        {
            // Set border width
            buttonImage.GetComponent<RectTransform>().SetAllOffsets(borderWidth);
            // Set border roundness
            Roundness = roundness;
            // Set border activated
            buttonBorder.enabled = hasBorder;
            // Set click sound
            _audioSource = VRUIPManager.instance.AudioSource;
            if (clickSoundEnabled) button.onClick.AddListener(() => _audioSource.PlayOneShot(clickSound));
        }

        private void SetupVariables()
        {
            // Setup scale for expand border animation.
            var additionalScale = 0.1f;
            var buttonSize = buttonImage.rectTransform.rect;
            var x = 1 + additionalScale;
            var y = 1 + (additionalScale * (buttonSize.width / buttonSize.height));
            _adjustedExpandBorderScale = new Vector3(x, y, 1);
            
            // Setup shine animation positions.
            var shineWidth = buttonShine.rectTransform.rect.width;
            _shineStartPosition = new Vector3(buttonImage.rectTransform.rect.xMin - shineWidth, 0, 0);
            _shineEndPosition = new Vector3(buttonImage.rectTransform.rect.xMax + shineWidth, 0, 0);
        }

        private void SetupAnimations()
        {
            // Set animations
            switch (animationType)
            {
                case ButtonAnimationType.Shine:
                    buttonShine.transform.localPosition = _shineStartPosition;
                    buttonShine.gameObject.SetActive(true);
                    _animationHoverEvent.AddListener(ShineHover);
                    _animationExitEvent.AddListener(ShineExit);
                    break;
                case ButtonAnimationType.BorderExpand:
                    expandBorder.pixelsPerUnitMultiplier = 1 / roundness;
                    _animationHoverEvent.AddListener(ExpandBorderHover);
                    _animationExitEvent.AddListener(ExpandBorderExit);
                    break;
                case ButtonAnimationType.Slide:
                    buttonSlide.gameObject.SetActive(true);
                    buttonSlide.rectTransform.SetWidth(0);
                    buttonHoverColor = buttonNormalColor;
                    borderHoverColor = borderNormalColor;
                    hasBorder = false;
                    buttonBorder.enabled = false;
                    SetupButtonColors();
                    _animationHoverEvent.AddListener(SlideHover);
                    _animationExitEvent.AddListener(SlideExit);
                    break;
            }
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

        private void ExpandBorderHover()
        {
            if (!expandBorder.gameObject.activeInHierarchy) expandBorder.gameObject.SetActive(true);
            expandBorder.transform.SmoothScale(_adjustedExpandBorderScale);
            expandBorder.SmoothAlpha(0);
        }

        private void ExpandBorderExit()
        {
            if (expandBorder.transform.localScale == Vector3.one)
            {
                if (expandBorder.gameObject.activeInHierarchy)
                {
                    expandBorder.gameObject.SetActive(false);
                    expandBorder.SetAlpha(1);
                }
                return;
            }

            expandBorder.transform.SmoothScale(Vector3.one);
            expandBorder.SmoothAlpha(0.5f, 4);
        }

        private void ShineHover()
        {
            buttonShine.transform.SmoothMovement(_shineEndPosition);
        }

        private void ShineExit()
        {
            buttonShine.transform.SmoothMovement(_shineStartPosition);
        }

        private void SlideHover()
        {
            buttonSlide.rectTransform.SmoothWidth(buttonImage.rectTransform.rect.width);
        }

        private void SlideExit()
        {
            buttonSlide.rectTransform.SmoothWidth(0);
        }
        
        // ----------

        // CUSTOM EVENTS ----------
        private void OnEnter()
        {
            buttonImage.color = buttonHoverColor;
            buttonBorder.color = borderHoverColor;
            buttonText.color = textHoverColor;
            if (hoverSound != null) _audioSource.PlayOneShot(hoverSound, 0.5f);
        }

        private void OnExit()
        {
            buttonImage.color = buttonNormalColor;
            buttonBorder.color = borderNormalColor;
            buttonText.color = textNormalColor;
        }

        private void OnDown()
        {
            buttonImage.color = buttonClickColor;
            buttonBorder.color = borderClickColor;
            buttonText.color = textClickColor;
            buttonSlide.color = buttonClickColor;
        }

        private void OnUp()
        {
            buttonImage.color = buttonHoverColor;
            buttonBorder.color = borderHoverColor;
            buttonText.color = textHoverColor;
            buttonSlide.color = textNormalColor;
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
        
        // ----------
        
        private enum ButtonAnimationType
        {
            None,
            Shine,
            BorderExpand,
            Slide
        } 
        
#if UNITY_EDITOR
        // ON VALIDATE ----------
        private void OnValidate()
        {
            if (!followsTheme)
            {
                // Update button visuals
                buttonImage.color = buttonNormalColor;
                buttonBorder.color = borderNormalColor;
                buttonText.color = textNormalColor;
            }
            else
            {
                if (VRUIPManager.instance != null)
                {
                    SetupElement(VRUIPManager.instance.CurrentColorTheme);
                }
            }
            
            // Set text
            buttonText.text = text;
            
            // Border
            Roundness = roundness;
            
            buttonImage.GetComponent<RectTransform>().SetAllOffsets(borderWidth);
        }

        // ----------
#endif
    }
}

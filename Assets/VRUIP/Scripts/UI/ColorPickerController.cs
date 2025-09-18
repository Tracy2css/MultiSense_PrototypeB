using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

namespace VRUIP
{
    public class ColorPickerController : A_Canvas, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        [Header("Events")]
        public UnityEvent<Color> onColorChanged;
        private Color _currentColor; // Current color of the picker
        
        [Header("Components")]
        [SerializeField] private Image background;
        [SerializeField] private Image gradientImage;
        [SerializeField] private Image currentColorImage;
        [SerializeField] private Image currentColorOutline;
        [SerializeField] private Slider hueSlider;
        [SerializeField] private Image handleImage;
        [SerializeField] private Image handleOutline;
        [SerializeField] private Image sliderBackground;
        [SerializeField] private Image colorPickerCircle;
        [SerializeField] private TMP_InputField currentColorText;

        private readonly int _width = 100; // Width of the texture
        private readonly int _height = 100; // Height of the texture
        // private Color _currentColor; // Current color of the gradient
        private int _currentHue = 0; // Current hue of the gradient
        private int _currentSaturation; // Current saturation of the gradient
        private int _currentValue; // Current value of the gradient
        private bool _isDragging;
        private float _gradientScreenWidth;
        private float _gradientScreenHeight;

        public Color CurrentColor => _currentColor;

        protected override void Start()
        {
            base.Start();
            SetupColorPicker();
            SetInitialColor(Color.white); // Set default color to white
        }

        protected override void SetColors(ColorTheme theme)
        {
            background.color = theme.primaryColor;
            currentColorOutline.color = theme.secondaryColor;
            currentColorText.textComponent.color = theme.secondaryColor;
        }

        public void SetInitialColor(Color color)
        {
            _currentColor = color;

            // Update gradient, hue slider, and other components to match the initial color
            Color.RGBToHSV(_currentColor, out float h, out float s, out float v);
            _currentHue = Mathf.RoundToInt(h * 360);
            _currentSaturation = Mathf.RoundToInt(s * 100);
            _currentValue = Mathf.RoundToInt(v * 100);

            // Update UI components
            SetGradient();
            hueSlider.SetValueWithoutNotify(_currentHue);
            currentColorImage.color = _currentColor;
            currentColorText.SetTextWithoutNotify(ColorUtility.ToHtmlStringRGB(_currentColor));
            onColorChanged.Invoke(_currentColor);
         }
        
        private void SetupColorPicker()
        {
            // Set the initial hue of the gradient
            SetGradient();
            SetSliderBackground();
            hueSlider.value = _currentHue;
            handleImage.color = Color.HSVToRGB(_currentHue / 360f, 1, 1);
            hueSlider.onValueChanged.AddListener(OnSliderValueChanged);
            currentColorText.onValueChanged.AddListener(OnColorInputTextChanged);
            var gradientRect = gradientImage.rectTransform.rect;
            _gradientScreenHeight = gradientRect.height;
            _gradientScreenWidth = gradientRect.width;
            GetCurrentColor();
        }
        
        private void OnSliderValueChanged(float value)
        {
            // Update the current hue
            _currentHue = (int) value;

            // Update the gradient
            SetGradient();
            
            // Set slider handle color
            handleImage.color = Color.HSVToRGB(_currentHue / 360f, 1, 1);
            
            // Update the current color
            GetCurrentColor();
        }

        private void OnColorInputTextChanged(string value)
        {
            var hex = value;
            // Check if the input is a valid hex color
            if (!hex.StartsWith("#")) return;
            if (hex.Length != 7) return;
            // Set the current color
            var color = ColorUtility.TryParseHtmlString(hex, out var parsedColor) ? parsedColor : Color.white;
            _currentColor = color;
            currentColorImage.color = color;
            Color.RGBToHSV(color, out var h, out var s, out var v);
            _currentHue = (int) Mathf.Round(h * 360);
            _currentSaturation = (int) Mathf.Round(s * 100);
            _currentValue = (int) Mathf.Round(v * 100);
            
            // Update the slider
            hueSlider.SetValueWithoutNotify(_currentHue);
            // Update the gradient
            SetGradient();
            // Set slider handle color
            handleImage.color = Color.HSVToRGB(_currentHue / 360f, 1, 1);
            // Update the picker position
            var position = new Vector2(_currentSaturation / 100f * _gradientScreenWidth, _currentValue / 100f * _gradientScreenHeight);
            colorPickerCircle.transform.localPosition = position;
            onColorChanged.Invoke(_currentColor);
        }

        /// <summary>
        /// Set the gradient background based on the current hue.
        /// </summary>
        private void SetGradient()
        {
            // Create a new texture for the gradient
            var gradientTexture = new Texture2D(_width, _height)
            {
                filterMode = FilterMode.Point
            };

            // Loop through each pixel in the texture and set its color based on the gradient
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    // Get color based on the current hue and the current pixel's position in the texture
                    var pixelColor = Color.HSVToRGB(_currentHue / 360f, x / 100f, y / 100f);

                    // Set the color of the current pixel in the texture
                    gradientTexture.SetPixel(x, y, pixelColor);
                }
            }

            // Apply the changes to the texture
            gradientTexture.Apply();

            // Create a new sprite from the texture
            var gradientSprite = Sprite.Create(gradientTexture, new Rect(0f, 0f, _width, _height), new Vector2(0.5f, 0.5f));

            // Assign the sprite to a SpriteRenderer component to display it
            gradientImage.sprite = gradientSprite;
        }

        private void SetSliderBackground()
        {
            var gradientTexture = new Texture2D(360, 1)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int x = 0; x < 360; x++)
            {
                var pixelColor = Color.HSVToRGB(x / 360f, 1, 1);
                gradientTexture.SetPixel(x, 0, pixelColor);
            }
            
            gradientTexture.Apply();
            var gradientSprite = Sprite.Create(gradientTexture, new Rect(0f, 0f, 360, 1), new Vector2(0.5f, 0.5f));
            sliderBackground.sprite = gradientSprite;
        }
        
        /// <summary>
        /// Get the current color based on the picker position.
        /// </summary>
        private void GetCurrentColor()
        {
            var position = colorPickerCircle.transform.localPosition;
            var saturation = position.x / _gradientScreenWidth;
            var value = Mathf.Abs(position.y / _gradientScreenHeight);
            _currentSaturation = (int) Mathf.Round(saturation * 100);
            _currentValue = (int) Mathf.Round(value * 100);
            var color = Color.HSVToRGB(_currentHue / 360f, _currentSaturation / 100f, _currentValue / 100f);
            currentColorImage.color = color;
            _currentColor = color;
            var unformattedHex = color.ToHexString();
            currentColorText.SetTextWithoutNotify("#" + unformattedHex[..6]);
            onColorChanged.Invoke(_currentColor);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == gradientImage.gameObject)
            {
#if META_SDK
                VRUIPManager.instance.GetRayInteractorPosition(out var worldPosition);
                var screenPoint = Camera.WorldToScreenPoint(worldPosition);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(gradientImage.rectTransform, screenPoint, Camera, out var localPoint);
#else
                RectTransformUtility.ScreenPointToLocalPointInRectangle(gradientImage.rectTransform, eventData.position, Camera, out var localPoint);
#endif
                colorPickerCircle.transform.localPosition = localPoint;
                GetCurrentColor();
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == gradientImage.gameObject)
            {
#if META_SDK
                VRUIPManager.instance.GetRayInteractorPosition(out var worldPosition);
                var screenPoint = Camera.WorldToScreenPoint(worldPosition);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(gradientImage.rectTransform, screenPoint, Camera, out var localPoint);
#else
                // If user is dragging inside of gradient
                if (!_isDragging) _isDragging = true;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(gradientImage.rectTransform, eventData.position, Camera, out var localPoint);
#endif
                colorPickerCircle.transform.localPosition = localPoint;
                GetCurrentColor();
            }
            else if (_isDragging)
            {
#if META_SDK
                VRUIPManager.instance.GetRayInteractorPosition(out var worldPosition);
                var screenPoint = Camera.WorldToScreenPoint(worldPosition);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(gradientImage.rectTransform, screenPoint, Camera, out var localPoint);
#else
                // If user dragged outside of gradient but is still holding
                RectTransformUtility.ScreenPointToLocalPointInRectangle(gradientImage.rectTransform, eventData.position, Camera, out var localPoint);
#endif
                var adjustedPoint = new Vector2(Mathf.Clamp(localPoint.x, 0, _gradientScreenWidth), Mathf.Clamp(localPoint.y, 0, _gradientScreenHeight));
                colorPickerCircle.transform.localPosition = adjustedPoint;
                GetCurrentColor();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
        }
    }
}

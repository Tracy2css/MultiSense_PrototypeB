using System;
using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class VRCanvas : A_Canvas
    {
        [Header("Colors")]
        [SerializeField] private Color backgroundColor;
        
        [Header("Properties")]
        [SerializeField] [Tooltip("Allow resizing/scaling canvas by dragging its corners.")] private bool isScalable = false;
        [SerializeField] private Vector2 minSize = new Vector2(100, 100);
        [SerializeField] private Vector2 maxSize = new Vector2(1000, 1000);
        
        [Header("Components")]
        [SerializeField] private Image background;
        
        private ScaleUIButton _scaleButtonPrefab;

        private void Awake()
        {
            SetupCanvas();
        }

        protected override void Start()
        {
            base.Start();
            if (isScalable)
            {
                InitializeScaling();
            }
        }

        [ContextMenu("Setup Canvas (VRUIP)")]
        private void SetupCanvas()
        {
            if (background != null) background.color = backgroundColor;
        }

        protected override void SetColors(ColorTheme theme)
        {
            // Background is primary
            if (background != null) background.color = theme.primaryColor;
        }
        
        // SCALING FUNCTIONS ----------
        /// <summary>
        /// Initialize scaling by creating scale buttons.
        /// </summary>
        private void InitializeScaling()
        {
            _scaleButtonPrefab = VRUIPManager.instance.scaleButton;
            for (var i = 0; i < 4; i++)
            {
                var button = Instantiate(_scaleButtonPrefab, transform);
                button.name = "ScaleButton-" + Enum.GetName(typeof(ScaleUIButton.CanvasCorner), (ScaleUIButton.CanvasCorner)i);
                button.Initialize(RectTransform, (ScaleUIButton.CanvasCorner)i, i, minSize, maxSize);
            }
        }
    }
}

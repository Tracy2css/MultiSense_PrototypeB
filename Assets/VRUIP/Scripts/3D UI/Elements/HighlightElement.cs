using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VRUIP
{
    public class HighlightElement : Element
    {
        [SerializeField] private Image staticBorder;
        [SerializeField] private Transform borderPixel;
        [SerializeField] private HighlightType highlightType;
        
        [Header("Rotating Highlight Properties")]
        [SerializeField] [Range(1, 3)] private int rotationSpeed = 1;

        private readonly Dictionary<HighlightType, UnityAction[]> _highlightFunctions = new();
        private Vector2 _elementSize;
        private Vector2 _corners;
        private Vector2 _borderPosition;
        private int _rotationSpeedAdjusted;
        private TrailRenderer _trailRenderer;

        private const float TOLERANCE = 0.1f;
        private const float HIGHLIGHTSPEED = 2f;

        private void Awake()
        {
            InitializeFunctions();
        }

        private void InitializeFunctions()
        {
            if (_highlightFunctions.Count > 0) return;
            _highlightFunctions.Add(HighlightType.Rotating, new UnityAction[] { RotatingHighlight });
            _highlightFunctions.Add(HighlightType.Static, new UnityAction[] { StaticHighlight, ResetStaticHighlight });
        }

        protected override void Initialize()
        {
            // if already initialized, return
            if (initialized) return;
            
            borderPixel.gameObject.SetActive(highlightType == HighlightType.Rotating);
            staticBorder.gameObject.SetActive(highlightType == HighlightType.Static);
            var functionArray = _highlightFunctions[highlightType];
            
            // Register functions
            if (highlightType == HighlightType.Rotating)
            {
                RegisterOnEnter(() => borderPixel.gameObject.SetActive(true));
                RegisterOnExit(() =>
                {
                    borderPixel.gameObject.SetActive(false);
                    ResetRotatingHighlight();
                });
                RegisterOnOver(functionArray[0]);
                _trailRenderer = borderPixel.GetComponent<TrailRenderer>();
            }
            else if (highlightType == HighlightType.Static)
            {
                RegisterOnOver(functionArray[0]);
                RegisterOnOff(functionArray[1]);
            }

            _elementSize = GetComponent<RectTransform>().rect.size;
            _corners = new Vector2(_elementSize.x / 2, _elementSize.y / 2);
            _borderPosition = new Vector2(-_corners.x, _corners.y);
            _rotationSpeedAdjusted = rotationSpeed * 10;
            
            // Set initialized to true
            base.Initialize();
        }

        private void StaticHighlight()
        {
            var currentColor = staticBorder.color;
            if (currentColor.a < 1)
            {
                var newAlpha = Mathf.MoveTowards(currentColor.a, 1, Time.deltaTime * HIGHLIGHTSPEED);
                var newColor = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
                staticBorder.color = newColor;
            }
        }

        private void ResetStaticHighlight()
        {
            var currentColor = staticBorder.color;
            if (currentColor.a >= 0)
            {
                var newAlpha = Mathf.MoveTowards(currentColor.a, 0, Time.deltaTime * HIGHLIGHTSPEED);
                var newColor = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
                staticBorder.color = newColor;
            }
        }
        
        private void RotatingHighlight()
        {
            if (Math.Abs(_borderPosition.y - _corners.y) < TOLERANCE && _borderPosition.x < _corners.x)
            {
                _borderPosition = new Vector2(_borderPosition.x + _rotationSpeedAdjusted, _corners.y);
                borderPixel.localPosition = _borderPosition;
            }
            else if (Math.Abs(_borderPosition.y - _corners.y) < TOLERANCE && _borderPosition.x >= _corners.x)
            {
                _borderPosition = new Vector2(_corners.x, _borderPosition.y - _rotationSpeedAdjusted);
                borderPixel.localPosition = _borderPosition;
            }
            else if (_borderPosition.y > -_corners.y && Math.Abs(_borderPosition.x - _corners.x) < TOLERANCE)
            {
                _borderPosition = new Vector2(_corners.x, _borderPosition.y - _rotationSpeedAdjusted);
                borderPixel.localPosition = _borderPosition;
            }
            else if (Math.Abs(_borderPosition.y + _corners.y) < TOLERANCE && _borderPosition.x >= _corners.x)
            {
                _borderPosition = new Vector2(_borderPosition.x - _rotationSpeedAdjusted, -_corners.y);
                borderPixel.localPosition = _borderPosition;
            }
            else if (Math.Abs(_borderPosition.y + _corners.y) < TOLERANCE && _borderPosition.x > -_corners.x)
            {
                _borderPosition = new Vector2(_borderPosition.x - _rotationSpeedAdjusted, -_corners.y);
                borderPixel.localPosition = _borderPosition;
            }
            else if (_borderPosition.y < _corners.y && Math.Abs(_borderPosition.x + _corners.x) < TOLERANCE)
            {
                _borderPosition = new Vector2(-_corners.x, _borderPosition.y + _rotationSpeedAdjusted);
                borderPixel.localPosition = _borderPosition;
            }
        }

        private void ResetRotatingHighlight()
        {
            // Reset to the top left corner.
            var resetPosition = new Vector2(-_corners.x, _corners.y);
            borderPixel.localPosition = resetPosition;
            _borderPosition = resetPosition;
            _trailRenderer.Clear();
        }

        private enum HighlightType
        {
            Static,
            Rotating,
        }
    }
}
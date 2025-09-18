using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if META_SDK
using Oculus.Interaction;
#endif

namespace VRUIP
{
    /// <summary>
    /// This is a script that is attached to the buttons that are used to scale a canvas.
    /// </summary>
    public class ScaleUIButton : A_ColorController, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Components")]
        [SerializeField] private Image icon;
        [SerializeField] private RectTransform expander;
        
        private RectTransform _buttonTransform;
        private RectTransform _scalableCanvas;
        private CanvasCorner _corner;
        private Vector3 _originalPosition;
        private Vector2 _originalSize;
        private LineRenderer _lineRenderer;
        private bool _isVR;
        private Vector2 _minSize;
        private Vector2 _maxSize;
        private Vector2 _expanderSize = new(1000, 1000);
        private BoxCollider _canvasBoxCollider;

        private readonly Dictionary<CanvasCorner, Vector2[]> anchors = new()
        {
            { CanvasCorner.BottomLeft, new[] { Vector2.zero, Vector2.zero } },
            { CanvasCorner.TopLeft, new[] { new Vector2(0,1), new Vector2(0,1) } },
            { CanvasCorner.TopRight, new[] { new Vector2(1,1), new Vector2(1,1) } },
            { CanvasCorner.BottomRight, new[] { new Vector2(1,0), new Vector2(1,0) } }
        };
        
        private readonly Dictionary<CanvasCorner, Vector2> pivots = new()
        {
            { CanvasCorner.BottomLeft, new Vector2(1, 1) },
            { CanvasCorner.TopLeft, new Vector2(1, 0) },
            { CanvasCorner.TopRight, new Vector2(0, 0) },
            { CanvasCorner.BottomRight, new Vector2(0, 1) }
        };

        public void Initialize(RectTransform scalableCanvas, CanvasCorner corner, int cornerIndex, Vector2 minSize, Vector2 maxSize)
        {
            _isVR = VRUIPManager.instance.IsVR;
            _scalableCanvas = scalableCanvas;
            _corner = corner;
            _buttonTransform = GetComponent<RectTransform>();
            var rotate = _corner == CanvasCorner.TopLeft || _corner == CanvasCorner.BottomRight;
            transform.localEulerAngles = rotate ? new Vector3(0, 0, 90) : Vector3.zero;
#if !META_SDK
            _lineRenderer = VRUIPManager.instance.lineRenderer;
#else
            _canvasBoxCollider = _scalableCanvas.GetComponent<BoxCollider>();
#endif
            
            // Set min and max size.
            _minSize = minSize;
            _maxSize = maxSize;

            // Set anchors
            _buttonTransform.anchorMin = anchors[_corner][0];
            _buttonTransform.anchorMax = anchors[_corner][1];
            
            // Set positions to corners
            var corners = new Vector3[4];
            scalableCanvas.GetLocalCorners(corners);
            transform.localPosition = corners[cornerIndex];
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _originalPosition = transform.position;
            _originalSize = _scalableCanvas.sizeDelta;
            // Set pivots
            Util.SetPivot(_scalableCanvas, pivots[_corner]);
#if META_SDK
            var canvasSize = _scalableCanvas.sizeDelta;
            if (_canvasBoxCollider != null)
            {
                var x = _corner == CanvasCorner.BottomRight || _corner == CanvasCorner.TopRight ? canvasSize.x / 2f : -canvasSize.x / 2f;
                var y = _corner == CanvasCorner.TopLeft || _corner == CanvasCorner.TopRight ? canvasSize.y / 2f : -canvasSize.y / 2f;
                _canvasBoxCollider.center = new Vector3(x , y, _canvasBoxCollider.center.z);
            }
#endif
            expander.sizeDelta = _expanderSize;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            var size = GetComponent<RectTransform>().sizeDelta;
            expander.sizeDelta = size;
#if META_SDK
            var canvasSize = _scalableCanvas.sizeDelta;
            _canvasBoxCollider.size = canvasSize;
            var x = _corner == CanvasCorner.BottomRight || _corner == CanvasCorner.TopRight ? canvasSize.x / 2f : -canvasSize.x / 2f;
            var y = _corner == CanvasCorner.TopLeft || _corner == CanvasCorner.TopRight ? canvasSize.y / 2f : -canvasSize.y / 2f;
            _canvasBoxCollider.center = new Vector3(x , y, _canvasBoxCollider.center.z);
#endif
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 pointerEndPosition;
            if (_isVR)
            {
#if META_SDK
                if (!VRUIPManager.instance.GetRayInteractorPosition(out pointerEndPosition)) return;
#else
                pointerEndPosition = _lineRenderer.GetPosition(_lineRenderer.positionCount - 1);
#endif
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_scalableCanvas, eventData.position, VRUIPManager.instance.mainCamera, out var mousePos);
                pointerEndPosition = _scalableCanvas.TransformPoint(mousePos);
            }
            
            float differenceX = 0;
            float differenceY = 0;
            if (_corner == CanvasCorner.TopLeft)
            {
                differenceX = _originalPosition.x - pointerEndPosition.x;
                differenceY = pointerEndPosition.y - _originalPosition.y;
            }
            else if (_corner == CanvasCorner.BottomRight)
            {
                differenceX = pointerEndPosition.x - _originalPosition.x;
                differenceY = _originalPosition.y - pointerEndPosition.y;
            }
            else if (_corner == CanvasCorner.BottomLeft)
            {
                differenceX = _originalPosition.x - pointerEndPosition.x;
                differenceY = _originalPosition.y - pointerEndPosition.y;
            }
            else if (_corner == CanvasCorner.TopRight)
            {
                differenceX = pointerEndPosition.x - _originalPosition.x;
                differenceY = pointerEndPosition.y - _originalPosition.y;
            }
            
            var difference = new Vector2(differenceX, differenceY);
            var lossyScale = _scalableCanvas.lossyScale;
            var pixelDifference = new Vector3(difference.x / lossyScale.x, difference.y / lossyScale.y);

            var newSize = _originalSize + (Vector2)pixelDifference;
            var adjustedSize = new Vector2(Mathf.Clamp(newSize.x, _minSize.x, _maxSize.x), Mathf.Clamp(newSize.y, _minSize.y, _maxSize.y));
#if META_SDK
            var expandedColliderSize = adjustedSize + new Vector2(200, 200);
            _canvasBoxCollider.size = expandedColliderSize;
            var x = _corner == CanvasCorner.BottomRight || _corner == CanvasCorner.TopRight ? expandedColliderSize.x / 2f : -expandedColliderSize.x / 2f;
            var y = _corner == CanvasCorner.TopLeft || _corner == CanvasCorner.TopRight ? expandedColliderSize.y / 2f : -expandedColliderSize.y / 2f;
            _canvasBoxCollider.center = new Vector3(x , y, _canvasBoxCollider.center.z);
#endif
            _scalableCanvas.sizeDelta = adjustedSize;
        }

        public enum CanvasCorner
        {
            BottomLeft,
            TopLeft,
            TopRight,
            BottomRight
        }

        protected override void SetColors(ColorTheme theme)
        {
            icon.color = theme.secondaryColor;
        }
    }
}

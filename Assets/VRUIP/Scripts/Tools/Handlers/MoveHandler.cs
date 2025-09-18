using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace VRUIP
{
    public class MoveHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Transform movableObject;
        [SerializeField] private Image handleImage;

        [Header("Axis")] [SerializeField] private bool x, y;

        private Camera _camera;
        private bool _isMoving;
        private Vector3 _handleObjectOffset;
        private float _cameraClipPlane;
        private bool _isVR;
        private LineRenderer _pointer;
        private RectTransform _handleRectTransform;
        private Vector2 _originalSize;
        private Vector2 _movingSize = new(2000, 2000);
        private float _movingConstZ;
        
        private const float SPEED = 10;

        private void Awake()
        {
            _handleObjectOffset = movableObject.position - transform.position;
        }

        private void Start()
        {
            #if (OCULUS_INTEGRATION || XR_ITK || META_SDK)
            _isVR = true;
            #else
            _isVR = false;
            #endif
            _camera = VRUIPManager.instance.mainCamera;
            #if !META_SDK
            _pointer = VRUIPManager.instance.lineRenderer;
            #endif
            if (_camera != null) _cameraClipPlane = transform.position.z - _camera.transform.position.z;
            _handleRectTransform = GetComponent<RectTransform>();
            _originalSize = _handleRectTransform.sizeDelta;
        }

        private void FixedUpdate()
        {
            // If isMoving is true, move the movableObject to the pointer position.
            if (_isMoving)
            {
                // If the target framework is not VR, use the mouse position to move the movableObject.
                if (!_isVR)
                {
                    var mousePosition = Mouse.current.position.ReadValue();
                    var mouseWorldPosition = _camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, _cameraClipPlane));
                    var newX = x ? mouseWorldPosition.x : movableObject.position.x - _handleObjectOffset.x;
                    var newY = y ? mouseWorldPosition.y : movableObject.position.y - _handleObjectOffset.y;
                    //var newZ = z ? mouseWorldPosition.z - _handleObjectOffset.z : movableObject.position.z; ;
                    var newPosition = new Vector3(newX, newY, _movingConstZ) + _handleObjectOffset;
                    movableObject.position = Vector3.Lerp(movableObject.position, newPosition, Time.deltaTime * SPEED);
                }
                // if it is VR use the pointer position to move the movableObject.
                else
                {
                    var pointerPosition = _pointer.GetPosition(_pointer.positionCount - 1);
                    var newX = x ? pointerPosition.x : movableObject.position.x - _handleObjectOffset.x;
                    var newY = y ? pointerPosition.y : movableObject.position.y - _handleObjectOffset.y;
                    //var newZ = z ? mouseWorldPosition.z - _handleObjectOffset.z : movableObject.position.z; ;
                    var newPosition = new Vector3(newX, newY, _movingConstZ) + _handleObjectOffset;
                    movableObject.position = newPosition;
                }
            }
        }

        // On pointer down, set handle image alpha to .5, set isMoving to true and set handle object offset.
        public void OnPointerDown(PointerEventData eventData)
        {
            _isMoving = true;
            _movingConstZ = movableObject.position.z;
            _handleObjectOffset = movableObject.position - eventData.pointerCurrentRaycast.worldPosition;
            handleImage.SetAlpha(.5f);
            _handleRectTransform.sizeDelta = _movingSize;
        }

        // On pointer up, set handle image alpha back to 1 and set isMoving to false.
        public void OnPointerUp(PointerEventData eventData)
        {
            _isMoving = false;
            handleImage.SetAlpha(1);
            _handleRectTransform.sizeDelta = _originalSize;
        }
    }
}

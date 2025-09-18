using UnityEngine;

namespace VRUIP
{
    public class GrabHandler : A_Grabbable
    {
        [SerializeField] private Transform movableObject;
        [SerializeField] private bool enableXZRotation;

        private bool _isMoving;
        private Vector3 _handleObjectOffset;
        private const int SPEED = 10;
        private Vector3 _position;
        private Vector3 _rotation;

        protected override void Start()
        {
            base.Start();
            var t = transform;
            _position = t.localPosition;
            _rotation = t.localEulerAngles;
            _handleObjectOffset = movableObject.position - t.position;
            RegisterOnGrab(() => _isMoving = true);
            RegisterOnRelease(() =>
            {
                _isMoving = false;
                t.localPosition = _position;
                t.localEulerAngles = _rotation;
            });
        }

        protected override void Update()
        {
            base.Update();
            if (_isMoving)
            {
                var t = transform;
            
                // Set position.
                var newPosition = t.position + _handleObjectOffset;
                movableObject.position = Vector3.Lerp(movableObject.position, newPosition, Time.deltaTime * SPEED);
            
                // Set rotation.
                var rotationY = Mathf.LerpAngle(movableObject.eulerAngles.y,t.eulerAngles.y, Time.deltaTime * SPEED);
                var rotationX = enableXZRotation ? Mathf.LerpAngle(movableObject.eulerAngles.x,t.eulerAngles.x, Time.deltaTime * SPEED) : 0;
                var rotationZ = enableXZRotation ? Mathf.LerpAngle(movableObject.eulerAngles.z,t.eulerAngles.z, Time.deltaTime * SPEED) : 0;
                movableObject.eulerAngles = new Vector3(rotationX, rotationY, rotationZ);
            }
        }
    }
}

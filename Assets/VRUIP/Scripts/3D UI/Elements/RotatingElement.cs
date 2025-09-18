#if META_SDK
using Oculus.Interaction;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace VRUIP
{
    public class RotatingElement : Element
    {
        [Header("Element Properties")]
        [SerializeField] private Transform objectToRotate;
        [SerializeField][Range(1,3)] private int reactionIntensity = 1;

        private LineRenderer _lineRenderer; // FOR VR INPUT
#if META_SDK
        private RayInteractor _handRayInteractor; // FOR META SDK INPUT
        private RayInteractor _controllerRayInteractor; // FOR META SDK INPUT
#endif
        
        // CONSTANTS
        private const float REACTION_MULTIPLIER = 30; // FOR VR INPUT
        private const float DAMPING_VALUE = 30; // FOR MOUSE INPUT
        private const float REACTION_SPEED = 200;

        protected override void Initialize()
        {
            base.Initialize();
            RegisterOnOver(RotateTowardsPointer);
            RegisterOnOff(ResetRotation);
            #if !META_SDK
            _lineRenderer = VRUIPManager.instance.lineRenderer;
            #else
            _handRayInteractor = VRUIPManager.instance.handRayInteractor;
            _controllerRayInteractor = VRUIPManager.instance.controllerRayInteractor;
            #endif
        }

        private void RotateTowardsPointer()
        {
            #if (OCULUS_INTEGRATION || XR_ITK)
            // FOR XR INPUT:
            var pointerEndPosition = transform.InverseTransformPoint(_lineRenderer.GetPosition(_lineRenderer.positionCount - 1));
            var localPosition = objectToRotate.localPosition;
            var offsetX = localPosition.x - pointerEndPosition.x;
            var offsetY = localPosition.y - pointerEndPosition.y;
            var xRot = -offsetY * REACTION_MULTIPLIER * reactionIntensity;
            var yRot = offsetX * REACTION_MULTIPLIER * reactionIntensity;
            #elif META_SDK
            // FOR META SDK INPUT:
            var activeRayInteractor = _handRayInteractor.isActiveAndEnabled ? _handRayInteractor : _controllerRayInteractor;
            if (!activeRayInteractor.isActiveAndEnabled || !activeRayInteractor.CollisionInfo.HasValue) return;
            var pointerEndPosition = transform.InverseTransformPoint(activeRayInteractor.CollisionInfo.Value.Point);
            var localPosition = objectToRotate.localPosition;
            var offsetX = localPosition.x - pointerEndPosition.x;
            var offsetY = localPosition.y - pointerEndPosition.y;
            var xRot = -offsetY * REACTION_MULTIPLIER * reactionIntensity;
            var yRot = offsetX * REACTION_MULTIPLIER * reactionIntensity;
            #else
            // FOR MOUSE INPUT ENABLE THIS AND DISABLE PREVIOUS:
            var screenPosition = mainCamera.WorldToScreenPoint(transform.position);
            var offsetX = screenPosition.x - Mouse.current.position.x.ReadValue();
            var offsetY = screenPosition.y - Mouse.current.position.y.ReadValue();
            var xRot = -offsetY / (DAMPING_VALUE / reactionIntensity);
            var yRot = offsetX / (DAMPING_VALUE / reactionIntensity);
            #endif

            var localEulerAngles = objectToRotate.localEulerAngles;
            var newX = Mathf.MoveTowardsAngle(localEulerAngles.x, xRot, Time.deltaTime * REACTION_SPEED);
            var newY = Mathf.MoveTowardsAngle(localEulerAngles.y, yRot, Time.deltaTime * REACTION_SPEED);
            localEulerAngles = new Vector3(newX, newY, 0);
            objectToRotate.localEulerAngles = localEulerAngles;
        }

        private void ResetRotation()
        {
            if (objectToRotate.localEulerAngles == Vector3.zero) return;
            var localEulerAngles = objectToRotate.localEulerAngles;
            var newX = Mathf.MoveTowardsAngle(localEulerAngles.x, 0, Time.deltaTime * REACTION_SPEED);
            var newY = Mathf.MoveTowardsAngle(localEulerAngles.y, 0, Time.deltaTime * REACTION_SPEED);
            localEulerAngles = new Vector3(newX, newY, 0);
            objectToRotate.localEulerAngles = localEulerAngles;
        }
    }
}

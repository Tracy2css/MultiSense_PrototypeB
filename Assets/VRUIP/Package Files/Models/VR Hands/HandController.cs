using UnityEngine;
using UnityEngine.InputSystem;

namespace VRUIP
{
    public class HandController : MonoBehaviour
    {
        public InputActionProperty pinchAction;
        public InputActionProperty gripAction;
        public Animator handAnimator;

        private void Update()
        {
            var triggerValue = pinchAction.action.ReadValue<float>();
            var gripValue = gripAction.action.ReadValue<float>();
            handAnimator.SetFloat("Trigger", triggerValue);
            handAnimator.SetFloat("Grip", gripValue);
        }
    }
}

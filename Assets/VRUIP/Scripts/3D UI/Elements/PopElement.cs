using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class PopElement : Element
    {
        [SerializeField][Range(1,3)] private int popAmount = 1;

        private Vector3 _originalPosition;
        private Vector3 _popPosition;

        private const int POP_SPEED = 1;
        private const float POP_DAMPNING = 0.1f;


        protected override void Initialize()
        {
            base.Initialize();
            _originalPosition = transform.localPosition;
            _popPosition = new Vector3(_originalPosition.x, _originalPosition.y, -popAmount * POP_DAMPNING);
            RegisterOnOver(Pop);
            RegisterOnOff(ResetPosition);
        }

        private void Pop()
        {
            if (transform.localPosition == _popPosition) return;
            var newZ = Mathf.MoveTowards(transform.localPosition.z, _popPosition.z, Time.deltaTime * POP_SPEED);
            transform.localPosition = new Vector3(_originalPosition.x, _originalPosition.y, newZ);
        }

        private void ResetPosition()
        {
            if (transform.localPosition == _originalPosition) return;
            var newZ = Mathf.MoveTowards(transform.localPosition.z, _originalPosition.z, Time.deltaTime * POP_SPEED);
            transform.localPosition = new Vector3(_originalPosition.x, _originalPosition.y, newZ);
        }
    }
}
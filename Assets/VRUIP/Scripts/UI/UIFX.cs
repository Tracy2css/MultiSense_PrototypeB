using System;
using UnityEngine;

namespace VRUIP
{
    public class UIFX : MonoBehaviour
    {
        public bool alwaysFacePlayer;

        private void Update()
        {
            if (alwaysFacePlayer)
            {
                LookAtPlayer();
            }
        }

        /// <summary>
        /// Rotate this canvas to be facing the player.
        /// </summary>
        private void LookAtPlayer()
        {
            var direction = transform.position - VRUIPManager.instance.mainCamera.transform.position;
            var lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation;
        }
    }
}

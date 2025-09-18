using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRUIP
{
    public class CameraVR : A_Canvas
    {
        [SerializeField] private Camera unityCamera;
        [SerializeField] private Picture picturePrefab;
        [SerializeField] private Transform imageSpawnLocation;

        private List<Sprite> _pictures;
        private int _depthIndex = 0;
        
        public bool IsSelfie => Math.Abs(unityCamera.transform.localEulerAngles.y - 180) < 0.1;

        /// <summary>
        /// Flip this camera to selfie or front.
        /// </summary>
        public void FlipCamera()
        {
            unityCamera.transform.localEulerAngles = IsSelfie ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
        }

        /// <summary>
        /// Snap a picture with this Camera.
        /// </summary>
        public void SnapPicture()
        {
            var cameraTexture = Util.ToTexture2D(unityCamera.targetTexture);
            var sprite = Util.ToSprite(cameraTexture);
            picturePrefab.Create(imageSpawnLocation, sprite);
        }

        /// <summary>
        /// Zoom In.
        /// </summary>
        public void ZoomIn()
        {
            if (_depthIndex < 2)
            {
                _depthIndex++;
                unityCamera.fieldOfView -= 10;
            }
        }

        /// <summary>
        /// Zoom out.
        /// </summary>
        public void ZoomOut()
        {
            if (_depthIndex > -2)
            {
                _depthIndex--;
                unityCamera.fieldOfView += 10;
            }
        }

        protected override void SetColors(ColorTheme theme)
        {
            //Nothing here for now.
        }
    }
}

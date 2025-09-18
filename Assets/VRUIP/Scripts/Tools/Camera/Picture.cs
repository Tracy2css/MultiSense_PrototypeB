using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class Picture : A_Canvas
    {
        [SerializeField] private Image image;
        [SerializeField] private Button deleteButton;
        
        private GameObject _imageContainer;

        /// <summary>
        /// Create a new picture.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="sprite"></param>
        public void Create(Transform position, Sprite sprite)
        {
            if (_imageContainer == null)
            {
                _imageContainer = new GameObject("ImageContainer");
                _imageContainer.transform.SetParent(null);
            }
            var clone = Instantiate(this, position.position, position.rotation);
            clone.transform.SetParent(_imageContainer.transform);
            clone.image.sprite = sprite;
            clone.deleteButton.onClick.AddListener(clone.Delete);
        }

        // Delete this picture.
        private void Delete()
        {
            Destroy(this.gameObject);
        }

        protected override void SetColors(ColorTheme theme)
        {
            //Nothing here for now.
        }
    }
}

using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class GalleryController : A_ColorController
    {
        [SerializeField] private GalleryItem[] images;

        [Header("Properties")]
        [SerializeField] private bool scrollAnimation;
        
        [Header("Colors")]
        [SerializeField] private Color buttonNormalColor;
        [SerializeField] private Color buttonHoverColor;
        [SerializeField] private Color buttonClickColor;
        [SerializeField] private Color titleTextColor;

        [Header("Components")]
        [SerializeField] private Image currentImage;
        [SerializeField] private Image nextImage;
        [SerializeField] private TextMeshProUGUI currentTitle;
        [SerializeField] private TextMeshProUGUI nextTitle;
        [SerializeField] private IconController leftButton;
        [SerializeField] private IconController rightButton;

        private int _index;
        private bool _isScrolling;
        private int scrollSpeed = 1;

        private void Awake()
        {
            SetupGallery();
        }

        protected override void Start()
        {
            base.Start();
            scrollSpeed = VRUIPManager.instance.IsVR ? 2 : 1;
        }

        [ContextMenu("Setup Gallery (VRUIP)")]
        private void SetupGallery()
        {
            // Check that gallery is not empty.
            if (images.Length < 1)
            {
                Debug.LogError("Please include at least 1 image in gallery.");
                return;
            }
            
            // Populate components.
            _index = 0;
            var firstItem = images[0];
            currentImage.sprite = firstItem.image;
            currentTitle.text = firstItem.title;
            
            // Add event listeners to buttons.
            leftButton.RegisterOnClick(NavigateLeft);
            rightButton.RegisterOnClick(NavigateRight);
            
            // Setup colors
            leftButton.iconNormalColor = buttonNormalColor;
            leftButton.iconHoverColor = buttonHoverColor;
            leftButton.iconClickColor = buttonClickColor;
            rightButton.iconNormalColor = buttonNormalColor;
            rightButton.iconHoverColor = buttonHoverColor;
            rightButton.iconClickColor = buttonClickColor;
            currentTitle.color = nextTitle.color = titleTextColor;
        }
        
        private void NavigateLeft()
        {
            if (_index == 0) _index = images.Length - 1;
            else _index--;
            var currentItem = images[_index];
            if (scrollAnimation)
            {
                if (!_isScrolling) StartCoroutine(ScrollLeft());
            }
            else
            {
                currentImage.sprite = currentItem.image;
                currentTitle.text = currentItem.title;
            }
        }

        private void NavigateRight()
        {
            if (_index == images.Length - 1) _index = 0;
            else _index++;
            var currentItem = images[_index];
            if (scrollAnimation)
            {
                if (!_isScrolling) StartCoroutine(ScrollRight());
            }
            else
            {
                currentImage.sprite = currentItem.image;
                currentTitle.text = currentItem.title;
            }
        }

        private IEnumerator ScrollLeft()
        {
            // Disable clicking while scrolling animation is happening.
            _isScrolling = true;
            leftButton.button.interactable = false;
            rightButton.button.interactable = false;
            
            // Set next image properties.
            nextImage.transform.SetLocalX(-400);
            nextImage.sprite = images[_index].image;
            nextTitle.transform.SetLocalX(-400);
            nextTitle.text = images[_index].title;
            
            // Loop until images are at correct position.
            while (currentImage.transform.localPosition.x < 400)
            {
                currentImage.transform.SetLocalX(5 * scrollSpeed, true);
                nextImage.transform.SetLocalX(5 * scrollSpeed, true);
                currentTitle.transform.SetLocalX(5 * scrollSpeed, true);
                nextTitle.transform.SetLocalX(5 * scrollSpeed, true);

                yield return null;
            }

            // Reset images and titles for next click.
            currentImage.sprite = nextImage.sprite;
            currentImage.transform.localPosition = nextImage.transform.localPosition;
            nextImage.transform.SetLocalX(-400);
            currentTitle.text = nextTitle.text;
            currentTitle.transform.localPosition = nextTitle.transform.localPosition;
            nextTitle.transform.SetLocalX(-400);
            
            // Restore clicking functionality.
            leftButton.button.interactable = true;
            rightButton.button.interactable = true;
            _isScrolling = false;
        }
        
        private IEnumerator ScrollRight()
        {
            // Disable clicking while scrolling animation is happening.
            _isScrolling = true;
            leftButton.button.interactable = false;
            rightButton.button.interactable = false;
            
            // Set next image properties.
            nextImage.transform.SetLocalX(400);
            nextImage.sprite = images[_index].image;
            nextTitle.transform.SetLocalX(400);
            nextTitle.text = images[_index].title;
            
            // Loop until images are at correct position.
            while (currentImage.transform.localPosition.x > -400)
            {
                currentImage.transform.SetLocalX(-5 * scrollSpeed, true);
                nextImage.transform.SetLocalX(-5 * scrollSpeed, true);
                currentTitle.transform.SetLocalX(-5 * scrollSpeed, true);
                nextTitle.transform.SetLocalX(-5 * scrollSpeed, true);
                
                yield return null;
            }
            
            // Reset images and titles for next click.
            currentImage.sprite = nextImage.sprite;
            currentImage.transform.localPosition = nextImage.transform.localPosition;
            nextImage.transform.SetLocalX(400);
            currentTitle.text = nextTitle.text;
            currentTitle.transform.localPosition = nextTitle.transform.localPosition;
            nextTitle.transform.SetLocalX(400);
            
            // Restore clicking functionality.
            leftButton.button.interactable = true;
            rightButton.button.interactable = true;
            _isScrolling = false;
        }

        [Serializable]
        private class GalleryItem
        {
            public string title;
            public Sprite image;
        }

        protected override void SetColors(ColorTheme theme)
        {
            currentTitle.color = nextTitle.color = theme.secondaryColor;
        }
    }
}

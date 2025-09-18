using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VRUIP
{
    public class Element : A_UIIntercations
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private AudioClip hoverSound;
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private bool hasOverlay;
        [SerializeField] private bool hasBanner;
        [SerializeField] private Image image;
        [SerializeField] private Image overlay;
        [SerializeField] private TextMeshProUGUI bannerTitle;

        private AudioSource _audioSource;
        protected bool initialized;
        protected Camera mainCamera;
        
        // Colors
        private readonly Color clickColor = new Color(0, 0, 0, 200/255f);
        private readonly Color hoverColor = new Color(1, 1, 1, 30/255f);

        public Vector2 Size
        {
            get
            {
                var rect = rectTransform.rect;
                var scale = rectTransform.localScale;
                return new Vector2(rect.width * scale.x, rect.height * scale.y);
            }
        }

        protected override void Start()
        {
            base.Start();
            if (!initialized) Initialize();
        }

        /// <summary>
        /// Create a copy of this element.
        /// </summary>
        public Element CreateElement(Vector3 position, Transform parent)
        {
            var clone = Instantiate(this, parent);
            clone.transform.localPosition = position;
            clone.Initialize();
            return clone;
        }
        
        /// <summary>
        /// Create an element from an ElementInfo.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="position"></param>
        /// <param name="parent"></param>
        public void CreateElementFromInfo(Collection.ElementInfo info, Vector3 position, Transform parent)
        {
            var element = CreateElement(position, parent);
            element.SetInfo(info);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Create a copy of this element in the editor.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="parent"></param>
        public Element CreateElementEditor(Vector3 position, Transform parent)
        {
            var clone = PrefabUtility.InstantiatePrefab(gameObject, parent) as GameObject;
            if (clone == null) throw new System.Exception("Could not instantiate element.");
            clone.transform.localPosition = position;
            return clone.GetComponent<Element>();
        }
        
        /// <summary>
        /// Create an element from an ElementInfo.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="position"></param>
        /// <param name="parent"></param>
        public void CreateElementFromInfoEditor(Collection.ElementInfo info, Vector3 position, Transform parent)
        {
            var element = CreateElementEditor(position, parent);
            element.SetInfo(info);
        }
#endif

        /// <summary>
        /// Initialize this Element with the correct info or properties.
        /// </summary>
        protected virtual void Initialize()
        {
            // Do the initialization here.
            initialized = true;
            _audioSource = VRUIPManager.instance.AudioSource;
            mainCamera = VRUIPManager.instance.mainCamera;
            RegisterOnEnter(() => _audioSource.PlayOneShot(hoverSound, 0.5f));
            RegisterOnClicked(() => _audioSource.PlayOneShot(clickSound));
            
            // If has an overlay, register the events.
            if (hasOverlay)
            {
                RegisterOnEnter(() => overlay.gameObject.SetActive(true));
                RegisterOnExit(() => overlay.gameObject.SetActive(false));
                RegisterOnDown(() => overlay.color = clickColor);
                RegisterOnUp(() => overlay.color = hoverColor);
            }
        }

        protected override void SetColors(ColorTheme theme)
        {
            // Nothing needed here.
        }

        private void SetInfo(Collection.ElementInfo info)
        {
            if (hasBanner) bannerTitle.text = info.title;
            image.sprite = info.sprite;
        }
    }
}

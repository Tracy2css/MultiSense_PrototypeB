using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class TabButtonController : A_UIIntercations
    {
        [Header("Tab Properties")]
        [SerializeField] private string tabName;
        [SerializeField] private AudioClip tabClickSound;
        [SerializeField] private AudioClip tabHoverSound;
    
        [Header("Components")]
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI tabText;
        [SerializeField] private TabController tabController;

        private Color _normalColor;
        private Color _hoverColor;
        private Color _selectedColor;
    
        private bool _isSelected;
        private AudioSource _audioSource;

        protected override void Start()
        {
            base.Start();
        
            // Set the tab name
            tabText.text = tabName;

            // Register functions
            RegisterOnEnter(OnEntered);
            RegisterOnExit(OnExited);
            RegisterOnDown(OnDown);
            RegisterOnClicked(OnClicked);
            
            // Get the audio source
            _audioSource = VRUIPManager.instance.AudioSource;
        }

        // This function is called by the TabController
        public void Initialize(TabController controller)
        {
            tabController = controller;
        }
        
        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            background.color = _isSelected ? _selectedColor : _normalColor;
        }

        private void OnEntered()
        {
            if (!_isSelected) background.color = _hoverColor;
            if (tabHoverSound != null) _audioSource.PlayOneShot(tabHoverSound, 0.5f);
        }
        
        private void OnExited()
        {
            if (!_isSelected) background.color = _normalColor;
        }
        
        private void OnDown()
        {
            if (!_isSelected) background.color = _selectedColor;
        }

        private void OnClicked()
        {
            tabController.SelectTab(this);
            if (tabClickSound != null) _audioSource.PlayOneShot(tabClickSound);
        }

        protected override void SetColors(ColorTheme theme)
        {
            _normalColor = theme.thirdColor;
            _hoverColor = theme.fourthColor;
            _selectedColor = theme.primaryColor;
        
            background.color = _isSelected ? _selectedColor : _normalColor;
            tabText.color = theme.secondaryColor;
        }
    
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (tabText != null) tabText.text = tabName;
            else Debug.LogWarning("VRUIP: Please assign text component to TabButtonController.");
        }
#endif
    }
}

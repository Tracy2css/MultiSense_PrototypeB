using TMPro;
using UnityEngine;

namespace VRUIP
{
    public class OptionController : A_ColorController
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI optionText;
        [SerializeField] private TextMeshProUGUI optionLabel;
        [SerializeField] private IconController leftArrowButton;
        [SerializeField] private IconController rightArrowButton;

        [Header("Options")]
        [SerializeField] private string[] options = { "Option 1", "Option 2", "Option 3" };

        [Header("Properties")]
        [SerializeField] private string optionLabelText;

        public string CurrentOption => options[_currentOptionIndex];

        private int _currentOptionIndex;

        public string[] Options
        {
            get => options;
            set => options = value;
        }
    
        private void Awake()
        {
            SetupOption();
        }

        /// <summary>
        /// Setup this Option instance.
        /// </summary>
        [ContextMenu("Setup Option (VRUIP)")]
        private void SetupOption()
        {
            optionLabel.text = optionLabelText;
            _currentOptionIndex = 0;
            optionText.text = options[_currentOptionIndex];
            leftArrowButton.RegisterOnClick(NavigateLeft);
            rightArrowButton.RegisterOnClick(NavigateRight);
        }

        private void NavigateLeft()
        {
            if (_currentOptionIndex == 0) _currentOptionIndex = options.Length - 1;
            else _currentOptionIndex--;
            optionText.text = options[_currentOptionIndex];
        }

        private void NavigateRight()
        {
            if (_currentOptionIndex == options.Length - 1) _currentOptionIndex = 0;
            else _currentOptionIndex++;
            optionText.text = options[_currentOptionIndex];
        }

        protected override void SetColors(ColorTheme theme)
        {
            optionText.color = theme.secondaryColor;
            optionLabel.color = theme.secondaryColor;
        }
    }
}

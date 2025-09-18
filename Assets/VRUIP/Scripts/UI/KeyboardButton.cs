using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class KeyboardButton : A_ColorController
    {
        [Header("Button Type")]
        [SerializeField] private KeyboardButtonType buttonType;
        
        [Header("Button Properties")]
        [SerializeField] private string character;
        [SerializeField] private Keyboard keyboard;

        [Header("Button Components")]
        [SerializeField] private Image buttonBackground;
        [SerializeField] private TextMeshProUGUI buttonText;

        public string Character => character;
        public KeyboardButtonType Type => buttonType;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClicked);
        }

        /// <summary>
        /// On this button clicked.
        /// </summary>
        private void OnClicked()
        {
            keyboard.ButtonPressed(this);
        }

        public enum KeyboardButtonType
        {
            Character,
            Space,
            Enter,
            Tab,
            Delete,
            Shift,
            Caps,
            Custom,
            Clear
        }

        /// <summary>
        /// Set the parent keyboard that this button belongs to.
        /// </summary>
        /// <param name="parentKeyboard"></param>
        public void SetKeyboard(Keyboard parentKeyboard)
        {
            keyboard = parentKeyboard;
        }

        protected override void SetColors(ColorTheme theme)
        {
            buttonBackground.color = theme.thirdColor;
            buttonText.color = theme.secondaryColor;
        }
    }
}

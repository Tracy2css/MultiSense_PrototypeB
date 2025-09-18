using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VRUIP
{
    public class Keyboard : A_Canvas
    {
        // KEYBOARD PROPERTIES
        [SerializeField] private KeyboardFieldType fieldType;
        [SerializeField] private TMP_InputField tmpInput;
        [SerializeField] private TextMeshProUGUI tmpText;
        [SerializeField] [Tooltip("Number of spaces added when Tab is clicked.")] private int tabSpaces;
        
        // KEYBOARD COMPONENTS
        [SerializeField] private Image background;
        [SerializeField] private GameObject normalSection;
        [SerializeField] private GameObject shiftSection;
        [SerializeField] private Transform contentTransform;
        [SerializeField] private GameObject handles;

        // KEYBOARD AUDIO
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip shiftCapsClickSound;
        [SerializeField] private AudioClip spaceClickSound;
        [SerializeField] private AudioClip backspaceClickSound;
        [SerializeField] private AudioClip enterClickSound;
        
        private bool _caps;
        private bool _shift;
        private EventSystem _eventSystem;
        private int _legacyInputCaretPosition;
        
        public bool IsTmpInput => fieldType == KeyboardFieldType.TMPInputField;

        protected override void Start()
        {
            base.Start();
            _eventSystem = EventSystem.current;
            
            // if part of canvas, disable movement handles.
            if (GetComponentInParent<Canvas>() != null) handles.SetActive(false);
        }

        public Keyboard Create(Vector3 position, Quaternion rotation, Vector3 contentRotation, bool fadeIn = false)
        {
            var clone = Instantiate(this, position, rotation);
            var angle = clone.transform.eulerAngles;
            clone.transform.eulerAngles = new Vector3(0, angle.y, 0f);
            clone.SetContentRotation(contentRotation);
            if (fadeIn)
            {
                clone.SetAlpha(0);
                clone.FadeInCanvas();
            }
            return clone;
        }

        /// <summary>
        /// Keyboard Button pressed.
        /// </summary>
        /// <param name="button"></param>
        public void ButtonPressed(KeyboardButton button)
        {
            // Check if there is an input field assigned.
            if (tmpInput == null && tmpText == null)
            {
                Debug.LogWarning("No input field is assigned to the keyboard.");
                return;
            }
            
            // Choose what to do based on button type.
            switch (button.Type)
            {
                case KeyboardButton.KeyboardButtonType.Character:
                case KeyboardButton.KeyboardButtonType.Custom:
                    audioSource.clip = buttonClickSound;
                    audioSource.Play();
                    Type(button.Character);

                    // If shift was previously pressed and caps lock is off, turn it off.
                    if (_shift && !_caps)
                    {
                        normalSection.SetActive(true);
                        shiftSection.SetActive(false);
                        _shift = false;
                    }
                    
                    break;

                case KeyboardButton.KeyboardButtonType.Space:
                    audioSource.clip = spaceClickSound;
                    audioSource.Play();
                    Type(" ");
                    break;
                
                case KeyboardButton.KeyboardButtonType.Enter:
                    audioSource.clip = enterClickSound;
                    audioSource.Play();
                    Type("\n");
                    break;
                
                case KeyboardButton.KeyboardButtonType.Delete:
                    audioSource.clip = backspaceClickSound;
                    audioSource.Play();
                    Delete();
                    break;

                case KeyboardButton.KeyboardButtonType.Tab:
                    audioSource.clip = spaceClickSound;
                    audioSource.Play();
                    var tab = new string(' ', Math.Abs(tabSpaces));
                    Type(tab);
                    break;
                
                case KeyboardButton.KeyboardButtonType.Caps:
                    audioSource.clip = shiftCapsClickSound;
                    audioSource.Play();
                    normalSection.SetActive(_caps);
                    shiftSection.SetActive(!_caps);
                    // If caps lock is on, turn off shift.
                    if (_caps) _shift = false;
                    _caps = !_caps;
                    break;
                
                case KeyboardButton.KeyboardButtonType.Shift:
                    audioSource.clip = shiftCapsClickSound;
                    audioSource.Play();
                    // If caps lock is on, don't do anything.
                    if (_caps) return;
                    normalSection.SetActive(_shift);
                    shiftSection.SetActive(!_shift);
                    _shift = !_shift;
                    break;
                
                case KeyboardButton.KeyboardButtonType.Clear:
                    audioSource.clip = backspaceClickSound;
                    audioSource.Play();
                    switch (fieldType)
                    {
                        case KeyboardFieldType.TMPInputField:
                            tmpInput.text = "";
                            break;
                        case KeyboardFieldType.TMPText:
                            tmpText.text = "";
                            break;
                    }
                    break;
            }
        }
        
        // Type in the keyboard's selected input field.
        private void Type(string text)
        {
            switch (fieldType)
            {
                case KeyboardFieldType.TMPInputField:
                    TypeTMP(text);
                    break;
                case KeyboardFieldType.TMPText:
                    tmpText.text += text;
                    break;
            }
        }
        
        /// <summary>
        /// Type in a TMP Input Field
        /// </summary>
        /// <param name="text"></param>
        private void TypeTMP(string text)
        {
            // Set the input field text as the selected UI.
            if (_eventSystem.currentSelectedGameObject != tmpInput.gameObject)
            {
                _eventSystem.SetSelectedGameObject(tmpInput.gameObject);
            }
            
            // If there is a selection, delete it.
            if (tmpInput.selectionAnchorPosition != tmpInput.selectionFocusPosition)
            {
                DeleteSelectedTextTMP();
            }
            
            // Type the character at the caret position.
            var caretPos = tmpInput.caretPosition;
            tmpInput.text = tmpInput.text.Insert(caretPos, text);
            tmpInput.caretPosition = caretPos + text.Length;
            tmpInput.selectionAnchorPosition = tmpInput.caretPosition;
            tmpInput.selectionFocusPosition = tmpInput.caretPosition;

            // Activate the input field to show the caret
            tmpInput.ActivateInputField();
        }

        /// <summary>
        /// Delete button functionality.
        /// </summary>
        private void Delete()
        {
            switch (fieldType)
            {
                case KeyboardFieldType.TMPInputField:
                    DeleteTMP();
                    break;
                case KeyboardFieldType.TMPText:
                    if (tmpText.text.Length == 0) return;
                    tmpText.text = tmpText.text.Substring(0, tmpText.text.Length - 1);
                    break;
            }
        }
        
        /// <summary>
        /// Delete the last character in a TMP Input Field
        /// </summary>
        private void DeleteTMP()
        {
            // if there is no text, return
            if (tmpInput.text.Length == 0) return;
            
            // Set the input field text as the selected UI.
            if (_eventSystem.currentSelectedGameObject != tmpInput.gameObject)
            {
                _eventSystem.SetSelectedGameObject(tmpInput.gameObject);
            }

            // If there is a selection, delete it.
            if (tmpInput.selectionAnchorPosition != tmpInput.selectionFocusPosition)
            {
                DeleteSelectedTextTMP();
            }
            // If there is no selection, delete the last character.
            else
            {
                var caretPos = tmpInput.caretPosition;

                if (caretPos <= tmpInput.text.Length)
                {
                    if (caretPos == 0) return; // Can't delete if the caret is at the start of the text.
                    tmpInput.text = tmpInput.text.Remove(caretPos - 1, 1);
                    tmpInput.caretPosition = caretPos - 1;
                }
                
                tmpInput.ActivateInputField();
            }
        }

        /// <summary>
        /// Delete the selected text in a TMP Input Field.
        /// </summary>
        private void DeleteSelectedTextTMP()
        {
            var startPos = Mathf.Min(tmpInput.selectionAnchorPosition, tmpInput.selectionFocusPosition);
            var endPos = Mathf.Max(tmpInput.selectionAnchorPosition, tmpInput.selectionFocusPosition);

            if (startPos != endPos)
            {
                tmpInput.text = tmpInput.text.Remove(startPos, endPos - startPos);
                tmpInput.selectionAnchorPosition = startPos;
                tmpInput.selectionFocusPosition = startPos;
            }
        }

        private enum KeyboardFieldType
        {
            TMPInputField,
            TMPText
        }

        protected override void SetColors(ColorTheme theme)
        {
            background.color = theme.primaryColor;
        }

        public void SetInput(TMP_InputField input)
        {
            fieldType = KeyboardFieldType.TMPInputField;
            tmpInput = input;
        }

        // Set the rotation of the content without the handle.
        public void SetContentRotation(Vector3 rotation)
        {
            contentTransform.localEulerAngles = rotation;
        }
    }
}

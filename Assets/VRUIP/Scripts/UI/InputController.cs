using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VRUIP
{
    public class InputController : A_ColorController
    {
        [Header("Components")]
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Image background;

        private bool _textIsHidden;
        private bool _alreadySelected;

        private void Awake()
        {
            // Register functions for select and deselect events.
            inputField.onSelect.AddListener(arg => OnInputSelected());
            inputField.onDeselect.AddListener(arg => StartCoroutine(DelayedSelectionCheck()));
        }

        private IEnumerator DelayedSelectionCheck()
        {
            yield return null; // Wait for one frame

            var selectedObject = EventSystem.current.currentSelectedGameObject;
            var isNull = selectedObject == null;

            // if selected object is an input field, set the keyboard input to that input field
            if (!isNull && selectedObject.transform.parent.TryGetComponent<InputController>(out var newInputField))
            {
                VRUIPManager.instance.SetKeyboardInput(newInputField.inputField);
                _alreadySelected = false;
                yield break;
            }
            
            // If the selected object is null or not the input field or a keyboard button or a move handler, hide the keyboard
            var hideKeyboard = isNull || (selectedObject != inputField.gameObject &&
                                          selectedObject.GetComponent<KeyboardButton>() == null &&
                                          selectedObject.GetComponent<MoveHandler>() == null);

            // Hide the keyboard and reset booleans to false.
            if (hideKeyboard)
            {
                VRUIPManager.instance.HideKeyboard();
                VRUIPManager.instance.IsKeyboardOnInput = false;
                _alreadySelected = false;
            }
        }

        public string Text
        {
            get => inputField.text;
            set => inputField.text = value;
        }
        public bool TextIsHidden => _textIsHidden;

        // Toggles the text between hidden (as in passwords) and visible.
        public void ToggleTextHidden(bool hide)
        {
            inputField.contentType = hide ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            inputField.ForceLabelUpdate();
            _textIsHidden = hide;
        }
        
        protected override void SetColors(ColorTheme theme)
        {
            background.color = theme.secondaryColor;
            inputField.textComponent.color = theme.primaryColor;
            inputField.placeholder.color = theme.thirdColor;
            inputField.selectionColor = theme.fourthColor;
        }

        private void OnDisable()
        {
            _alreadySelected = false;
        }

        private void OnInputSelected()
        {
            // if this input is already selected, don't do anything.
            if (_alreadySelected) return;

            // Set this input field as selected.
            _alreadySelected = true;

            // If keyboard didn't have input selected as last thing, show the keyboard, otherwise don't do anything.
            if (!VRUIPManager.instance.IsKeyboardOnInput)
            {
                // Show the keyboard.
                VRUIPManager.instance.ShowKeyboard();
                
                // Set the keyboard to be on input.
                VRUIPManager.instance.IsKeyboardOnInput = true;
                
                // Set the keyboard input to this input field.
                VRUIPManager.instance.SetKeyboardInput(inputField);
            }
        }
    }
}

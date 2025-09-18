using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VRUIP
{
    public class DropdownController : A_ColorController
    {
        [Header("Colors")] 
        [SerializeField] private Color normalColor;
        [SerializeField] private Color hoverColor;
        [SerializeField] private Color pressedColor;
        [SerializeField] private Color textColor;
        
        [Header("Components")]
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private Toggle option;
        [SerializeField] private TextMeshProUGUI selectedText;
        [SerializeField] private TextMeshProUGUI optionText;

        private void Awake()
        {
            SetupDropdown();
        }

        [ContextMenu("Setup Dropdown (VRUIP)")]
        private void SetupDropdown()
        {
            var newColors = new ColorBlock()
            {
                normalColor = normalColor,
                highlightedColor = hoverColor,
                pressedColor = pressedColor,
                selectedColor = hoverColor,
                colorMultiplier = 1,
            };
            dropdown.colors = newColors;
            option.colors = newColors;
            selectedText.color = optionText.color = textColor;
        }

        protected override void SetColors(ColorTheme theme)
        {
            normalColor = theme.secondaryColor;
            hoverColor = theme.fourthColor;
            pressedColor = theme.thirdColor;
            textColor = theme.primaryColor;
            dropdown.Hide();
            if (EventSystem.current.currentSelectedGameObject == dropdown.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            SetupDropdown();
        }
    }
}

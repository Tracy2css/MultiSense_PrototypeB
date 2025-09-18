using System;
using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class TabController : A_ColorController
    {
        [Header("Tab Properties")]
        [SerializeField] private Tab[] tabs;

        [Header("Components")]
        [SerializeField] private Image tabsBackground;
        private TabButtonController _activeTab;

        private void Awake()
        {
            InitializeTabButtons();
        }

        protected override void Start()
        {
            // Select the first tab
            if (tabs.Length > 0) 
            {
                SelectTab(tabs[0].button);
            }
        }

        private void InitializeTabButtons()
        {
            foreach (var tab in tabs)
            {
                tab.button.Initialize(this);
            }
        }

        // Selects the tab at the given index
        public void SelectTab(int index)
        {
            if (index < 0 || index >= tabs.Length) return;
            SelectTab(tabs[index].button);
        }

        // Selects the given tab button and deselects all others
        public void SelectTab(TabButtonController tabButton)
        {
            foreach (var tab in tabs)
            {
                if (tab.button == tabButton)
                {
                    tab.button.SetSelected(true);   // set the current button to selected
                    tab.objectToActivate.SetActive(true); // activate the object
                    _activeTab = tab.button;       // set the active tab to the current button
                }
                else
                {
                    tab.button.SetSelected(false); // deactivate the other buttons
                    tab.objectToActivate.SetActive(false); // deactivate the other objects
                }
            }
        }

        protected override void SetColors(ColorTheme theme)
        {
            tabsBackground.color = theme.thirdColor;
        }

        [Serializable]
        private class Tab
        {
            public TabButtonController button; // TabbUTTON（Button va, aa, ta）
            public GameObject objectToActivate; // ButtonObject（Object VA, AA, TA）

        }
    }
}
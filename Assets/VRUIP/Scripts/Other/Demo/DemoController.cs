using System;
using System.Collections;
using UnityEngine;

namespace VRUIP.Demo
{
    public class DemoController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private IconController nextDemoIcon;
        [SerializeField] private IconController previousDemoIcon;
        [SerializeField] private SwitchController themeSwitch;
        [SerializeField] private Transform demoContainer;
        [SerializeField] private GameObject navigationPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject demoTypePanel;
        [SerializeField] private GameObject welcomePanel;
        [SerializeField] private ButtonController uiDemoButton;
        [SerializeField] private ButtonController toolsDemoButton;
        [SerializeField] private ButtonController ui3DDemoButton;
        [SerializeField] private ButtonController mainMenuButton;
        [SerializeField] private IconController settingsIcon;
        [SerializeField] private TextController currentDemoTitle;

        [Header("Demo Sections")]
        [SerializeField] private UIDemoSection[] demoSections;
        [SerializeField] private ToolsDemoSection[] toolsDemoSections;
        [SerializeField] private ToolsDemoSection[] UI3DDemoSections;
        
        private int _currentUISectionIndex;
        private int _currentToolSectionIndex;
        private int _current3DUISectionIndex;
        private bool _isTransitioning = false;
        private Transform _cameraTransform;
        private DemoType currentDemoType;

        private void Awake()
        {
            SetupDemo();
        }

        private void Start()
        {
            themeSwitch.RegisterOnValueChanged(ChangeTheme);
            if (!VRUIPManager.instance.IsVR) return;
            _cameraTransform = VRUIPManager.instance.mainCamera.transform;
            StartCoroutine(SetDemoPosition());
        }

        private void SetupDemo()
        {
            for (var i = 0; i < demoSections.Length; i++)
            {
                if (i == 0) continue;
                demoSections[i].HideOnStart();
            }
            uiDemoButton.RegisterOnClick(() => StartDemo(DemoType.UI));
            toolsDemoButton.RegisterOnClick(() => StartDemo(DemoType.Tools));
            ui3DDemoButton.RegisterOnClick(() => StartDemo(DemoType.UI3D));
            settingsIcon.RegisterOnClick(OnSettingsButtonClicked);
            mainMenuButton.RegisterOnClick(OnMainMenuButtonClicked);
        }
        
        private void OnMainMenuButtonClicked()
        {
            navigationPanel.SetActive(false);
            demoTypePanel.SetActive(true);
            welcomePanel.SetActive(true);
            settingsPanel.SetActive(false);

            if (currentDemoType == DemoType.UI)
            {
                demoSections[_currentUISectionIndex].HideSection();
            }
            else if (currentDemoType == DemoType.Tools)
            {
                toolsDemoSections[_currentToolSectionIndex].HideSection();
            }
            else if (currentDemoType == DemoType.UI3D)
            {
                UI3DDemoSections[_current3DUISectionIndex].HideSection();
            }
            
            _currentUISectionIndex = 0;
            _currentToolSectionIndex = 0;
            _current3DUISectionIndex = 0;
        }

        private void OnSettingsButtonClicked()
        {
            settingsPanel.SetActive(!settingsPanel.activeInHierarchy);
        }

        /// <summary>
        /// Start a demo of a certain type.
        /// </summary>
        /// <param name="type"></param>
        public void StartDemo(DemoType type)
        {
            welcomePanel.SetActive(false);
            demoTypePanel.SetActive(false);
            navigationPanel.SetActive(true);
            switch (type)
            {
                case DemoType.UI:
                    demoSections[0].ShowSection();
                    nextDemoIcon.ClearListeners();
                    previousDemoIcon.ClearListeners();
                    nextDemoIcon.RegisterOnClick(NextUIDemo);
                    previousDemoIcon.RegisterOnClick(PreviousUIDemo);
                    currentDemoTitle.Text = demoSections[0].name;
                    currentDemoType = DemoType.UI;
                    break;
                case DemoType.Tools:
                    toolsDemoSections[0].ShowSection();
                    nextDemoIcon.ClearListeners();
                    previousDemoIcon.ClearListeners();
                    nextDemoIcon.RegisterOnClick(NextToolDemo);
                    previousDemoIcon.RegisterOnClick(PreviousToolDemo);
                    currentDemoTitle.Text = toolsDemoSections[0].name;
                    currentDemoType = DemoType.Tools;
                    break;
                case DemoType.UI3D:
                    UI3DDemoSections[0].ShowSection();
                    nextDemoIcon.ClearListeners();
                    previousDemoIcon.ClearListeners();
                    nextDemoIcon.RegisterOnClick(Next3DUIDemo);
                    previousDemoIcon.RegisterOnClick(Previous3DUIDemo);
                    currentDemoTitle.Text = UI3DDemoSections[0].name;
                    currentDemoType = DemoType.UI3D;
                    break;
            }
        }
        
        private void NextUIDemo()
        {
            if (_isTransitioning) return;
            if (_currentUISectionIndex >= demoSections.Length - 1) return;
            _isTransitioning = true;
            demoSections[_currentUISectionIndex].HideSection();
            _currentUISectionIndex++;
            demoSections[_currentUISectionIndex].ShowSection(() => _isTransitioning = false);
            currentDemoTitle.Text = demoSections[_currentUISectionIndex].name;
        }
        
        private void PreviousUIDemo()
        {
            if (_isTransitioning) return;
            if (_currentUISectionIndex <= 0) return;
            _isTransitioning = true;
            demoSections[_currentUISectionIndex].HideSection();
            _currentUISectionIndex--;
            demoSections[_currentUISectionIndex].ShowSection(() => _isTransitioning = false);
            currentDemoTitle.Text = demoSections[_currentUISectionIndex].name;
        }

        private void NextToolDemo()
        {
            if (_currentToolSectionIndex >= toolsDemoSections.Length - 1) return;
            toolsDemoSections[_currentToolSectionIndex].HideSection();
            _currentToolSectionIndex++;
            toolsDemoSections[_currentToolSectionIndex].ShowSection();
            currentDemoTitle.Text = toolsDemoSections[_currentToolSectionIndex].name;
        }
        
        private void PreviousToolDemo()
        {
            if (_currentToolSectionIndex <= 0) return;
            toolsDemoSections[_currentToolSectionIndex].HideSection();
            _currentToolSectionIndex--;
            toolsDemoSections[_currentToolSectionIndex].ShowSection();
            currentDemoTitle.Text = toolsDemoSections[_currentToolSectionIndex].name;
        }
        
        private void Next3DUIDemo()
        {
            if (_current3DUISectionIndex >= UI3DDemoSections.Length - 1) return;
            UI3DDemoSections[_current3DUISectionIndex].HideSection();
            _current3DUISectionIndex++;
            UI3DDemoSections[_current3DUISectionIndex].ShowSection();
            currentDemoTitle.Text = UI3DDemoSections[_current3DUISectionIndex].name;
        }
        
        private void Previous3DUIDemo()
        {
            if (_current3DUISectionIndex <= 0) return;
            UI3DDemoSections[_current3DUISectionIndex].HideSection();
            _current3DUISectionIndex--;
            UI3DDemoSections[_current3DUISectionIndex].ShowSection();
            currentDemoTitle.Text = UI3DDemoSections[_current3DUISectionIndex].name;
        }

        private void ChangeTheme(bool isLightMode)
        {
            VRUIPManager.instance.colorMode = isLightMode ? VRUIPManager.ColorThemeMode.LightMode : VRUIPManager.ColorThemeMode.DarkMode;
            VRUIPManager.instance.SetTheme();
        }

        private IEnumerator SetDemoPosition()
        {
            while (_cameraTransform.position.y == 0)
            {
                yield return null;
            }
            demoContainer.SetY(_cameraTransform.position.y - .1f);
        }

        public enum DemoType
        {
            UI,
            Tools,
            UI3D
        }
    }
    
    [Serializable]
    class UIDemoSection
    {
        public string name;
        public A_Canvas[] canvases;
        public GameObject[] objects;

        public void HideSection(Action callback = null)
        {
            foreach (var canvas in canvases)
            {
                if (canvas.gameObject.activeInHierarchy) canvas.FadeOutCanvas(callback);
            }

            foreach (var obj in objects)
            {
                obj.SetActive(false);
            }
        }
        
        public void ShowSection(Action callback = null)
        {
            foreach (var canvas in canvases)
            {
                if (!canvas.IsSetup) canvas.Setup();
                canvas.FadeInCanvas(callback);
            }
            foreach (var obj in objects)
            {
                obj.SetActive(true);
            }
        }
        
        public void HideOnStart()
        {
            foreach (var canvas in canvases)
            {
                canvas.SetAlpha(0);
                canvas.gameObject.SetActive(false);
            }
        }
    }

    [Serializable]
    class ToolsDemoSection
    {
        public string name;
        public GameObject[] objects;
        
        public void HideSection()
        {
            foreach (var obj in objects)
            {
                obj.SetActive(false);
            }
        }
        
        public void ShowSection()
        {
            foreach (var obj in objects)
            {
                obj.SetActive(true);
            }
        }
    }
}

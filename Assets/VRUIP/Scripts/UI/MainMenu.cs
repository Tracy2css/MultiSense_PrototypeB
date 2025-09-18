using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VRUIP
{
    public class MainMenu : A_Canvas
    {
        [Header("Common Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        [Header("Pages")]
        [SerializeField] private GameObject mainPage;
        [SerializeField] private Button[] pageButtons; // Buttons that take you to a different page on menu
        [SerializeField] private GameObject[] pages;
        [Header("Events")]
        public UnityEvent onPlay;
        public UnityEvent onQuit;
        [Header("Components")]
        [SerializeField] private Image background;

        protected override void Start()
        {
            base.Start();
            InitializeButtons();
        }

        protected override void SetColors(ColorTheme theme)
        {
            background.color = theme.primaryColor;
        }

        /// <summary>
        /// Initialize main menu buttons.
        /// </summary>
        private void InitializeButtons()
        {
            // Initialize Play and Quit buttons
            playButton.onClick.AddListener(OnPlayButton);
            quitButton.onClick.AddListener(OnQuitButton);
            
            // Make sure equal number of pages and page buttons.
            if (pageButtons.Length != pages.Length)
            {
                Debug.Log("Main Menu: please enter an equal number of page buttons and pages in the inspector of the MainMenu script.");
                return;
            }

            // Initialize page buttons
            for (int i = 0; i < pageButtons.Length; i++)
            {
                var temp = i;
                pageButtons[i].onClick.AddListener(() => SwitchToPage(pages[temp]));
            }
        }

        private void SwitchToPage(GameObject page)
        {
            if (!pages.Contains(page))
            {
                Debug.Log("This page object is not in the list of pages.");
                return;
            }
            mainPage.SetActive(false);
            foreach (var pageObject in pages)
            {
                pageObject.SetActive(page == pageObject);
            }
        }

        private void OnPlayButton()
        {
            onPlay.Invoke();
        }

        private void OnQuitButton()
        {
            /* UNCOMMENT THIS CODE IF YOU WANT TO QUIT THE APPLICATION
            onQuit?.Invoke();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
            */
        }

        public void SwitchToMainPage()
        {
            foreach (var page in pages)
            {
                page.SetActive(false);
            }
            mainPage.SetActive(true);
        }
    }
}

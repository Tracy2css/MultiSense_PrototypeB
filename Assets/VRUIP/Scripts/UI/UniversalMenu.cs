using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class UniversalMenu : A_Canvas
    {
        [Header("Windows")]
        [SerializeField] private UniversalMenuWindow[] windows;
        
        [Header("Menu Components")]
        // [SerializeField] private TextMeshProUGUI time;
        [SerializeField] private Image background;
        [SerializeField] private Image volumeBackground;
        

        // private DateTime _currentTime;
        private bool _windowIsTransitioning;

        // private void Awake()
        // {
        //     Initialize();
        // }

        // private void Update()
        // {
        //     UpdateTime();
        // }

        // private void Initialize()
        // {
        //     _currentTime = DateTime.Now;
        //     time.text = _currentTime.ToString("HH:mm");
        // }

        // private void UpdateTime()
        // {
        //     if (DateTime.Now.Minute != _currentTime.Minute)
        //     {
        //         _currentTime = DateTime.Now;
        //         time.text = _currentTime.ToString("HH:mm");
        //     }
        // }

        /// <summary>
        /// Open a certain window in universal menu.
        /// </summary>
        /// <param name="window"></param>
        public void OpenWindow(UniversalMenuWindow window)
        {
            if (_windowIsTransitioning) return;
            // If the window is not in the list, return.
            if (!windows.Contains(window)) return;
            
            // If the window is already active, close it and all windows.
            _windowIsTransitioning = true;
            if (window.IsOpen)
            {
                foreach (var windowObject in windows)
                {
                    windowObject.SetOpen(false, () => _windowIsTransitioning = false);
                }
                return;
            }
            
            // Close all windows and open the desired window.
            foreach (var windowObject in windows)
            {
                windowObject.SetOpen(windowObject == window, () => _windowIsTransitioning = false);
            }
        }

        /// <summary>
        /// Enable an object if it's disabled and disable it if it's enabled.
        /// </summary>
        /// <param name="obj"></param>
        public void EnableOrDisableObject(GameObject obj)
        {
            obj.SetActive(!obj.activeInHierarchy);
        }

        protected override void SetColors(ColorTheme theme)
        {
            background.color = theme.primaryColor;
            volumeBackground.color = theme.primaryColor;
            // time.color = theme.secondaryColor;
        }
    }
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
#if META_SDK
using Oculus.Interaction;
#endif

namespace VRUIP
{
    public class VRUIPManager : MonoBehaviour
    {
        public static VRUIPManager instance;
        
        // Properties
        public ColorThemeMode colorMode;
        public ColorTheme customColorTheme;

        // Framework Specific Components
        public Camera mainCamera;
        #if !META_SDK
        public LineRenderer lineRenderer;
        #else
        public RayInteractor handRayInteractor;
        public RayInteractor controllerRayInteractor;
        public GrabInteractor controllerGrabInteractor;
        #endif
        #if OCULUS_INTEGRATION
        public LaserPointer laserPointer;
        public OVRGrabber leftHand;
        public OVRGrabber rightHand;
        #endif

        // UI 
        public ScaleUIButton scaleButton;
        [SerializeField] private Keyboard keyboardPrefab;

        
        // PROPERTIES
        
        /// <summary>
        /// Returns the current Color theme, or sets it.
        /// </summary>
        public ColorTheme CurrentColorTheme
        {
            get => _colorThemes[colorMode];
            set
            {
                colorMode = ColorThemeMode.Custom;
                customColorTheme = value;
                SetTheme();
            }
        }
        
        public bool IsVR { get; private set; }
        public AudioSource AudioSource => _audioSource;
        public bool IsKeyboardOnInput { get; set; } = false;

        private List<A_ColorController> _colorControllers = new();
        private Dictionary<ColorThemeMode, ColorTheme> _colorThemes;
        private AudioSource _audioSource;
        private Keyboard _keyboard;

        private void Awake()
        {
            SetupInstance();
        }

        private void Start()
        {
#if OCULUS_INTEGRATION
            laserPointer.laserBeamBehavior = LaserPointer.LaserBeamBehavior.OnWhenHitTarget;
#endif
        }

        private void SetupInstance()
        {
            if (instance == null)
            {
                instance = this;
#if !META_SDK
                if (lineRenderer != null) lineRenderer.sortingOrder = 30001;
#endif
                if (_audioSource == null) _audioSource = GetComponentInChildren<AudioSource>();
                SetupThemes();
#if (OCULUS_INTEGRATION || XR_ITK || META_SDK)
                IsVR = true;
#else
                IsVR = false;
#endif
            }
        }

        public void GoToScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        public void ShowKeyboard()
        {
            // if there is no keyboard assigned, throw an error
            if (keyboardPrefab == null)
            {
                Debug.LogWarning("Error: Please assign a keyboard prefab to the VRUIPManager.");
                return;
            }
            
            // calculate the spawn point of the keyboard
            var spawnPoint = CalculateSpawnPoint();

            // if the keyboard has not been spawned yet, spawn it
            if (_keyboard == null)
            {
                // if first time showing the keyboard, spawn it
                _keyboard = keyboardPrefab.Create(spawnPoint.Item1, spawnPoint.Item2, spawnPoint.Item3, true);
            }
            else
            {
                // if the keyboard has already been spawned, just move it to the spawn point and fade it in
                var keyboardTransform = _keyboard.transform;
                keyboardTransform.position = spawnPoint.Item1;
                keyboardTransform.rotation = spawnPoint.Item2;
                _keyboard.SetContentRotation(spawnPoint.Item3);
                keyboardTransform.eulerAngles = new Vector3(0, keyboardTransform.eulerAngles.y, 0);
                
                // if the keyboard is not active, activate it and set the alpha to 0 since the FadeInCanvas function activates the canvas object not the keyboards.
                if (!_keyboard.gameObject.activeInHierarchy)
                {
                    _keyboard.gameObject.SetActive(true);
                    _keyboard.SetAlpha(0);
                }
                
                // fade in the keyboard
                _keyboard.FadeInCanvas();
            }
        }

        public void HideKeyboard()
        {
            // if there is no keyboard assigned, return
            if (_keyboard == null) return;
            
            // if the keyboard is active, fade it out
            if (_keyboard.gameObject.activeInHierarchy)
            {
                _keyboard.FadeOutCanvas();
            }
        }
        
        #if META_SDK
        /// <summary>
        /// Get the position of the ray interactor that is currently active.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool GetRayInteractorPosition(out Vector3 position)
        {
            position = Vector3.zero;
            if (handRayInteractor.isActiveAndEnabled)
            {
                if (handRayInteractor.CollisionInfo.HasValue)
                {
                    position = handRayInteractor.CollisionInfo.Value.Point;
                    return true;
                }
            }
            else if (controllerRayInteractor.isActiveAndEnabled)
            {
                if (controllerRayInteractor.CollisionInfo.HasValue)
                {
                    position = controllerRayInteractor.CollisionInfo.Value.Point;
                    return true;
                }
            }

            return false;
        }
        #endif

        /// <summary>
        /// Set the input field for the default keyboard.
        /// </summary>
        /// <param name="inputField"></param>
        public void SetKeyboardInput(TMP_InputField inputField)
        {
            if (_keyboard == null)
            {
                Debug.LogError("Error: Keyboard is not spawned yet.");
                return;
            }
            _keyboard.SetInput(inputField);
        }

        private (Vector3, Quaternion, Vector3) CalculateSpawnPoint()
        {
            var cameraTransform = mainCamera.transform;
            var forward = cameraTransform.forward;
            var spawnPosition = cameraTransform.position + forward * .6f + new Vector3(0, -0.4f, 0); // Adjust the spawn distance as needed
            var spawnRotation = Quaternion.LookRotation(spawnPosition - cameraTransform.position);
            var contentRotation = new Vector3(0f, 0f, 0f);

            return (spawnPosition, spawnRotation, contentRotation);
        }

        /// <summary>
        /// Set the theme of the UI.
        /// </summary>
        public void SetTheme()
        {
            foreach (var controller in _colorControllers)
            {
                controller.SetupElement(_colorThemes[colorMode]);
            }
        }

        private void SetupThemes()
        {
            // Dark Mode
            var darkPrimary = Util.HexToColor("#262626");
            var darkSecondary = Util.HexToColor("#ffffff");
            var darkThird = Util.HexToColor("#454545");
            var darkFourth = Util.HexToColor("#8e65bf");
            // Light Mode:
            var lightPrimary = Util.HexToColor("#FFFFFF");
            var lightSecondary = Util.HexToColor("#4d4d4d");
            var lightThird = Util.HexToColor("#dedede");
            var lightFourth = Util.HexToColor("#498feb");

            _colorThemes = new Dictionary<ColorThemeMode, ColorTheme>
            {
                { ColorThemeMode.DarkMode, new ColorTheme(darkPrimary, darkSecondary, darkThird, darkFourth) },
                { ColorThemeMode.LightMode, new ColorTheme(lightPrimary, lightSecondary, lightThird, lightFourth) },
                { ColorThemeMode.Custom, customColorTheme }
            };
        }

        public void RegisterElement(A_ColorController element)
        {
            _colorControllers.Add(element);
        }

        // ENUMS ----------
        public enum TargetFramework
        {
            UnityEditorTesting,
            OculusIntegration,
            XRInteractionToolkit,
            MetaSDK
        }

        public enum ColorThemeMode
        {
            DarkMode,
            LightMode,
            Custom
        }
        // ----------
        
#if UNITY_EDITOR        
        private void OnValidate()
        {
            if (IsPrefab()) return;
            SetupInstance();
        }

        private bool IsPrefab()
        {
            return string.IsNullOrEmpty(gameObject.scene.path) || string.IsNullOrEmpty(gameObject.scene.name);
        }
#endif
    }
}

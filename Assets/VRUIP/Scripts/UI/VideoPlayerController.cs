using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace VRUIP
{
    public class VideoPlayerController : A_Canvas
    {
        [Header("Custom Properties")]
        [SerializeField] private string videoTitle;
        [SerializeField] private VideoClip video;
        [SerializeField] [Tooltip("Hide the overlay if nothing on the screen is pressed for a certain time.")]
        private bool hideOverlayIfInactive;
        [SerializeField] [Tooltip("The overlay gets hidden after this many seconds if nothing is pressed (If hide is active).")]
        private float hideOverlayAfter = 5f;

        [Header("Components")]
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private CanvasGroup overlay;
        [SerializeField] private IconController playButton;
        [SerializeField] private IconController volumeButton;
        [SerializeField] private Slider videoSlider;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private SliderController volumeSlider;
        [SerializeField] private Sprite playSprite;
        [SerializeField] private Sprite pauseSprite;
        [SerializeField] private Image playButtonBackground;
        [SerializeField] private Image volumeButtonBackground;
        [SerializeField] private Image handleImage;

        private bool _overlayActive = true;
        private bool _inTransition;
        private bool _playing;
        private bool _blockHideOverlayCounter;
        private bool _preSliderVideoIsPlaying;
        private string _currentVideoLengthString;
        private double _previousTime;
        private float _overlayHideCounter;

        private void Awake()
        {
            SetupVideoPlayer();
        }

        private void SetupVideoPlayer()
        {
            playButton.RegisterOnClick(OnPlayButtonClick);
            // Add event for on video finished.
            videoPlayer.loopPointReached += OnVideoFinished;
            // Calculate video length and store in string.
            _currentVideoLengthString = FormatTime(videoPlayer.length);
            UpdateSlider();
            // On slider changed
            videoSlider.onValueChanged.AddListener(SetVideoTime);
            // Volume
            volumeSlider.SetValue(videoPlayer.GetDirectAudioVolume(0), false);
            volumeSlider.RegisterOnChanged(SetVideoVolume);
            volumeButton.RegisterOnClick(OnVolumeButtonClicked);
            // Title & Video
            titleText.text = videoTitle;
            videoPlayer.clip = video;
        }

        private void Update()
        {
            if (_overlayActive && hideOverlayIfInactive && !_blockHideOverlayCounter)
            {
                _overlayHideCounter += Time.deltaTime;
                if (_overlayHideCounter > hideOverlayAfter)
                {
                    HideOverlay();
                }
            }
            if (Math.Abs(_previousTime - videoPlayer.time) < 0.0001f) return;
            UpdateSlider();
            _previousTime = videoPlayer.time;
        }

        /// <summary>
        /// On video clicked.
        /// </summary>
        public void OnVideoClicked()
        {
            // Check if transition is already in progress.
            if (_inTransition) return;
            
            if (_overlayActive)
            {
                HideOverlay();
            }
            else
            {
                ShowOverlay();
            }
        }

        private void ShowOverlay()
        {
            overlay.gameObject.SetActive(true);
            _inTransition = true;
            StartCoroutine(overlay.TransitionAlpha(1, 2, () =>
            {
                overlay.interactable = true;
                _overlayActive = true;
                _inTransition = false;
            }));
        }

        private void HideOverlay()
        {
            overlay.interactable = false;
            _inTransition = true;
            _overlayHideCounter = 0;
            volumeSlider.gameObject.SetActive(false);
            _blockHideOverlayCounter = false;
            StartCoroutine(overlay.TransitionAlpha(0, 2, () =>
            {
                _overlayActive = false;
                _inTransition = false;
                overlay.gameObject.SetActive(false);
            }));
        }

        private void OnPlayButtonClick()
        {
            _overlayHideCounter = 0;
            if (_playing)
            {
                PauseVideo();
            }
            else
            {
                PlayVideo();
            }
        }

        private void PlayVideo()
        {
            videoPlayer.Play();
            playButton.iconImage.sprite = pauseSprite;
            _playing = true;
        }

        private void PauseVideo()
        {
            videoPlayer.Pause();
            playButton.iconImage.sprite = playSprite;
            _playing = false;
        }

        private void OnVideoFinished(VideoPlayer player)
        {
            _playing = false;
            playButton.iconImage.sprite = playSprite;
        }

        private void UpdateSlider()
        {
            videoSlider.SetValueWithoutNotify((float)(videoPlayer.time / videoPlayer.length));
            timeText.text = FormatTime(videoPlayer.time) + " / " + _currentVideoLengthString;
        }

        private string FormatTime(double seconds)
        {
            int hours = (int)(seconds / 3600);
            int minutes = (int)((seconds % 3600) / 60);
            int remainingSeconds = (int)(seconds % 60);

            string result = hours > 0 
                ? $"{hours:D2}:{minutes:D2}:{remainingSeconds:D2}" 
                : $"{minutes:D2}:{remainingSeconds:D2}";

            return result;
        }

        private void SetVideoTime(float sliderValue)
        {
            videoPlayer.time = sliderValue * videoPlayer.length;
            _overlayHideCounter = 0;
        }

        private void SetVideoVolume(float volume)
        {
            videoPlayer.SetDirectAudioVolume(0, volume);
        }

        private void OnVolumeButtonClicked()
        {
            var show = !volumeSlider.gameObject.activeInHierarchy;
            volumeSlider.gameObject.SetActive(show);
            _blockHideOverlayCounter = show;
        }

        // Record if video is playing pre slider drag so we can play video after choosing the new time.
        public void BeginDragSlider()
        {
            _preSliderVideoIsPlaying = _playing;
            PauseVideo();
        }

        // If video was playing pre drag, play after drag is over.
        public void EndDragSlider()
        {
            if (_preSliderVideoIsPlaying) PlayVideo();
        }

        /// <summary>
        /// Set the video for this video player and specify the starting time or leave startingTime to start at beginning.
        /// </summary>
        /// <param name="videoClip"></param>
        /// <param name="title"></param>
        /// <param name="startingTime"></param>
        public void SetVideo(VideoClip videoClip, string title = "", float startingTime = 0)
        {
            videoPlayer.clip = videoClip;
            videoPlayer.time = startingTime;
            titleText.text = title;
        }

        protected override void SetColors(ColorTheme theme)
        {
            var color = theme.primaryColor;
            color.a = 139f / 255f;
            playButtonBackground.color = volumeButtonBackground.color = color;
            handleImage.color = theme.fourthColor;
        }
    }
}

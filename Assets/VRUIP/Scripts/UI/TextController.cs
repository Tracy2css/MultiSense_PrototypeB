using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace VRUIP
{
    public class TextController : A_ColorController
    {
        // Settings.
        [SerializeField] [Tooltip("If enabled, text will be empty on start and will be waiting for the function call to start typing.")]
        private bool enableTyping = false;
        [SerializeField] private bool typingOnAwake = false;
        [SerializeField] private bool pauseLongerOnPeriod = false;
        [SerializeField] private float delayBetweenLetters = 0.04f;
        
        // Events.
        [SerializeField] private UnityEvent onTypingFinished = new();
        
        // Components.
        [SerializeField] private TextMeshProUGUI text;

        private string _currentText = "";
        private bool _isTyping;
        private bool _typingFinished;
        private string _fullText;
        private Coroutine _typingCoroutine;

        public string Text
        {
            get => text.text;
            set => text.text = value;
        }

        protected override void Start()
        {
            base.Start();
            if (enableTyping)
            {
                _fullText = text.text;
                text.text = "";
            }
            if (typingOnAwake)
            {
                StartTyping();
            }
        }

        /// <summary>
        /// Start typing this text.
        /// </summary>
        public void StartTyping()
        {
            // Already typed this text.
            if (_typingFinished) return;
            
            _typingCoroutine = StartCoroutine(TypeText());
        }

        private IEnumerator TypeText()
        {
            if (_isTyping) yield break;
            
            _isTyping = true;

            for (int i = 0; i < _fullText.Length; i++)
            {
                var currentChar = _fullText[i];
                _currentText += currentChar;
                // Add cursor.
                text.text = _currentText + "|";
                // If period and waiting longer enabled, wait longer.
                if (pauseLongerOnPeriod && currentChar == '.') yield return new WaitForSeconds(delayBetweenLetters * 6);
                
                yield return new WaitForSeconds(delayBetweenLetters);
            }
            // Remove cursor.
            text.text = _currentText;

            _isTyping = false;
            _typingFinished = true;
            onTypingFinished?.Invoke();
        }

        private void OnDisable()
        {
            if (_typingCoroutine == null) return;
            StopCoroutine(_typingCoroutine);
            onTypingFinished?.Invoke();
            text.text = _fullText;
            _isTyping = false;
            _typingFinished = true;
        }

        protected override void SetColors(ColorTheme theme)
        {
            text.color = theme.secondaryColor;
        }
    }
}

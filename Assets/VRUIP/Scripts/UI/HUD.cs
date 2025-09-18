using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class HUD : MonoBehaviour
    {
        [Header("HUD Options")]
        [SerializeField] private HUDOptions type;
        [SerializeField] private Transform hand;

        [Header("Circle")]
        [SerializeField] private GameObject circleSection;
        [SerializeField] private Image healthCircle;
        [SerializeField] private Image staminaCircle;
        [SerializeField] private Image xpCircle;
        [SerializeField] private TextMeshProUGUI healthCircleText;
        [SerializeField] private TextMeshProUGUI staminaCircleText;
        [SerializeField] private TextMeshProUGUI xpCircleText;
    
        [Header("Bar")]
        [SerializeField] private GameObject barSection;
        [SerializeField] private Image healthBar;
        [SerializeField] private Image staminaBar;
        [SerializeField] private Image xpBar;
        [SerializeField] private TextMeshProUGUI healthBarText;
        [SerializeField] private TextMeshProUGUI staminaBarText;
        [SerializeField] private TextMeshProUGUI xpBarText;
    
        private int _health;
        private int _maxHealth;
        private int _stamina;
        private int _maxStamina;
        private int _xp;
        private int _maxXp;
        private int _level;

        private Image _healthVisual;
        private Image _staminaVisual;
        private Image _xpVisual;
        private TextMeshProUGUI _healthText;
        private TextMeshProUGUI _staminaText;
        private TextMeshProUGUI _xpText;

        private void Start()
        {
            // Set section
            circleSection.SetActive(type == HUDOptions.Circle);
            barSection.SetActive(type == HUDOptions.Bar);
            // Set visuals
            _healthVisual = type == HUDOptions.Bar ? healthBar : healthCircle;
            _staminaVisual = type == HUDOptions.Bar ? staminaBar : staminaCircle;
            _xpVisual = type == HUDOptions.Bar ? xpBar : xpCircle;
            // Set text
            _healthText = type == HUDOptions.Bar ? healthBarText : healthCircleText;
            _staminaText = type == HUDOptions.Bar ? staminaBarText : staminaCircleText;
            _xpText = type == HUDOptions.Bar ? xpBarText : xpCircleText;
            // Set parent and rotation
            if (VRUIPManager.instance.IsVR)
            {
                transform.AddComponent<UIFX>().alwaysFacePlayer = true;
            }

            // SET FOR EXAMPLE
            _health = 90;
            _stamina = 50;
            _xp = 750;
            
            _maxHealth = 100;
            _maxStamina = 100;
            _maxXp = 1000;
            
            UpdateUI(_health, _maxHealth, _healthVisual, _healthText);
            UpdateUI(_stamina, _maxStamina, _staminaVisual, _staminaText);
            UpdateUI(_xp, _maxXp, _xpVisual, _xpText);
        }

        /// <summary>
        /// Set hud type, 0 is circle, anything else is bar.
        /// </summary>
        /// <param name="hudType"></param>
        public void SetupHUD(int hudType)
        {
            type = hudType == 0 ? HUDOptions.Circle : HUDOptions.Bar;
            // Set section
            circleSection.SetActive(type == HUDOptions.Circle);
            barSection.SetActive(type == HUDOptions.Bar);
            // Set visuals
            _healthVisual = type == HUDOptions.Bar ? healthBar : healthCircle;
            _staminaVisual = type == HUDOptions.Bar ? staminaBar : staminaCircle;
            _xpVisual = type == HUDOptions.Bar ? xpBar : xpCircle;
            // Set text
            _healthText = type == HUDOptions.Bar ? healthBarText : healthCircleText;
            _staminaText = type == HUDOptions.Bar ? staminaBarText : staminaCircleText;
            _xpText = type == HUDOptions.Bar ? xpBarText : xpCircleText;
            
            UpdateUI(_health, _maxHealth, _healthVisual, _healthText);
            UpdateUI(_stamina, _maxStamina, _staminaVisual, _staminaText);
            UpdateUI(_xp, _maxXp, _xpVisual, _xpText);
        }

#if (XR_ITK || OCULUS_INTEGRATION || META_SDK)
        private void Update()
        {
            KeepStable();
        }
#endif

        public void UpdateHealth(int amount)
        {
            _health += amount;
            UpdateUI(_health, _maxHealth, _healthVisual, _healthText);
        }

        public void UpdateStamina(int amount)
        {
            _stamina += amount;
            UpdateUI(_stamina, _maxStamina, _staminaVisual, _staminaText);
        }
    
        public void UpdateXp(int amount)
        {
            _xp += amount;
            UpdateUI(_xp, _maxXp, _xpVisual, _xpText);
        }
    
        public void UpdateMaxHealth(int newMax)
        {
            _maxHealth = newMax;
            UpdateUI(_health, _maxHealth, _healthVisual, _healthText);
        }

        public void UpdateMaxStamina(int newMax)
        {
            _maxStamina = newMax;
            UpdateUI(_stamina, _maxStamina, _staminaVisual, _staminaText);
        }
    
        public void UpdateMaxXp(int newMax)
        {
            _maxXp = newMax;
            UpdateUI(_xp, _maxXp, _xpVisual, _xpText);
        }
    
        public void UpdateLevel(int level)
        {
            _level = level;
        }

        private void UpdateUI(int current, int max, Image visual, TextMeshProUGUI text)
        {
            if (visual == null) return;
            var percentageFill = current / (float)max;
            visual.fillAmount = percentageFill;
            text.text = current.ToString();
        }

        private enum HUDOptions
        {
            Circle,
            Bar
        }

        private void KeepStable()
        {
            transform.position = Vector3.Lerp(transform.position, hand.position + new Vector3(0, 0.3f, 0), Time.deltaTime * 10);
        }
    }
}

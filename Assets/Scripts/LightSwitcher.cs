using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SwitchController : MonoBehaviour
{
    [Header("Colors On")]
    [SerializeField] private Color backgroundOnColor = Color.green;
    [SerializeField] private Color circleOnColor = Color.white;

    [Header("Colors Off")]
    [SerializeField] private Color backgroundOffColor = Color.gray;
    [SerializeField] private Color circleOffColor = Color.white;

    [Header("Properties")]
    [SerializeField] [Tooltip("Enable text that tells you whether this switch is on or off.")]
    private bool enableStateText;
    [SerializeField] private bool customStateText;
    [SerializeField] private string onStateText = "ON";
    [SerializeField] private string offStateText = "OFF";
    [SerializeField] private AudioClip switchSound;

    [Header("Events")]
    [SerializeField] private UnityEvent onTurnOn;
    [SerializeField] private UnityEvent onTurnOff;

    [Header("Components")]
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image background;
    [SerializeField] private Image circleImage;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Light controlledLight; // The light you want to control

    public bool IsOn => toggle.isOn;

    private void Awake()
    {
        SetupSwitch();
    }

    // Setup the switch initially
    private void SetupSwitch()
    {
        // Update the visuals and light state based on toggle state
        UpdateVisuals(toggle.isOn);
        
        // Add listener for when the toggle value changes
        toggle.onValueChanged.AddListener(HandleToggleChanged);

        // Set the initial state of the light to match the toggle
        if (controlledLight != null)
        {
            controlledLight.enabled = toggle.isOn;
        }

        // Set the audio clip
        if (audioSource != null)
        {
            audioSource.clip = switchSound;
        }
    }

    // Handle toggle value changed
    private void HandleToggleChanged(bool on)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
        
        // Update visuals and light based on new state
        UpdateVisuals(on);

        // Control the light based on toggle state
        if (controlledLight != null)
        {
            controlledLight.enabled = on; // Do not deactivate the entire object, just the Light component
        }
    }

    // Update the visuals of the toggle switch
    private void UpdateVisuals(bool isOn)
    {
        circleImage.rectTransform.anchoredPosition = isOn ? new Vector3(24f, 0, 0) : new Vector3(-24f, 0, 0); // Move circle
        background.color = isOn ? backgroundOnColor : backgroundOffColor;
        circleImage.color = isOn ? circleOnColor : circleOffColor;
        stateText.color = isOn ? backgroundOnColor : backgroundOffColor;
        stateText.text = customStateText ? (isOn ? onStateText : offStateText) : (isOn ? "ON" : "OFF");
        
        if (isOn)
        {
            onTurnOn.Invoke();
        }
        else
        {
            onTurnOff.Invoke();
        }
    }
}

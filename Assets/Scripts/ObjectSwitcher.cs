using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjectSwitchController : MonoBehaviour
{
    [Header("Colors On")]
    [SerializeField] private Color backgroundOnColor = Color.green;
    [SerializeField] private Color circleOnColor = Color.white;
    [SerializeField] private Color textOnColor = Color.white; // Add separate color for text when on

    [Header("Colors Off")]
    [SerializeField] private Color backgroundOffColor = Color.gray;
    [SerializeField] private Color circleOffColor = Color.white;
    [SerializeField] private Color textOffColor = Color.black; // Add separate color for text when off

    [Header("State Text Colors")]
    [SerializeField] private Color customTextOnColor = Color.white; // Custom color for text when on
    [SerializeField] private Color customTextOffColor = Color.black; // Custom color for text when off

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
    [SerializeField] private GameObject controlledObject; // The object you want to control

    public bool IsOn => toggle.isOn;

    private void Awake()
    {
        SetupSwitch();
    }

    // Setup the switch initially
    private void SetupSwitch()
    {
        // Update the visuals and object active state based on toggle state
        UpdateVisuals(toggle.isOn);

        // Add listener for when the toggle value changes
        toggle.onValueChanged.AddListener(HandleToggleChanged);

        // Set the initial active state of the controlled object to match the toggle
        if (controlledObject != null)
        {
            controlledObject.SetActive(toggle.isOn);
        }

        // Set the audio clip
        if (audioSource != null)
        {
            audioSource.clip = switchSound;
        }

        // Set initial state text
        if (enableStateText)
        {
            stateText.gameObject.SetActive(true);
            stateText.color = customTextOffColor;
            stateText.text = customStateText ? offStateText : "OFF";
        }
        else
        {
            stateText.gameObject.SetActive(false);
        }
    }

    // Handle toggle value changed
    private void HandleToggleChanged(bool on)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Update visuals and object active state based on new state
        UpdateVisuals(on);

        // Control the object active state based on toggle state
        if (controlledObject != null)
        {
            controlledObject.SetActive(on); // Activate or deactivate the controlled object
        }
    }

    // Update the visuals of the toggle switch
    private void UpdateVisuals(bool isOn)
    {
        circleImage.rectTransform.anchoredPosition = isOn ? new Vector3(24f, 0, 0) : new Vector3(-24f, 0, 0); // Move circle
        background.color = isOn ? backgroundOnColor : backgroundOffColor;
        circleImage.color = isOn ? circleOnColor : circleOffColor;

        // Update state text visibility and content
        if (enableStateText)
        {
            stateText.gameObject.SetActive(true);
            stateText.color = isOn ? customTextOnColor : customTextOffColor; // Use custom color for text when on or off
            stateText.text = customStateText ? (isOn ? onStateText : offStateText) : (isOn ? "ON" : "OFF");
        }
        else
        {
            stateText.gameObject.SetActive(false);
        }

        // Trigger events for turning on or off
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

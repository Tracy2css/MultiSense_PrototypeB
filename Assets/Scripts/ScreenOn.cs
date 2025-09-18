using UnityEngine;
using UnityEngine.UI;

public class MaterialSwitcher : MonoBehaviour
{
    public Renderer targetRenderer; // The object that needs to switch materials
    public Material material1; // Initial material
    public Material material2; // Material to switch to
    public Button switchButton; // Button to trigger the switch
    public Slider emissionSlider; // Slider to control emission brightness
    public float maxEmission = 5.0f; // Maximum emission intensity
    private Material activeMaterial;
    public GameObject indicatorObject; // Indicator object to be shown with the slider

    // --- New variables for volume ---
    public Button volumeButton;         // New button for controlling volume
    public Slider volumeSlider;         // New slider for volume control
    public AudioSource targetAudio;     // AudioSource to control volume
    public GameObject indicatorObjectA; // Indicator object to be shown with the slider


    // --- Confirm button ---
    public Button confirmButton;        // Confirm button

    void Start()
    {
        // Ensure the initial material
        if (targetRenderer != null && material1 != null)
        {
            targetRenderer.material = material1;
        }
        
        // Ensure the state of the button and slider
        if (switchButton != null)
        {
            switchButton.onClick.AddListener(SwitchMaterial);
        }
        
        if (emissionSlider != null)
        {
            emissionSlider.gameObject.SetActive(false); // Initially hide the slider
            emissionSlider.onValueChanged.AddListener(AdjustEmission);
        }

        if (indicatorObject != null)
        {
            indicatorObject.SetActive(false); // Initially hide the indicator object
            indicatorObjectA.SetActive(false); // Initially hide the indicator object

        }

        // --- New: Set up volume button and slider ---
        if (volumeButton != null)
        {
            volumeButton.onClick.AddListener(ShowVolumeSlider);
        }

        if (volumeSlider != null)
        {
            volumeSlider.gameObject.SetActive(false); // Initially hide
            volumeSlider.onValueChanged.AddListener(AdjustVolume);
        }

        // --- Confirm button setup ---
        if (confirmButton != null)
        {
            confirmButton.gameObject.SetActive(false);
            confirmButton.onClick.AddListener(ConfirmSettings);
        }
    }

    void SwitchMaterial()
    {
        if (targetRenderer != null && material1 != null && material2 != null)
        {
            targetRenderer.material = material2;
            activeMaterial = targetRenderer.material;
            
            // Enable the emission feature for material2
            activeMaterial.EnableKeyword("_EMISSION");
            activeMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            
            // Hide the button
            switchButton.gameObject.SetActive(false);
            
            // Show the slider
            if (emissionSlider != null)
            {
                emissionSlider.gameObject.SetActive(true);
                emissionSlider.value = 0f; // Default initial brightness
            }

            // Show the indicator
            if (indicatorObject != null)
            {
                indicatorObject.SetActive(true);
            }

            // Hide volume button if visible
            if (volumeButton != null)
                volumeButton.gameObject.SetActive(false);

            // Show confirm button
            if (confirmButton != null)
                confirmButton.gameObject.SetActive(true);
        }
    }

    void AdjustEmission(float value)
    {
        if (activeMaterial != null)
        {
            Color emissionColor = Color.white * (value * maxEmission);
            activeMaterial.SetColor("_EmissionColor", emissionColor);
        }
    }

    // --- New: Show the volume slider and hide others ---
    void ShowVolumeSlider()
    {
        // Show volume slider
        if (volumeSlider != null)
        {
            volumeSlider.gameObject.SetActive(true);
            volumeSlider.value = (targetAudio != null) ? targetAudio.volume : 1.0f; // Default to 1.0 if not set
        }

        // Hide main buttons and emission controls
        if (volumeButton != null)
            volumeButton.gameObject.SetActive(false);
                        // Show the indicator
        if (indicatorObjectA != null)
        {
            indicatorObjectA.SetActive(true);
        }
        if (switchButton != null)
            switchButton.gameObject.SetActive(false);
        if (emissionSlider != null)
            emissionSlider.gameObject.SetActive(false);
        if (indicatorObject != null)
            indicatorObject.SetActive(false);

        // Show confirm button
        if (confirmButton != null)
            confirmButton.gameObject.SetActive(true);
    }

    // --- New: Adjust Audio Volume ---
    void AdjustVolume(float value)
    {
        if (targetAudio != null)
        {
            targetAudio.volume = value;
        }
    }

    // --- Confirm button logic: Hide controls, show main buttons ---
    void ConfirmSettings()
    {
        // Hide both sliders and confirm button
        if (emissionSlider != null)
            emissionSlider.gameObject.SetActive(false);
        if (volumeSlider != null)
            volumeSlider.gameObject.SetActive(false);
        if (confirmButton != null)
            confirmButton.gameObject.SetActive(false);

        // Hide indicator object
        if (indicatorObject != null)
            indicatorObject.SetActive(false);
        if (indicatorObjectA != null)
            indicatorObjectA.SetActive(false);

        // Show both main buttons
        if (switchButton != null)
            switchButton.gameObject.SetActive(true);
        if (volumeButton != null)
            volumeButton.gameObject.SetActive(true);
    }
}

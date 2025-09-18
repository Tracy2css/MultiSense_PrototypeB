using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LeftHandPanelController : MonoBehaviour
{
    // Reference to the left hand model (VR controller)
    public GameObject leftHandModel;

    // Reference to the left hand ray interactor (ray used for interactions)
    public GameObject leftRayInteractor;

    // Reference to the control panel that will appear on the left hand
    public GameObject controlPanel;

    // Reference to the control panel that will appear on the left hand
    public GameObject handlePanel;
    // The transform of the player's head or camera (for setting the event camera)
    public Camera eventCamera;

    // Reference to the button that will toggle the panel
    public Button toggleButton;

    // Reference to indicator
    public GameObject indicator;

    // Boolean to track if the control panel is active or not
    private bool isControlPanelActive = false;

    void Start()
    {
        // Ensure the control panel is disabled at the start
        controlPanel.SetActive(false);
        handlePanel.SetActive(false);

        // Add a listener to the button to call ToggleControlPanel when clicked
        toggleButton.onClick.AddListener(ToggleControlPanel);
    }

    // Method to toggle between showing the left hand model or the control panel
    void ToggleControlPanel()
    {
        if (isControlPanelActive)
        {
            // Hide the panel, show the left hand model and ray
            controlPanel.SetActive(false);
            handlePanel.SetActive(false);
            leftHandModel.SetActive(true);
            indicator.SetActive(true);
            leftRayInteractor.SetActive(true);
            isControlPanelActive = false;
        }
        else
        {
            // Show the panel, hide the left hand model and ray
            controlPanel.SetActive(true);
            handlePanel.SetActive(true);
            leftHandModel.SetActive(false);
            leftRayInteractor.SetActive(false);
            indicator.SetActive(false);
            

            isControlPanelActive = true;
        }
    }

}

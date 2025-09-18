using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DysonController : MonoBehaviour
{
    public Button SpeedUpButton;            // Button to increase fan speed
    public Button SpeedDownButton;          // Button to decrease fan speed
    public Toggle FanToggle;                // Fan toggle

    private bool isFanOn = false;
    private float speedStep = 0.1f;         // Step size for speed adjustment
    private float currentSpeed = 0.0f;      // Current fan speed

    void Start()
    {
        // Initialize fan state
        currentSpeed = 0.0f;

        // Bind UI controls
        FanToggle.onValueChanged.AddListener(ToggleFanMode);
        SpeedUpButton.onClick.AddListener(IncreaseSpeed);
        SpeedDownButton.onClick.AddListener(DecreaseSpeed);
    }

    public void ToggleFanMode(bool isOn)
    {
        isFanOn = isOn;

        if (isFanOn)
        {
            // Start the fan
            currentSpeed = 0.1f; // Initial speed when fan is turned on
        }
        else
        {
            // Stop the fan
            currentSpeed = 0.0f;
        }
    }

    public void IncreaseSpeed()
    {
        // Increase fan speed
        currentSpeed = Mathf.Clamp(currentSpeed + speedStep, 0, 1);
    }

    public void DecreaseSpeed()
    {
        // Decrease fan speed
        currentSpeed = Mathf.Clamp(currentSpeed - speedStep, 0, 1);
    }
}

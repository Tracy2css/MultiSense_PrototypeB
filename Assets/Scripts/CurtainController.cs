using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurtainController : MonoBehaviour
{
    public Transform curtain; // Reference to the curtain object
    public Button upButton; // Button to move the curtain up (Raise)
    public Button downButton; // Button to move the curtain down (Lower)
    public float maxHeight = 1.0f; // Maximum scale value for the curtain height (Fully down)
    public float minHeight = 0.1f; // Minimum scale value for the curtain height (Fully up)
    public float changeStep = 0.05f; // Amount by which the height changes per button press
    public float animationSpeed = 2.0f; // Speed of the height change animation

    public AudioSource curtainAudioSource; // Audio source to play movement sound
    public AudioClip moveSound; // Sound to play when curtain starts moving

    private float targetScaleY; // Target Y scale value
    private bool isAnimating = false; // Whether the curtain is animating
    private bool hasPlayedSound = false; // Flag to avoid repeating the sound

    private void Start()
    {
        // Initialize the target scale with the current scale
        targetScaleY = curtain.localScale.y;

        // Add listeners to the buttons
        upButton.onClick.AddListener(MoveUp);
        downButton.onClick.AddListener(MoveDown);
    }

    private void Update()
    {
        // Smoothly transition the curtain's scale towards the target value
        if (Mathf.Abs(curtain.localScale.y - targetScaleY) > 0.01f)
        {
            isAnimating = true;

            // Play sound once at the beginning of animation
            if (!hasPlayedSound && curtainAudioSource != null && moveSound != null)
            {
                curtainAudioSource.PlayOneShot(moveSound);
                hasPlayedSound = true;
            }

            // Lerp for smooth animation
            float newScaleY = Mathf.Lerp(curtain.localScale.y, targetScaleY, 0.1f); 
            curtain.localScale = new Vector3(curtain.localScale.x, newScaleY, curtain.localScale.z);
        }
        else
        {
            // Animation finished
            if (isAnimating)
            {
                isAnimating = false;
                hasPlayedSound = false; // Reset flag for the next movement
            }
        }
    }

    // Method to check animation state
    public bool IsAnimating()
    {
        return isAnimating;
    }

    // Method to move the curtain up 
    public void MoveUp()
    {
        if (targetScaleY > minHeight)
        {
            targetScaleY -= changeStep; 
            targetScaleY = Mathf.Clamp(targetScaleY, minHeight, maxHeight);
        }
    }

    // Method to move the curtain down 
    public void MoveDown()
    {
        if (targetScaleY < maxHeight)
        {
            targetScaleY += changeStep; 
            targetScaleY = Mathf.Clamp(targetScaleY, minHeight, maxHeight);
        }
    }
}

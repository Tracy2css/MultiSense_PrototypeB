using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidanceSwitcher : MonoBehaviour
{
    public Material[] guidanceMaterials;  // Array to hold different materials (guidance steps)
    private Renderer objectRenderer;  // Renderer component of the object
    private int currentIndex = 0;  // Index to track the current material (guidance step)

    public Button nextButton;  // Next button
    public Button forwardButton;  // Forward button
    public Button letsDoButton;  // "Let's Do" button

    void Start()
    {
        // Get the Renderer component from the current object
        objectRenderer = GetComponent<Renderer>();
        
        // Initialize with the first material (guidance step)
        if (guidanceMaterials.Length > 0)
        {
            objectRenderer.material = guidanceMaterials[currentIndex];
        }

        // Hide the "Let's Do" button initially
        letsDoButton.gameObject.SetActive(false);
    }

    // This method will be called when the "Next" button is clicked
    public void OnNextButtonClick()
    {
        if (guidanceMaterials.Length > 0)
        {
            // Increment the index and loop back if it exceeds the array length
            currentIndex = (currentIndex + 1) % guidanceMaterials.Length;
            
            // Update the material to the next guidance step
            objectRenderer.material = guidanceMaterials[currentIndex];

            // Check if the current index is 2 (Material Index 03), then handle button visibility
            if (currentIndex == 3)
            {
                // Show "Let's Do" button and hide "Next" and "Forward" buttons
                letsDoButton.gameObject.SetActive(true);
                nextButton.gameObject.SetActive(false);
                forwardButton.gameObject.SetActive(false);
            }
            else
            {
                // Hide "Let's Do" button and show "Next" and "Forward" buttons
                letsDoButton.gameObject.SetActive(false);
                nextButton.gameObject.SetActive(true);
                forwardButton.gameObject.SetActive(true);
            }
        }
    }

    // This method will be called when the "Forward" button is clicked
    public void OnForwardButtonClick()
    {
        if (guidanceMaterials.Length > 0)
        {
            // Decrement the index and loop back if it goes below 0
            currentIndex = (currentIndex - 1 + guidanceMaterials.Length) % guidanceMaterials.Length;
            
            // Update the material to the previous guidance step
            objectRenderer.material = guidanceMaterials[currentIndex];

            // Check if the current index is 2 (Material Index 03), then handle button visibility
            if (currentIndex == 3)
            {
                // Show "Let's Do" button and hide "Next" and "Forward" buttons
                letsDoButton.gameObject.SetActive(true);
                nextButton.gameObject.SetActive(false);
                forwardButton.gameObject.SetActive(false);
            }
            else
            {
                // Hide "Let's Do" button and show "Next" and "Forward" buttons
                letsDoButton.gameObject.SetActive(false);
                nextButton.gameObject.SetActive(true);
                forwardButton.gameObject.SetActive(true);
            }
        }
    }
}
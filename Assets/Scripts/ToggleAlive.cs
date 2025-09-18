using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleContent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject content; // Object to toggle visibility on short press and deactivate on long press
    public GameObject objectA; // Object A whose material will be changed
    public Material materialB; // Target material to switch to on long press
    public float longPressDuration = 1.0f; // Duration required for a long press (in seconds)
    public Image progressBarImage; // UI Image for the circular progress bar
    public GameObject longPressObject; // New object to show after long press release

    private bool isPointerDown = false; // Tracks if the button is being pressed
    private float pointerDownTimer = 0f; // Tracks how long the button has been pressed
    private Renderer objectARenderer; // Renderer component of object A
    private bool hasProcessedLongPress = false; // Tracks if long press duration has been reached

    // Initialize the renderer, progress bar, and long press object
    void Start()
    {
        // Get the Renderer component of object A
        if (objectA != null)
        {
            objectARenderer = objectA.GetComponent<Renderer>();
        }

        // Ensure progress bar is hidden initially
        if (progressBarImage != null)
        {
            progressBarImage.gameObject.SetActive(false);
            progressBarImage.fillAmount = 0f;
        }

        // Ensure long press object is hidden initially
        if (longPressObject != null)
        {
            longPressObject.SetActive(false);
        }
    }

    // Called when the button is pressed down
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isPointerDown) // Prevent multiple coroutines from starting
        {
            isPointerDown = true;
            pointerDownTimer = 0f;
            hasProcessedLongPress = false;

            // Show and reset progress bar
            if (progressBarImage != null)
            {
                progressBarImage.gameObject.SetActive(true);
                progressBarImage.fillAmount = 0f;
            }

            StartCoroutine(LongPressCoroutine());
        }
    }

    // Called when the button is released
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPointerDown) // Ensure the button was pressed
        {
            // Handle long press actions if duration was reached
            if (hasProcessedLongPress)
            {
                // Switch material
                if (objectARenderer != null && materialB != null)
                {
                    objectARenderer.material = materialB;
                }
                // Deactivate content
                if (content != null)
                {
                    content.SetActive(false);
                }
                // Show long press object
                if (longPressObject != null)
                {
                    longPressObject.SetActive(true);
                }
                // Deactivate the button
                gameObject.SetActive(false);
            }
            // Handle short press if long press duration wasn't reached
            else if (pointerDownTimer < longPressDuration)
            {
                ToggleVisibility();
            }

            // Hide and reset progress bar
            if (progressBarImage != null)
            {
                progressBarImage.gameObject.SetActive(false);
                progressBarImage.fillAmount = 0f;
            }

            // Reset state
            isPointerDown = false;
            pointerDownTimer = 0f;
            hasProcessedLongPress = false;
        }
    }

    // Coroutine to monitor long press duration and update progress bar
    private IEnumerator LongPressCoroutine()
    {
        while (isPointerDown && pointerDownTimer < longPressDuration)
        {
            pointerDownTimer += Time.deltaTime;
            // Update progress bar fill amount
            if (progressBarImage != null)
            {
                progressBarImage.fillAmount = Mathf.Clamp01(pointerDownTimer / longPressDuration);
            }
            yield return null;
        }

        // Mark long press as completed if duration was reached
        if (isPointerDown && !hasProcessedLongPress)
        {
            hasProcessedLongPress = true;
        }
    }

    // Toggles the visibility of the content object (used for short press)
    public void ToggleVisibility()
    {
        if (content != null)
        {
            content.SetActive(!content.activeSelf);
        }
    }
}
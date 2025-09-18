using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TickIconController : MonoBehaviour
{
    [Header("Buttons and Icons")]
    public Button[] buttons;           // Array of buttons
    public GameObject[] icons;         // Array of thumb icons (one per button)
    public int defaultActiveIconIndex = -1; // Index of the default active icon

    private void Start()
    {
        // Add listeners to each button
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Local copy of the index for the listener
            buttons[i].onClick.AddListener(() => OnButtonClicked(index));
        }

        // Initialize all icons to inactive
        DeactivateAllIcons();

        // Activate the default icon if a valid index is set
        if (defaultActiveIconIndex >= 0 && defaultActiveIconIndex < icons.Length)
        {
            icons[defaultActiveIconIndex].SetActive(true);
        }
    }

    private void OnButtonClicked(int index)
    {
        // Deactivate all icons
        DeactivateAllIcons();

        // Activate the icon for the clicked button
        icons[index].SetActive(true);
    }

    private void DeactivateAllIcons()
    {
        foreach (GameObject icon in icons)
        {
            icon.SetActive(false);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonStartController : MonoBehaviour
{
    [Header("Material Switch")]
    public MeshRenderer targetRenderer;
    public Material newMaterial;

    [Header("Activate on Click")]
    public List<GameObject> activateObjects;

    [Header("Deactivate on Click")]
    public List<GameObject> deactivateObjects;

    [Header("Hide This Button")]
    public bool hideThisButton = true;

    void Start()
    {

        // Set activate objects to false at start
        foreach (var obj in activateObjects)
        {
            if (obj != null) 
            {
                obj.SetActive(false);
            }
        }

        var thisButton = GetComponent<Button>();
        if (thisButton != null)
        {
            thisButton.onClick.AddListener(OnButtonClick);
        }
    }

    public void OnButtonClick()
    {
        if (targetRenderer != null && newMaterial != null)
        {
            targetRenderer.material = newMaterial;
        }

        foreach (var obj in activateObjects)
        {
            if (obj != null) obj.SetActive(true);
        }

        foreach (var obj in deactivateObjects)
        {
            if (obj != null) obj.SetActive(false);
        }

        if (hideThisButton)
        {
            gameObject.SetActive(false);
        }
    }
}

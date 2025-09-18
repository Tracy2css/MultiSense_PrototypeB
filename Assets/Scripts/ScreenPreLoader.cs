using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenPreLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneA = "SceneA"; // Name of the initial scene
    public string sceneB = "SceneB"; // Name of the scene to preload
    public string sceneC = "SceneC"; // Name of the scene to preload

    [Header("UI Settings")]
    public Slider progressBar; // Progress bar UI
    public GameObject progressBarContainer; // Parent container for progress bar

    [Header("Destroy Settings")]
    public List<GameObject> objectsToDestroyInSceneA; // List of objects to destroy in sceneA

    private AsyncOperation preloadOperationB; // Preload operation for sceneB
    private AsyncOperation preloadOperationC; // Preload operation for sceneC

    private void Start()
    {
        // Hide progress bar initially
        if (progressBarContainer != null) progressBarContainer.SetActive(false);

        // Start preloading the target scenes
        StartCoroutine(PreloadScenes());
    }

    private IEnumerator PreloadScenes()
    {
        // Preload sceneB and sceneC
        yield return StartCoroutine(PreloadSceneAdditive(sceneB, "B"));
        yield return StartCoroutine(PreloadSceneAdditive(sceneC, "C"));
    }

    private IEnumerator PreloadSceneAdditive(string sceneName, string sceneIdentifier)
    {
        // Use Additive mode to asynchronously load the target scene
        AsyncOperation preloadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        preloadOperation.allowSceneActivation = false; // Prevent automatic activation

        // Wait until the scene is fully loaded
        while (preloadOperation.progress < 0.9f)
        {
            yield return null;
        }

        // Assign the preload operation to the appropriate field
        if (sceneIdentifier == "B")
        {
            preloadOperationB = preloadOperation;
        }
        else if (sceneIdentifier == "C")
        {
            preloadOperationC = preloadOperation;
        }
    }

    public void LoadSceneB()
    {
        StartCoroutine(LoadSceneWithProgress(sceneB));
    }

    public void LoadSceneC()
    {
        StartCoroutine(LoadSceneWithProgress(sceneC));
    }

    private IEnumerator LoadSceneWithProgress(string sceneName)
    {
        // Show progress bar
        if (progressBarContainer != null) progressBarContainer.SetActive(true);

        // Reset fake progress
        float fakeProgress = 0f;
        float fakeSpeed = 0.2f;

        // Start simulating the progress bar
        while (fakeProgress < 1f)
        {
            // Simulate progress bar increment
            fakeProgress += fakeSpeed * Time.deltaTime;
            fakeProgress = Mathf.Clamp01(fakeProgress); // Ensure progress does not exceed 1

            // Update progress bar
            if (progressBar != null) progressBar.value = fakeProgress;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the progress bar reaches 1
        if (progressBar != null) progressBar.value = 1f;

        // Activate the target scene
        AsyncOperation preloadOperation = null;
        if (sceneName == sceneB)
        {
            preloadOperation = preloadOperationB;
        }
        else if (sceneName == sceneC)
        {
            preloadOperation = preloadOperationC;
        }

        if (preloadOperation != null)
        {
            preloadOperation.allowSceneActivation = true;

            // Wait until the target scene is fully activated
            while (!preloadOperation.isDone)
            {
                yield return null;
            }

            // Set the target scene as the active scene
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            // Destroy specified objects in sceneA
            foreach (GameObject obj in objectsToDestroyInSceneA)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }

            Debug.Log($"Scene {sceneName} loaded successfully.");
        }

        // Hide progress bar container
        if (progressBarContainer != null) progressBarContainer.SetActive(false);
    }
}
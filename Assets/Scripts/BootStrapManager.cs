using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BootStrapManager : MonoBehaviour
{
    public static BootStrapManager Instance { get; private set; }

    [Header("Scene Settings")]
    public string sceneA = "SceneA"; // Name of the initial scene

    [Header("UI Settings")]
    public Slider progressBar; // Progress bar UI
    public GameObject progressBarContainer; // Parent container for progress bar

    [Header("XR Settings")]
    public GameObject xrOriginToDestroy; // XR Origin object to destroy in the empty scene

    private AsyncOperation preloadOperationA; // Preload operation for sceneA

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure this instance persists across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Start()
    {
        // Show progress bar initially
        if (progressBarContainer != null) progressBarContainer.SetActive(true);

        // Start preloading the target scene
        StartCoroutine(PreloadSceneA());
    }

    private IEnumerator PreloadSceneA()
    {
        // Preload sceneA
        AsyncOperation preloadOperation = SceneManager.LoadSceneAsync(sceneA, LoadSceneMode.Additive);
        preloadOperation.allowSceneActivation = false; // Prevent automatic activation

        // Update progress bar while the scene is loading
        while (preloadOperation.progress < 0.9f)
        {
            yield return null;
        }

        preloadOperationA = preloadOperation;

        // Load sceneA with progress bar
        yield return StartCoroutine(LoadSceneWithProgress(sceneA));
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
        if (preloadOperationA != null)
        {
            preloadOperationA.allowSceneActivation = true;

            // // Wait until the target scene is fully activated
            // while (!preloadOperationA.isDone)
            // {
            //     yield return null;
            // }

            // // Set the target scene as the active scene
            // SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            // // Destroy the XR Origin in the empty scene to avoid conflicts
            // if (xrOriginToDestroy != null)
            // {
            //     Destroy(xrOriginToDestroy);
            // }

            // Debug.Log($"Scene {sceneName} loaded successfully.");
        }

        // Hide progress bar container
        if (progressBarContainer != null) progressBarContainer.SetActive(false);
    }
}
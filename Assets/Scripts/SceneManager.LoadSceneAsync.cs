using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoaderSlider : MonoBehaviour
{
    public Slider progressBar; // Reference to the UI Slider for progress
    public GameObject progressBarContainer; // Parent container for the progress bar (to show/hide it)
    public float progressBarSpeed = 0.2f; // Speed of the progress bar

    private void Start()
    {
        // Hide the progress bar container initially
        progressBarContainer.SetActive(false);
    }

    // Load a scene normally with Single Mode
    public void LoadScene(string sceneName)
    {
        Debug.Log($"Starting async load for scene: {sceneName}");
        progressBarContainer.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    // Coroutine to load a scene asynchronously
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        float fakeProgress = 0.0f;

        while (fakeProgress < 1.0f)
        {
            if (operation.progress >= 0.9f)
            {
                fakeProgress += Time.deltaTime * progressBarSpeed;
            }
            else
            {
                fakeProgress = Mathf.MoveTowards(fakeProgress, operation.progress, Time.deltaTime * progressBarSpeed);
            }

            progressBar.value = Mathf.Clamp01(fakeProgress);

            if (fakeProgress >= 1.0f)
            {
                Debug.Log("Scene fully loaded, activating...");
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}

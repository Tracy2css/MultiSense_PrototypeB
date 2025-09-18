using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class XROriginScenes : MonoBehaviour
{
    [Header("Scene Names")]
    public string sceneA = "SceneA";
    public string sceneB = "SceneB";
    public string sceneC = "SceneC";

    [Header("XR Origin")]
    public GameObject xrOriginEmpty; // XR Origin for empty scene

    private string currentScene;

    private void Start()
    {
        // Preload all scenes
        StartCoroutine(PreloadScenes());
    }

    private IEnumerator PreloadScenes()
    {
        yield return StartCoroutine(PreloadScene(sceneA));
        yield return StartCoroutine(PreloadScene(sceneB));
        yield return StartCoroutine(PreloadScene(sceneC));

        // Load sceneA initially
        ActivateScene(sceneA);

        // Destroy the empty scene's XR Origin
        if (xrOriginEmpty != null)
        {
            Destroy(xrOriginEmpty);
        }
    }

    private IEnumerator PreloadScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    private void Update()
    {
        // Switch to sceneB when the space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateScene(sceneB);
        }
    }

    private void ActivateScene(string sceneName)
    {
        if (currentScene != null)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            SceneManager.UnloadSceneAsync(currentScene);
        }

        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).completed += (asyncOperation) =>
        {
            currentScene = sceneName;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        };
    }
}
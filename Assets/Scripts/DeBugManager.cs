using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; private set; }

    [Header("Debug Settings")]
    public bool enableDebugLogs = true; // Enable or disable debug logs
    public bool logWarnings = true; // Enable or disable warning logs
    public bool logErrors = true; // Enable or disable error logs

    private void Awake()
    {
        // Ensure a single instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: persist across scenes
    }

    public void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log(message);
        }
    }

    public void LogWarning(string message)
    {
        if (logWarnings)
        {
            Debug.LogWarning(message);
        }
    }

    public void LogError(string message)
    {
        if (logErrors)
        {
            Debug.LogError(message);
        }
    }

    // Optionally: Toggle debugging at runtime
    public void ToggleDebugLogs(bool isEnabled)
    {
        enableDebugLogs = isEnabled;
        Debug.Log($"Debug logs {(isEnabled ? "enabled" : "disabled")}");
    }
}

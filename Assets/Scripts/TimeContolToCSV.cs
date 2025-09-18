using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;
#if TMP_PRESENT
using TMPro;
#endif

[AddComponentMenu("Logging/Button Toggle Logger")]
public class ButtonToggleLogger : MonoBehaviour
{
    // ===== Manual entries (legacy support) =====
    [Serializable]
    public class ButtonEntry {
        public string customName;   // Custom name for this button
        public Button buttonObject; // Reference to Button component
    }
    [Serializable]
    public class ToggleEntry {
        public string customName;   // Custom name for this toggle
        public Toggle toggleObject; // Reference to Toggle component
    }

    // ===== Group scanning entry: auto-detect Buttons/Toggles under a parent =====
    [Serializable]
    public class GroupEntry {
        [Tooltip("Parent object to scan for Buttons and Toggles.")]
        public GameObject root;

        [Tooltip("Optional override name for this group in CSV. If empty, uses root.name.")]
        public string groupName;

        [Tooltip("Include inactive children when scanning.")]
        public bool includeInactive = false;

        [Tooltip("If true, prefix the element name with 'GroupName/'.")]
        public bool prefixGroupToElement = false;
    }

    // ===== Name override entry: assign a custom export name for a specific GameObject =====
    [Serializable]
    public class NameOverrideEntry {
        [Tooltip("Target UI GameObject (Button or Toggle root).")]
        public GameObject target;
        [Tooltip("Custom name to use in CSV for this element.")]
        public string customName;
    }

    // ===== File Settings =====
    [Header("File Settings")]
    [Tooltip("Folder path relative to Application.dataPath, e.g., 'Logs'. Leave empty for root.")]
    public string folderPath = "Logs";
    [Tooltip("Base name of the CSV file (without .csv extension).")]
    public string fileName = "ButtonToggleLog";

    // ===== Auto-Scan & Naming Settings =====
    [Header("Auto-Scan Groups")]
    public List<GroupEntry> groups = new List<GroupEntry>();

    [Header("Name Resolution")]
    [Tooltip("Try to read visible label text (UI.Text / TMP) before falling back to GameObject name.")]
    public bool tryUseLabelText = true;

    [Tooltip("Global name overrides (acts like UINameTag without an extra component).")]
    public List<NameOverrideEntry> nameOverrides = new List<NameOverrideEntry>();

    [Header("CSV Layout")]
    [Tooltip("If true, adds a separate 'Group' column to CSV.")]
    public bool includeGroupColumn = true;

    // ===== Legacy Manual Lists =====
    [Header("Legacy Manual Lists (Optional)")]
    public List<ButtonEntry> trackedButtons = new List<ButtonEntry>();
    public List<ToggleEntry> trackedToggles = new List<ToggleEntry>();

    // ===== Internal state =====
    private readonly List<string[]> interactionLogs = new List<string[]>();
    private readonly List<(Button btn, ActionWrapper cb)> buttonListeners = new List<(Button, ActionWrapper)>();
    private readonly List<(Toggle tgl, BoolActionWrapper cb)> toggleListeners = new List<(Toggle, BoolActionWrapper)>();

    private class ActionWrapper { private readonly Action a; public ActionWrapper(Action x){a=x;} public void Invoke(){a?.Invoke();} }
    private class BoolActionWrapper { private readonly Action<bool> a; public BoolActionWrapper(Action<bool> x){a=x;} public void Invoke(bool v){a?.Invoke(v);} }

    // ===== Unity lifecycle =====
    void Start()
    {
        // Auto-scan groups
        foreach (var grp in groups)
        {
            if (grp == null || grp.root == null) continue;
            string groupName = string.IsNullOrWhiteSpace(grp.groupName) ? grp.root.name : grp.groupName;

            // Buttons
            var buttons = grp.root.GetComponentsInChildren<Button>(grp.includeInactive);
            foreach (var btn in buttons)
            {
                string elementName = ResolveElementName(btn.gameObject, groupName, grp.prefixGroupToElement);
                RegisterButton(btn, elementName, groupName);
            }

            // Toggles
            var toggles = grp.root.GetComponentsInChildren<Toggle>(grp.includeInactive);
            foreach (var tgl in toggles)
            {
                string elementName = ResolveElementName(tgl.gameObject, groupName, grp.prefixGroupToElement);
                RegisterToggle(tgl, elementName, groupName);
            }
        }

        // Manual lists
        foreach (var entry in trackedButtons)
        {
            if (entry?.buttonObject == null) continue;
            string name = !string.IsNullOrWhiteSpace(entry.customName) ? entry.customName
                        : ResolveElementName(entry.buttonObject.gameObject, "", false);
            RegisterButton(entry.buttonObject, name, "Legacy");
        }
        foreach (var entry in trackedToggles)
        {
            if (entry?.toggleObject == null) continue;
            string name = !string.IsNullOrWhiteSpace(entry.customName) ? entry.customName
                        : ResolveElementName(entry.toggleObject.gameObject, "", false);
            RegisterToggle(entry.toggleObject, name, "Legacy");
        }
    }

    void OnApplicationQuit() { SaveLogsToCSV(); }

    void OnDestroy()
    {
        // Clean up listeners
        foreach (var p in buttonListeners)
            if (p.btn) p.btn.onClick.RemoveListener(p.cb.Invoke);
        foreach (var p in toggleListeners)
            if (p.tgl) p.tgl.onValueChanged.RemoveListener(p.cb.Invoke);
        buttonListeners.Clear();
        toggleListeners.Clear();
    }

    // ===== Registration =====
    private void RegisterButton(Button btn, string elementName, string groupName)
    {
        var w = new ActionWrapper(() => LogButtonClick(elementName, groupName));
        btn.onClick.AddListener(w.Invoke);
        buttonListeners.Add((btn, w));
    }
    private void RegisterToggle(Toggle tgl, string elementName, string groupName)
    {
        var w = new BoolActionWrapper(v => LogToggleChange(elementName, groupName, v));
        tgl.onValueChanged.AddListener(w.Invoke);
        toggleListeners.Add((tgl, w));
    }

    // ===== Name resolution =====
    private string ResolveElementName(GameObject go, string groupName, bool prefixGroup)
    {
        // Global override table
        foreach (var ov in nameOverrides)
        {
            if (ov != null && ov.target == go && !string.IsNullOrWhiteSpace(ov.customName))
                return prefixGroup && !string.IsNullOrWhiteSpace(groupName) ? $"{groupName}/{ov.customName}" : ov.customName;
        }

        // Label text
        if (tryUseLabelText)
        {
            string label = TryGetAnyLabel(go);
            if (!string.IsNullOrWhiteSpace(label))
                return prefixGroup && !string.IsNullOrWhiteSpace(groupName) ? $"{groupName}/{label}" : label;

            foreach (Transform c in go.transform)
            {
                label = TryGetAnyLabel(c.gameObject);
                if (!string.IsNullOrWhiteSpace(label))
                    return prefixGroup && !string.IsNullOrWhiteSpace(groupName) ? $"{groupName}/{label}" : label;
            }
        }

        // Fallback: GameObject name
        return prefixGroup && !string.IsNullOrWhiteSpace(groupName) ? $"{groupName}/{go.name}" : go.name;
    }

    private string TryGetAnyLabel(GameObject go)
    {
        var t = go.GetComponent<Text>();
        if (t != null && !string.IsNullOrWhiteSpace(t.text)) return t.text.Trim();
#if TMP_PRESENT
        var tmp = go.GetComponent<TMP_Text>();
        if (tmp != null && !string.IsNullOrWhiteSpace(tmp.text)) return tmp.text.Trim();
#endif
        return null;
    }

    // ===== Logging =====
    private void LogButtonClick(string elementName, string groupName)
    {
        string ts = GetFormattedTime();
        if (includeGroupColumn)
            interactionLogs.Add(new string[] { "Button", groupName, elementName, "Click", "", ts });
        else
            interactionLogs.Add(new string[] { "Button", elementName, "Click", "", ts });
    }

    private void LogToggleChange(string elementName, string groupName, bool isOn)
    {
        string ts = GetFormattedTime();
        string state = isOn ? "ON" : "OFF";
        if (includeGroupColumn)
            interactionLogs.Add(new string[] { "Toggle", groupName, elementName, "StateChange", state, ts });
        else
            interactionLogs.Add(new string[] { "Toggle", elementName, "StateChange", state, ts });
    }

    private string GetFormattedTime()
    {
        DateTime utc = DateTime.UtcNow;
        DateTime local = utc.Add(TimeSpan.FromHours(10)); // Force UTC+10
        return local.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
    }

    // ===== Save CSV =====
    private void SaveLogsToCSV()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string uniqueFileName = $"{fileName}_{timestamp}.csv";

        string directoryPath = "";

        if (!string.IsNullOrEmpty(folderPath))
        {
            string cleanFolderPath = folderPath.Trim('/', '\\');
            directoryPath = Path.Combine(Application.dataPath, cleanFolderPath);
            try
            {
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to create directory {directoryPath}: {e.Message}. Falling back to desktop.");
                directoryPath = "";
            }
        }

        if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
        {
            directoryPath = GetDesktopPath();
            Debug.Log($"Using desktop path: {directoryPath}");
        }

        string fullFilePath = Path.Combine(directoryPath, uniqueFileName);

        try
        {
            using (StreamWriter writer = new StreamWriter(fullFilePath, false))
            {
                if (includeGroupColumn)
                    writer.WriteLine("Type,Group,ElementName,Action,State,UTC+10_Time");
                else
                    writer.WriteLine("Type,ElementName,Action,State,UTC+10_Time");

                foreach (var row in interactionLogs)
                    writer.WriteLine(string.Join(",", row));
            }
            Debug.Log($"CSV saved to: {fullFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save CSV: {e.Message}");
        }
    }

    private string GetDesktopPath()
    {
        try
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (!string.IsNullOrEmpty(desktopPath) && Directory.Exists(desktopPath))
                return desktopPath;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to get desktop path: {e.Message}");
        }

        try
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!string.IsNullOrEmpty(userProfile))
            {
                string desktopPath = Path.Combine(userProfile, "Desktop");
                if (Directory.Exists(desktopPath))
                    return desktopPath;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to get user profile desktop path: {e.Message}");
        }

        Debug.LogWarning("Could not find desktop path, using Application.dataPath as fallback");
        return Application.dataPath;
    }

    [ContextMenu("Save Logs Now")]
    public void SaveLogsManually() => SaveLogsToCSV();
}

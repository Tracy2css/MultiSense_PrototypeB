using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ContactHapticsDebugger : MonoBehaviour
{
    [Header("Controller binding (pick one)")]
    public XRBaseController xrController;            // Drag an XR Controller here if available
    public XRNode xrNode = XRNode.RightHand;         // Fallback to XRNode if controller is not assigned

    [Header("Haptic pulse")]
    [Range(0f,1f)] public float amplitude = 0.7f;
    [Min(0.01f)] public float duration = 0.15f;
    public float minInterval = 0.12f;                // Throttle interval between pulses

    [Header("Filters")]
    public LayerMask detectLayers = ~0;              // Layers that can trigger haptics (default: all)
    public string requiredTag = "";                  // If non-empty, only objects with this tag trigger haptics
    public bool onlyWhenNoXRInteractable = false;    // Avoid firing when XR Interaction Toolkit already handles it

    [Header("Polling (to debug initial overlap)")]
    public bool enablePolling = true;
    public float pollRadiusScale = 1.05f;            // Multiplier of SphereCollider radius
    public float pollInterval = 0.05f;

    private HashSet<Collider> hovering = new HashSet<Collider>();
    private InputDevice device;
    private bool deviceSupportsImpulse;
    private SphereCollider sphere;
    private float nextPollTime;
    private float lastPulseTime = -999f;

    void OnEnable()
    {
        sphere = GetComponent<SphereCollider>();
        var rb = GetComponent<Rigidbody>();
        // Debug logs disabled as requested:
        // Debug.Log($"[ContactHaptics] OnEnable. hasSphere={sphere!=null}, isTrigger={sphere!=null && sphere.isTrigger}, rbKinematic={rb!=null && rb.isKinematic}");
        TryResolveDevice();
    }

    void Update()
    {
        if (enablePolling && Time.time >= nextPollTime)
        {
            nextPollTime = Time.time + pollInterval;
            PollOverlap();
        }
    }

    // ---- Trigger callbacks path ----
    void OnTriggerEnter(Collider other)
    {
        if (!ShouldReact(other)) return;
        if (hovering.Add(other))
        {
            // Debug.Log($"[ContactHaptics] ENTER (Trigger) -> {Desc(other)}");
            SendPulse("Enter");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (hovering.Remove(other))
        {
            // Debug.Log($"[ContactHaptics] EXIT  (Trigger) -> {Desc(other)}");
        }
    }

    // ---- Collision callbacks path (optional fallback) ----
    void OnCollisionEnter(Collision col)
    {
        var other = col.collider;
        if (!ShouldReact(other)) return;
        if (hovering.Add(other))
        {
            // Debug.Log($"[ContactHaptics] ENTER (Collision) -> {Desc(other)}, contacts={col.contactCount}");
            SendPulse("Enter(Collision)");
        }
    }

    void OnCollisionExit(Collision col)
    {
        var other = col.collider;
        if (hovering.Remove(other))
        {
            // Debug.Log($"[ContactHaptics] EXIT  (Collision) -> {Desc(other)}");
        }
    }

    // ---- Polling to catch initial overlaps / missed callbacks ----
    void PollOverlap()
    {
        if (sphere == null) return;

        float r = sphere.radius * Mathf.Max(transform.lossyScale.x, Mathf.Max(transform.lossyScale.y, transform.lossyScale.z)) * pollRadiusScale;
        var hits = Physics.OverlapSphere(transform.TransformPoint(sphere.center), r, detectLayers, QueryTriggerInteraction.Collide);

        // ENTER
        foreach (var h in hits)
        {
            if (!ShouldReact(h)) continue;
            if (hovering.Add(h))
            {
                // Debug.Log($"[ContactHaptics] ENTER (Poll) -> {Desc(h)}");
                SendPulse("Enter(Poll)");
            }
        }

        // EXIT
        var toRemove = new List<Collider>();
        foreach (var c in hovering)
        {
            bool stillHit = false;
            for (int i = 0; i < hits.Length; i++)
                if (hits[i] == c) { stillHit = true; break; }
            if (!stillHit) toRemove.Add(c);
        }
        foreach (var c in toRemove)
        {
            hovering.Remove(c);
            // Debug.Log($"[ContactHaptics] EXIT  (Poll) -> {Desc(c)}");
        }
    }

    // ---- helpers ----
    bool ShouldReact(Collider other)
    {
        // Layer filter
        if ((detectLayers.value & (1 << other.gameObject.layer)) == 0) return false;

        // Tag filter
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return false;

        // Skip if the target (or its parents) contains an XRBaseInteractable and the flag is set
        if (onlyWhenNoXRInteractable && other.GetComponentInParent<XRBaseInteractable>() != null) return false;

        return true;
    }

    string Desc(Collider c)
    {
        string ln = LayerMask.LayerToName(c.gameObject.layer);
        bool hasXR = c.GetComponentInParent<XRBaseInteractable>() != null;
        return $"name={c.name}, layer={ln}, tag={c.tag}, xrInteractable={hasXR}";
    }

    bool TryResolveDevice()
    {
        device = InputDevices.GetDeviceAtXRNode(xrNode);
        bool valid = device.isValid && device.TryGetHapticCapabilities(out var caps) && caps.supportsImpulse && caps.numChannels > 0;
        deviceSupportsImpulse = valid;
        // Debug.Log($"[ContactHaptics] Device valid={device.isValid}, supportsImpulse={deviceSupportsImpulse}");
        return device.isValid;
    }

    void SendPulse(string src)
    {
        // Throttle to avoid spamming haptics
        if (Time.time - lastPulseTime < minInterval) return;
        lastPulseTime = Time.time;

        float a = Mathf.Clamp01(amplitude);
        float d = Mathf.Max(0.01f, duration);

        if (xrController != null)
        {
            xrController.SendHapticImpulse(a, d);
            // Debug.Log($"[ContactHaptics] {src} -> XRBaseController impulse a={a}, d={d}");
            return;
        }

        if (!device.isValid) TryResolveDevice();
        if (device.isValid && deviceSupportsImpulse)
        {
            bool ok = device.SendHapticImpulse(0, a, d);
            // Debug.Log($"[ContactHaptics] {src} -> InputDevice impulse ok={ok}, a={a}, d={d}");
        }
        else
        {
            // Debug.LogWarning($"[ContactHaptics] {src} -> device no haptic support / not valid (node={xrNode}).");
        }
    }

    void OnDrawGizmosSelected()
    {
        var sc = GetComponent<SphereCollider>();
        if (!enablePolling || sc == null) return;
        Gizmos.color = Color.cyan;
        float r = sc.radius * Mathf.Max(transform.lossyScale.x, Mathf.Max(transform.lossyScale.y, transform.lossyScale.z)) * pollRadiusScale;
        Gizmos.DrawWireSphere(transform.TransformPoint(sc.center), r);
    }
}

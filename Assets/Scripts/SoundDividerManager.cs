using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundDividerManager : MonoBehaviour
{
    public GameObject SoundDividerObject;       // Sound Divider object
    public AudioSource BackgroundAudioSource;   // Background audio source
    public Toggle DividerToggle;                // Divider toggle

    private float baseVolume;                   // Original background volume
    public float dividerEffectFactor = 0.7f;   // Volume reduction factor (30% reduction)

    private bool isDividerActive = false;

    void Start()
    {
        // Attach background sound and inherit its volume
        baseVolume = BackgroundAudioSource.volume;

        // Bind UI controls
        DividerToggle.onValueChanged.AddListener(ToggleSoundDivider);
        SoundDividerObject.SetActive(false); // Initially not active
    }

    public void ToggleSoundDivider(bool isActive)
    {
        isDividerActive = isActive;

        if (isDividerActive)
        {
            // Activate Divider: show object and reduce background volume
            SoundDividerObject.SetActive(true);
            BackgroundAudioSource.volume = baseVolume * dividerEffectFactor;
        }
        else
        {
            // Deactivate Divider: hide object and restore volume
            SoundDividerObject.SetActive(false);
            BackgroundAudioSource.volume = baseVolume;
        }
    }
}


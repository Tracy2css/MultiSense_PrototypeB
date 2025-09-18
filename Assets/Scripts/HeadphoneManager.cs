using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadphoneManager : MonoBehaviour
{
    public AudioSource MusicAudioSource;    // Music audio source
    public Button VolumeUpButton;           // Button to increase volume
    public Button VolumeDownButton;         // Button to decrease volume
    public Toggle HeadphoneToggle;          // Headphone toggle
    public AudioSource BackgroundAudioSource; // Background sound manager

    private bool isHeadphoneOn = false;
    private float volumeStep = 0.1f;        // Step size for volume adjustment

    void Start()
    {
        // Initialize headphone state
        MusicAudioSource.loop = true;
        MusicAudioSource.volume = 0.1f;
        MusicAudioSource.Stop();

        // Bind UI controls
        HeadphoneToggle.onValueChanged.AddListener(ToggleHeadphoneMode);
        VolumeUpButton.onClick.AddListener(IncreaseVolume);
        VolumeDownButton.onClick.AddListener(DecreaseVolume);
    }

    public void ToggleHeadphoneMode(bool isOn)
    {
        isHeadphoneOn = isOn;

        if (isHeadphoneOn)
        {
            // Start playing music through headphones
            MusicAudioSource.Play();
        }
        else
        {
            // Stop playing music through headphones
            MusicAudioSource.Stop();
        }
    }

    public void IncreaseVolume()
    {
        // Increase music volume
        MusicAudioSource.volume = Mathf.Clamp(MusicAudioSource.volume + volumeStep, 0, 1);
    }

    public void DecreaseVolume()
    {
        // Decrease music volume
        MusicAudioSource.volume = Mathf.Clamp(MusicAudioSource.volume - volumeStep, 0, 1);
    }
}

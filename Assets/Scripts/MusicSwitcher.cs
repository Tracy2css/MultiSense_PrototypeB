using UnityEngine;
using TMPro;   // Import TextMeshPro namespace

public class TMPDropdownMusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] songs;

    private TMP_Dropdown dropdown;

    void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>(); // Auto get from same GameObject
        dropdown.onValueChanged.AddListener(OnSongChanged);
    }

    void Start()
    {
        if (songs != null && songs.Length > 0 && audioSource != null)
        {
            // Default play first song
            audioSource.clip = songs[0];
            audioSource.Play();
        }
    }

    private void OnSongChanged(int index)
    {
        if (index >= 0 && index < songs.Length && audioSource != null)
        {
            audioSource.clip = songs[index];
            audioSource.Play();
        }
    }
}

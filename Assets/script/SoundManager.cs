using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [SerializeField] private AudioClip walkingSound;
    [SerializeField] private AudioClip runningSound;
    [SerializeField] private float soundEffectVolume = 1;

    private void Awake()
    {
        Instance = this;
        if (Instance != null)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayWalkingSound(AudioSource audioSource)
    {
        
        if (!audioSource.isPlaying)
        {
            audioSource.clip = walkingSound;
            audioSource.spatialBlend = 1.0f; // Set to 3D sound
            audioSource.Play();
            Debug.Log(audioSource.isPlaying);
        }
    }

    public void StopWalkingSound(AudioSource audioSource)
    {
        if (audioSource.isPlaying && audioSource.clip == walkingSound)
        {
            audioSource.Stop();
        }
    }

    public void PlayRunningSound(AudioSource audioSource)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = runningSound;
            audioSource.spatialBlend = 1.0f; // Set to 3D sound
            audioSource.Play();
            Debug.Log(audioSource.isPlaying);
        }
    }

    public void StopRunningSound(AudioSource audioSource)
    {
        if (audioSource.isPlaying && audioSource.clip == runningSound)
        {
            audioSource.Stop();
        }
    }

    private void Update()
    {
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        soundEffectVolume = SettingUI.Instance.soundEffectSlider.value;
        var audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            if (audioSource.clip == walkingSound || audioSource.clip == runningSound)
            {
                audioSource.volume = soundEffectVolume-0.3f;
            }
            else
            {
                audioSource.volume = soundEffectVolume;
            }
        }
    }
}
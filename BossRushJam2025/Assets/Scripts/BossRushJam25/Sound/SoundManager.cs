using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Utils;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private SerializableDictionary<string, AudioClip> sounds;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        Instance = this;
        musicSource.volume = 0f;
    }

    public void Play(string soundName)
    {
        sfxSource.PlayOneShot(sounds[soundName]);
    }

    public void PlayMusic(float volumeRatio)
    {
        if(!musicSource.isPlaying)
        {
            musicSource.Play();
        }

        StartCoroutine(FadeVolume(musicSource, targetVolume: volumeRatio, duration: 2f));
    }

    public IEnumerator FadeVolume(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;

        float progress = 0f;

        while(progress < duration)
        {
            float sineEasedRatio = 1 - Mathf.Cos(progress / duration * Mathf.PI / 2);
            source.volume = Mathf.Lerp(startVolume, targetVolume, sineEasedRatio);
            progress += Time.deltaTime;

            yield return null;
        }

        source.volume = targetVolume;
    }
}

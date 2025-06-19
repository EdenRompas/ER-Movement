using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

[Serializable]
public class Sounds
{
    public AudioClip Clip;
    public string Id;
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [SerializeField] private List<Sounds> _sounds = new List<Sounds>();
    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlaySFX(AudioClip clip, float volume, float delay = 0)
    {
        StartCoroutine(PlaySFXCoroutine(clip, volume, delay));
    }

    public void PlaySFX(string id, float volume)
    {
        foreach (var item in _sounds)
        {
            if (item.Id == id)
            {
                _audioSource.PlayOneShot(item.Clip, volume);
            }
        }
    }

    private IEnumerator PlaySFXCoroutine(AudioClip clip, float volume, float delay)
    {
        yield return new WaitForSeconds(delay);
        _audioSource.PlayOneShot(clip, volume);
    }
}
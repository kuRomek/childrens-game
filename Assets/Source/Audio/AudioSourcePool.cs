using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSourcePool : MonoBehaviour
{
    [SerializeField] private int _initialSourcesCount = 16;
    [SerializeField] private AudioMixerGroup _audioMixerGroup;

    private List<AudioSource> _availableSources;

    private void Awake()
    {
        _availableSources = new List<AudioSource>(_initialSourcesCount * 2);

        for (int i = 0; i < _initialSourcesCount; i++)
            _availableSources.Add(CreateAudioSource());
    }

    public AudioSource Get()
    {
        AudioSource audioSource;

        if (_availableSources.Count == 0)
        {
            audioSource = CreateAudioSource();
        }
        else
        {
            audioSource = _availableSources[_availableSources.Count - 1];
            _availableSources.RemoveAt(_availableSources.Count - 1);
        }

        return audioSource;
    }

    public void Release(AudioSource audioSource)
    {
        _availableSources.Add(audioSource);
    }

    private AudioSource CreateAudioSource()
    {
        var go = new GameObject("AudioSource");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        go.transform.SetParent(transform);
        audioSource.outputAudioMixerGroup = _audioMixerGroup;
        return audioSource;
    }
}

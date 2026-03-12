using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioPlayer : Installer
{
    private const string SoundsPath = "Sounds";

    [SerializeField] private AudioSourcePool _soundAudioSourcePool;
    [SerializeField] private AudioSourcePool _musicAudioSourcePool;

    public static AudioPlayer Instance { get; private set; }

    private Dictionary<AudioKeys.Sound, AudioClip> _sounds = new();
    private Dictionary<AudioKeys.Music, AudioClip> _music = new();

    public override void Install()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }

    public static async UniTaskVoid PlaySoundEffect(SoundEffect soundEffect)
    {
        AudioSource audioSource = Instance._soundAudioSourcePool.Get();
        audioSource.pitch = Random.Range(soundEffect.PitchRange.x, soundEffect.PitchRange.y);
        audioSource.transform.position = soundEffect.Position;
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 4f;
        audioSource.PlayOneShot(Instance.GetClip(soundEffect.SoundKey));

        await UniTask.WaitWhile(() => audioSource != null && audioSource.isPlaying);

        if (audioSource != null)
            Instance._soundAudioSourcePool.Release(audioSource);
    }

    private AudioClip GetClip(AudioKeys.Sound key)
    {
        if (_sounds.TryGetValue(key, out AudioClip clip) == false)
        {
            clip = Resources.Load<AudioClip>($"{SoundsPath}/{key}");
            _sounds[key] = clip;
        }

        return clip;
    }
}
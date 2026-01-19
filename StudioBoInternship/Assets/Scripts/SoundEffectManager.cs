using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance;

    private static AudioSource _audioSource;
    private static AudioSource _randomPitchAudioSource;
    private static AudioSource _voiceAudioSource;
    private static SoundEffectLibrary _soundEffectLibrary;
    public Slider sfxSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            AudioSource[] audioSources = GetComponents<AudioSource>();
            _audioSource = audioSources[0];
            _randomPitchAudioSource = audioSources[1];
            _voiceAudioSource = audioSources[2];
            _soundEffectLibrary = GetComponent<SoundEffectLibrary>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        sfxSlider = GameObject.Find("SFXSlider").GetComponent<Slider>();
        sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
        foreach (AudioSource audioSource in GetComponents<AudioSource>())
        {
            audioSource.volume = sfxSlider.value;
        }
    }

    public static void Play(string soundName, bool randomPitch = false)
    {
        AudioClip audioClip = _soundEffectLibrary.GetRandomClip(soundName);
        if (audioClip != null)
        {
            if (randomPitch)
            {
                _randomPitchAudioSource.pitch = Random.Range(1f, 1.5f);
                _randomPitchAudioSource.PlayOneShot(audioClip);
            }
            else
            {
                _audioSource.PlayOneShot(audioClip);
            }
        }
    }

    public static void PlayVoice(AudioClip voiceClip, float pitch = 1f)
    {
        _voiceAudioSource.pitch = pitch;
        _voiceAudioSource.PlayOneShot(voiceClip);
    }

    public static void SetVolume(float volume)
    {
        _audioSource.volume = volume;
        _randomPitchAudioSource.volume = volume;
        _voiceAudioSource.volume = volume;
    }

    public void OnValueChanged()
    {
        SetVolume(sfxSlider.value);
    }
}

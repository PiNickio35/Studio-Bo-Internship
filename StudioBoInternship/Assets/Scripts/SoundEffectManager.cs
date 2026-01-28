using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance;

    private static AudioSource _audioSource;
    private static AudioSource _randomPitchAudioSource;
    private static AudioSource _voiceAudioSource;
    private static AudioSource _musicAudioSource;
    private static SoundEffectLibrary _soundEffectLibrary;
    public Slider sfxSlider;
    public Slider musicSfxSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            AudioSource[] audioSources = GetComponents<AudioSource>();
            _audioSource = audioSources[0];
            _randomPitchAudioSource = audioSources[1];
            _voiceAudioSource = audioSources[2];
            _musicAudioSource = audioSources[3];
            _soundEffectLibrary = GetComponent<SoundEffectLibrary>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
        musicSfxSlider.onValueChanged.AddListener(delegate { OnMusicValueChanged(); });
        for (int i = 0; i < GetComponents<AudioSource>().Length; i++) 
        {
            GetComponents<AudioSource>()[i].volume = sfxSlider.value;
            if (i == 3)
            {
                GetComponents<AudioSource>()[i].volume = musicSfxSlider.value;
            }
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
    
    public static void SetMusicVolume(float volume)
    {
        _musicAudioSource.volume = volume;
    }

    public void OnValueChanged()
    {
        SetVolume(sfxSlider.value);
    }
    
    public void OnMusicValueChanged()
    {
        SetMusicVolume(musicSfxSlider.value);
    }
}

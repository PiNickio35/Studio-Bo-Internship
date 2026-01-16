using System.Collections.Generic;
using UnityEngine;

public class SoundEffectLibrary : MonoBehaviour
{
    private Dictionary<string, List<AudioClip>> _soundDictionary;
    [SerializeField] private SoundEffectGroup[] soundEffectGroups;

    private void Awake()
    {
        InitialiseDictionary();
    }

    private void InitialiseDictionary()
    {
        _soundDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (SoundEffectGroup soundEffectGroup in soundEffectGroups)
        {
            _soundDictionary[soundEffectGroup.Name] = soundEffectGroup.AudioClips;
        }
    }

    public AudioClip GetRandomClip(string clipName)
    {
        if (_soundDictionary.ContainsKey(clipName))
        {
            List<AudioClip> audioClips = _soundDictionary[clipName];
            if (audioClips.Count > 0)
            {
                return audioClips[Random.Range(0, audioClips.Count)];
            }
        }

        return null;
    }
}

[System.Serializable]
public struct SoundEffectGroup
{
    public string Name;
    public List<AudioClip> AudioClips;
}

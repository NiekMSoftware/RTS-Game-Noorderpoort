using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Vector2 pitchRange;
    [SerializeField] private Vector2 volumeRange;

    public static AudioManager Instance;

    public AudioGroup[] audioGroups;

    public enum AudioGroupNames
    {
        None,
        Rain
    }

    [System.Serializable]
    public class AudioGroup
    {
        public AudioGroupNames name;
        public AudioSource[] audioSources;
        public bool isPlaying = false;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySounds(AudioGroup audioGroup, bool shouldLoop = false, bool randomize = false)
    {
        if (audioGroup.name == AudioGroupNames.None) return;

        if (audioGroup.isPlaying)
        {
            foreach (var audio in audioGroup.audioSources)
            {
                audio.DOFade(0, 1).OnComplete(() => audio.Stop());
                audio.volume = 1;
            }
        }

        audioGroup.isPlaying = true;
        int randomNum = Random.Range(0, audioGroup.audioSources.Length);

        float randomVolume = 1;

        if (randomize)
        {
            randomVolume = Random.Range(volumeRange.x, volumeRange.y);
            audioGroup.audioSources[randomNum].pitch = Random.Range(pitchRange.x, pitchRange.y);
        }

        audioGroup.audioSources[randomNum].volume = 0;

        audioGroup.audioSources[randomNum].loop = shouldLoop;
        audioGroup.audioSources[randomNum].Play();
        audioGroup.audioSources[randomNum].DOFade(randomVolume, 1);
    }

    public void PlaySound(AudioSource audioSource, bool shouldLoop = false)
    {
        audioSource.Play();
        audioSource.loop = shouldLoop; 
    }
}
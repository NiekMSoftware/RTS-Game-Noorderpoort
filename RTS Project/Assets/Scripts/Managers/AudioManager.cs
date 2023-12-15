using DG.Tweening;
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
        Rain,
        HeavyRain
    }

    [System.Serializable]
    public class AudioGroup
    {
        public AudioGroupNames name;
        public List<AudioSource> audioSources;
        public List<AudioSource> currentAudioSources;
        public bool isPlaying = false;
    }

    private void Awake()
    {
        Instance = this;

        foreach (var audioGroup in audioGroups)
        {
            audioGroup.currentAudioSources = new List<AudioSource>(audioGroup.audioSources);
        }
    }

    public void PlaySounds(AudioGroup audioGroup, bool shouldLoop = false, bool randomize = false)
    {
        if (audioGroup.name == AudioGroupNames.None) return;

        Sequence audioSequence = DOTween.Sequence();
        AudioSource audioSourceToStop = null;

        if (audioGroup.isPlaying)
        {
            foreach (var audio in audioGroup.audioSources)
            {
                if (audio.isPlaying)
                {
                    audioSourceToStop = audio;
                    break;
                }
            }
        }

        audioGroup.isPlaying = true;

        if (audioGroup.currentAudioSources.Count <= 0)
        {
            audioGroup.currentAudioSources = new List<AudioSource>(audioGroup.audioSources);

            for (int i = 0; i < audioGroup.currentAudioSources.Count; i++)
            {
                if (audioGroup.currentAudioSources[i] == audioSourceToStop)
                {
                    audioGroup.currentAudioSources.Remove(audioSourceToStop);
                    break;
                }
            }
        }

        int randomNum = Random.Range(0, audioGroup.currentAudioSources.Count);
        AudioSource audioSource = audioGroup.currentAudioSources[randomNum];

        float randomVolume = 1;

        if (randomize)
        {
            randomVolume = Random.Range(volumeRange.x, volumeRange.y);
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        }

        audioSource.volume = 0;

        audioSource.loop = shouldLoop;
        audioSource.Play();
        if (audioSourceToStop)
        {
            audioSequence.Append(audioSourceToStop.DOFade(0, 1));
        }
        audioSequence.Append(audioSource.DOFade(randomVolume, 1).OnComplete(() => StopAudioSource(audioSourceToStop, audioGroup, audioSource)));
        audioGroup.currentAudioSources.Remove(audioSource);
        audioSequence.Play();
    }

    private void StopAudioSource(AudioSource previousAudio, AudioGroup audioGroup, AudioSource currentAudio)
    {
        if (previousAudio)
        {
            previousAudio.Stop();
        }
    }

    public void PlaySound(AudioSource audioSource, bool shouldLoop = false, bool randomize = false)
    {
        audioSource.Play();
        audioSource.loop = shouldLoop;

        if (randomize)
        {
            audioSource.volume = Random.Range(volumeRange.x, volumeRange.y);
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        }
    }
}
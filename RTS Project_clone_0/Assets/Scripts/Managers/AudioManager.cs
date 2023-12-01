using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;

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
        public bool isPlaying = false;
    }

    private void Awake()
    {
        Instance = this;
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
                    print("something is playing");
                    audioSourceToStop = audio;
                    break;
                }
            }
        }

        audioGroup.isPlaying = true;
        int randomNum = Random.Range(0, audioGroup.audioSources.Count);


        float randomVolume = 1;

        if (randomize)
        {
            randomVolume = Random.Range(volumeRange.x, volumeRange.y);
            audioGroup.audioSources[randomNum].pitch = Random.Range(pitchRange.x, pitchRange.y);
        }

        audioGroup.audioSources[randomNum].volume = 0;

        audioGroup.audioSources[randomNum].loop = shouldLoop;
        audioGroup.audioSources[randomNum].Play();
        if (audioSourceToStop)
        {
            print("fade out audio");
            audioSequence.Append(audioSourceToStop.DOFade(0, 1));
        }
        audioSequence.Append(audioGroup.audioSources[randomNum].DOFade(randomVolume, 1).OnComplete(() => StopAudioSource(audioSourceToStop, audioGroup, randomNum)));
        audioSequence.Play();
    }

    private void StopAudioSource(AudioSource audioSource, AudioGroup audioGroup, int randomNum)
    {
        if (audioSource)
        {
            print("stop");
            audioSource.Stop();
            audioGroup.audioSources.Remove(audioGroup.audioSources[randomNum]);
            audioGroup.audioSources.Add(audioSource);
        }
    }

    public void PlaySound(AudioSource audioSource, bool shouldLoop = false)
    {
        audioSource.Play();
        audioSource.loop = shouldLoop;
    }
}
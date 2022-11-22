using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public AudioSource _audioSuccess;
    public AudioSource _audioWarnning;
    public AudioSource _audioBeepLoop;
    public AudioSource _audioStarOrder;

    // Singleton instantiation
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.FindObjectOfType<AudioManager>();
            }
            return _instance;
        }
    }

    void Start()
    {
        _audioBeepLoop.loop = true;
    }

    public static bool IsPlayingAudio(AudioSource audio)
    {
        return (null != audio) ? audio.isPlaying : false;
    }

    public static void SetAudioVolume(AudioSource audio, float volume)
    {
        if(null != audio)
        {
            audio.volume = volume;
        }
    }

    public static void PlayAudio(AudioSource audio)
    {
        if(null != audio)
        {
            audio.Play();
        }
    }

    public static void StopAudio(AudioSource audio)
    {
        if(null != audio)
        {
            audio.Stop();
        }
    }

    public static void PauseAudio(AudioSource audio)
    {
        if(null != audio)
        {
            audio.Pause();
        }
    }

    public static IEnumerator FadeAudio(AudioSource audioSource, float duration, float targetVolume, bool killAudio)
    {
        if(null != audioSource)
        {
            float currentTime = 0;
            float start = audioSource.volume;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                if(audioSource.volume == targetVolume && killAudio)
                {
                    StopAudio(audioSource);
                }
                yield return null;
            }
        }
        yield break;
    }
}

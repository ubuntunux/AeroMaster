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

    public bool IsPlayingAudio(AudioSource audio)
    {
        return (null != audio) ? audio.isPlaying : false;
    }

    public void SetAudioVolume(AudioSource audio, float volume)
    {
        if(null != audio)
        {
            audio.volume = volume;
        }
    }

    public void PlayAudio(AudioSource audio)
    {
        if(null != audio)
        {
            audio.Play();
        }
    }

    public void StopAudio(AudioSource audio)
    {
        if(null != audio)
        {
            audio.Stop();
        }
    }

    public void PauseAudio(AudioSource audio)
    {
        if(null != audio)
        {
            audio.Pause();
        }
    }
}

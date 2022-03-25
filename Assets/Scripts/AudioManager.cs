using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public AudioSource _audioSuccess;
    public AudioSource _audioWarnning;
    public AudioSource _audioBeepLoop;

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
}

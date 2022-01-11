using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    public AudioSource _audioSource;
    public AudioClip[] _bgmList;

    void PlayRandomMusic()
    {
        int count = _bgmList.Length;
        int index = Random.Range(0, count - 1);
        _audioSource.clip = _bgmList[index];
        _audioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayRandomMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if(null == _audioSource.clip || false == _audioSource.isPlaying)
        {
            PlayRandomMusic();
        }
    }
}

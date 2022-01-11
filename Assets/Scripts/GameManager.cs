using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public AudioSource _audioSource; //A primary audioSource a large portion of game sounds are passed through
    public AudioMixer _audioMaster;
    public AudioClip _missionCompleteAudio;
    public GameObject _missionCompleteText;    
    private float _masterVolumeStore = 0.0f;
    private float _musicVolumeStore = 0.0f;
    private bool _paused = false;
    private bool _missionComplete = false;

    // Singleton instantiation
    public static GameManager Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

#if UNITY_ANDROID
    public Vector2 GetAltitudeTouchDelta()
    {
        for(int i = 0; i < Input.touchCount; ++i)
        {
            Touch touch = Input.GetTouch(i);
            if(touch.position.x < (Screen.width / 2.0f))
            {
                return touch.deltaPosition;
            }
        }
        return Vector2.zero;
    }
#endif

    public void SetPause(bool pause)
    {
        if(pause == _paused)
        {
            return;
        }

        if (pause)
        {
            // pause
            Time.timeScale = 0.0f;            
            _audioMaster.GetFloat("MasterVolume", out _masterVolumeStore);
            _audioMaster.SetFloat("MasterVolume", -80.0f);
        }
        else
        {
            // resume
            Time.timeScale = 1.0f;
            _audioMaster.SetFloat("MasterVolume", _masterVolumeStore);
        }

        _paused = pause;
    }

    public void OnClickPause()
    {
        SetPause(!_paused);
    }

    public void SetMissionComplete(bool missionComplete)
    {
        if(missionComplete != _missionComplete)
        {
            if(missionComplete)
            {
                Player.Instance.SetControllable(false);
                MainCamera.Instance.SetTrackingPlayer(false);
                _audioMaster.GetFloat("MusicVolume", out _musicVolumeStore);
                _audioMaster.SetFloat("MusicVolume", -80.0f);
                _audioSource.PlayOneShot(_missionCompleteAudio);
            }

            _missionCompleteText.SetActive(missionComplete);
            _missionComplete = missionComplete;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioMaster.GetFloat("MasterVolume", out _masterVolumeStore);
        _audioMaster.GetFloat("MusicVolume", out _musicVolumeStore);

        Reset();
    }

    public void Reset()
    {
        SetPause(false);
        SetMissionComplete(false);

        LevelManager.Instance.Reset();
        Vector3 startPoint = LevelManager.Instance.GetStartPoint();
        Player.Instance.ResetPlayer(startPoint);
        MainCamera.Instance.Reset();

        _audioMaster.SetFloat("MasterVolume", _masterVolumeStore);
        _audioMaster.SetFloat("MusicVolume", _musicVolumeStore);
    }

    // Update is called once per frame
    void Update()
    {
    }
}

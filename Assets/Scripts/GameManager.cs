using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class GameManager : MonoBehaviour
{
    public AudioSource _audioSource; //A primary audioSource a large portion of game sounds are passed through
    public AudioMixer _audioMaster;
    public AudioClip _missionCompleteAudio;
    public AudioClip _gameOverAudio;
    public GameObject _missionCompleteText;    
    private float _masterVolumeStore = 0.0f;
    private float _musicVolumeStore = 0.0f;
    private bool _paused = false;
    private bool _levelEnded = false;

    private static GameManager _instance;

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

    public void RunBack()
    {
        if(LevelManager.Instance.IsLevelProfile())
        {
            Application.Quit();
        }
        else if(LevelManager.Instance.IsLevelLobby())
        {
            LevelManager.Instance.GoToLevelProfile();
        }
        else
        {
            LevelManager.Instance.GoToLevelLobby();
        }
    }

    public void OnClickToggleMusic()
    {
        _musicVolumeStore = (_musicVolumeStore < 0.0f) ? 0.0f : -80.0f;
        _audioMaster.SetFloat("MusicVolume", _musicVolumeStore);
    }

    public void SetLevelStart(bool controllable, bool invincibility, Vector3 startPoint, bool isFlying = false, bool autoFlyingToRight = true)
    {
        MainCamera.Instance.ResetMainCamera();
        Player.Instance.ResetPlayer();        
        Player.Instance.SetControllable(controllable);
        Player.Instance.SetInvincibility(invincibility);
        Player.Instance.SetPosition(startPoint);
        if(isFlying)
        {
            Player.Instance.SetAutoFlying(autoFlyingToRight);
        }

        bool hideControllerUI = LevelManager.Instance.IsLevelProfile() || LevelManager.Instance.IsLevelLobby();
        UIManager.Instance.SetVisibleControllerUI(!hideControllerUI);

        _audioMaster.SetFloat("MusicVolume", _musicVolumeStore);
        _missionCompleteText.SetActive(false);
        _levelEnded = false;
    }

    public void SetLevelEnd(bool isMissionSuccess = true)
    {
        Player.Instance.SetControllable(!isMissionSuccess);
        MainCamera.Instance.SetTrackingPlayer(!isMissionSuccess);
        _audioMaster.GetFloat("MusicVolume", out _musicVolumeStore);
        _audioMaster.SetFloat("MusicVolume", -80.0f);
        _audioSource.PlayOneShot(isMissionSuccess ? _missionCompleteAudio : _gameOverAudio);

        _missionCompleteText.SetActive(true);
        _missionCompleteText.GetComponent<TextMeshProUGUI>().text = isMissionSuccess ? "Mission Completed" : "Mission Failed";
        _levelEnded = true;
    }

    void Start()
    {
        _audioMaster.GetFloat("MasterVolume", out _masterVolumeStore);
        _audioMaster.GetFloat("MusicVolume", out _musicVolumeStore);
        ResetGameManager();
    }

    public void ResetGameManager()
    {
        SetPause(false);

        UIManager.Instance.ResetUIManager();
        LevelManager.Instance.ResetLevelManager();
        Player.Instance.ResetPlayer();
        Player.Instance.SetControllable(false);
        Player.Instance.SetInvincibility(false);
        MainCamera.Instance.ResetMainCamera();

        _audioMaster.SetFloat("MasterVolume", _masterVolumeStore);
        _audioMaster.SetFloat("MusicVolume", _musicVolumeStore);
    }

    // Update is called once per frame
    void Update()
    {
    }
}

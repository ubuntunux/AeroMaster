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
    private float _masterVolumeStore = 0.0f;
    private float _musicVolumeStore = 0.0f;
    private bool _paused = false;
    private bool _levelEnded = false;
    private float _lastTouchPositionY = 0.0f;
    private bool _isTouched = false;

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
                if(false == _isTouched)
                {
                    _lastTouchPositionY = touch.position.y;
                    _isTouched = true;
                }

                Vector2 deltaPosition = touch.deltaPosition;
                deltaPosition.y = (touch.position.y - _lastTouchPositionY) / (Screen.height * Constants.INPUT_Y_HEIGHT_RATIO);
                return deltaPosition;
            }
        }
        _isTouched = false;
        
        return Vector2.zero;
    }
#endif

    public bool GetPaused()
    {
        return _paused;
    }

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
        LevelManager.Instance.OnLevelStart();
        MainCamera.Instance.ResetMainCamera();
        Player.Instance.ResetPlayer();
        Player.Instance.SetControllable(controllable);
        Player.Instance.SetInvincibility(invincibility);
        Player.Instance.SetPosition(startPoint);
        if(isFlying)
        {
            Player.Instance.SetAutoFlying(autoFlyingToRight);
        }

        UIManager.Instance.OnLevelStart();

        _audioMaster.SetFloat("MusicVolume", _musicVolumeStore);
        _lastTouchPositionY = 0.0f;
        _isTouched = false;        
        _levelEnded = false;
    }

    public void SetLevelEnd(bool isMissionSuccess = true)
    {
        LevelManager.Instance.OnLevelEnd();
        Player.Instance.SetControllable(false);
        MainCamera.Instance.SetTrackingPlayer(false);
        UIManager.Instance.OnLevelEnd(isMissionSuccess);
        ActorScriptManager.Instance.ClearActorScriptsPages();
        
        _audioMaster.GetFloat("MusicVolume", out _musicVolumeStore);
        _audioMaster.SetFloat("MusicVolume", -80.0f);
        _audioSource.PlayOneShot(isMissionSuccess ? _missionCompleteAudio : _gameOverAudio);
        _levelEnded = true;

        // save data
        SaveData.Instance.Save(Constants.DefaultDataName);
    }

    void Start()
    {
        _audioMaster.GetFloat("MasterVolume", out _masterVolumeStore);
        _audioMaster.GetFloat("MusicVolume", out _musicVolumeStore);

        ResetGameManager();        
    }

    public void ResetGameManager()
    {
        // load data
        SaveData.Instance.Load(Constants.DefaultDataName);

        SetPause(false);

        UIManager.Instance.ResetUIManager();
        LevelManager.Instance.ResetLevelManager();

        Player.Instance.LoadPlayerData(SaveData.Instance._playerData);
        Player.Instance.ResetPlayer();
        Player.Instance.SetControllable(false);
        Player.Instance.SetInvincibility(false);

        MainCamera.Instance.ResetMainCamera();

        _audioMaster.SetFloat("MasterVolume", _masterVolumeStore);
        _audioMaster.SetFloat("MusicVolume", _musicVolumeStore);
    }

    public bool CheckMissionRegion()
    {   
        Vector2 region = LevelManager.Instance.GetMissionRegion();
        
        if(Vector2.zero != region)
        {   
            Vector3 playerPosition = Player.Instance.GetPosition();
            bool isRightDirection = 1.0f == Player.Instance.GetFrontDirection();
            float minToPlayer = playerPosition.x - region.x;
            float playerToMax = region.y - playerPosition.x;

            // mission region warning sign
            if(false == isRightDirection && minToPlayer <= Constants.WARNING_DISTANCE)
            {   
                UIManager.Instance.ShowMissionRegionWarning(true, false);
            }
            else if(isRightDirection && playerToMax <= Constants.WARNING_DISTANCE)
            {
                UIManager.Instance.ShowMissionRegionWarning(true, true);
            }
            else
            {
                UIManager.Instance.ShowMissionRegionWarning(false, false);
            }

            // check mission failed
            return minToPlayer <= 0.0f || playerToMax <= 0.0f;
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
    }
}

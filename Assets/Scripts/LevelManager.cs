using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

abstract public class LevelBase: MonoBehaviour
{
    abstract public string GetMissionTitle();
    abstract public string GetMissionDetails();
    abstract public void OnStartLevel();
    abstract public void OnExitLevel();
    abstract public bool IsEndLevel();
    abstract public void UpdateLevel();
    abstract public int GetMissionTime();
}

public class LevelManager : MonoBehaviour
{
    public GameObject _levelProfilePrefab;
    public GameObject _levelLobbyPrefab;
    public GameObject _levelTutorialPrefab;
    public GameObject _levelMissionPrefab;
    
    GameObject _previousLevelPrefab = null;
    GameObject _currentLevelPrefab = null;
    GameObject _currentLevel = null;
    bool _firstUpdate = true;

    Vector3 _startPosition = Vector3.zero;

    IndicatorUI _goalIndicator = null;    
    Vector3 _goalPosition = Vector3.zero;

    Vector2 _missionRegion = Vector2.zero;
    List<RegionMarkerFX> _regionMarkerFXs = new List<RegionMarkerFX>();

    // Singleton instantiation
    static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.FindObjectOfType<LevelManager>();
            }
            return _instance;
        }
    }

    void Start()
    {
    }

    public void GoToLevelProfile()
    {
        SetCurrentLevel(_levelProfilePrefab);
    }

    public void GoToLevelLobby()
    {
        SetCurrentLevel(_levelLobbyPrefab);
    }

    public GameObject GetLevelTutorialPrefab()
    {
        return _levelTutorialPrefab;
    }

    public GameObject GetLevelMissionPrefab()
    {
        return _levelMissionPrefab;
    }

    public LevelBase GetCurrentLevel()
    {
        return null != _currentLevel ? _currentLevel.GetComponent<LevelBase>() : null;
    }

    public void SetCurrentLevel(GameObject levelPrefab, float fadeInRatio = 1.0f)
    {
        UIManager.Instance.SetFadeInOutAndLevelChange(levelPrefab, fadeInRatio);
    }

    public void SetCurrentLevelCallback(GameObject levelPrefab)
    {
        if(levelPrefab != _currentLevelPrefab)
        {
            UIManager.Instance.ResetCharacterText();

            if(null != _currentLevel)
            {
                _currentLevel.GetComponent<LevelBase>().OnExitLevel();
                _currentLevel.SetActive(false);
                Destroy(_currentLevel);
                _currentLevel = null;
            }

            _previousLevelPrefab = _currentLevelPrefab;
            _currentLevelPrefab = levelPrefab;

            if(null != levelPrefab)
            {
                // Note: before create a new level
                BeforeCreateNewLevel();

                // create & update the new level
                _currentLevel = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
                _currentLevel.transform.SetParent(transform, false);
                _currentLevel.SetActive(true);
                // will call - GameManager.Instance.SetLevelStart
                _currentLevel.GetComponent<LevelBase>().OnStartLevel();
            }
        }
    }

    public bool IsNullLevelPrefab()
    {
        return null == _currentLevelPrefab;
    }

    public bool IsLevelProfile()
    {
        return _levelProfilePrefab == _currentLevelPrefab;
    }

    public bool IsLevelLobby()
    {
        return _levelLobbyPrefab == _currentLevelPrefab;
    }

    public bool IsEndCurrentLevel()
    {
        return (null != _currentLevelPrefab) ? _currentLevelPrefab.GetComponent<LevelBase>().IsEndLevel() : true;
    }

    public int GetMissionTime()
    {
        return (null == _currentLevel) ? 0 : _currentLevel.GetComponent<LevelBase>().GetMissionTime();
    }

    // Start Point
    public Vector3 GetStartPosition()
    {
        return _startPosition;
    }

    public void RegistStartPosition(Vector3 startPosition)
    {
        _startPosition = startPosition;
    }

    // Goal Point
    public Vector3 GetGoalPosition()
    {
        return _goalPosition;
    }

    public void RegistGoalPosition(Vector3 goalPosition)
    {
        if(null != _goalIndicator)
        {
            _goalIndicator = UIManager.Instance.CreateIndicatorUI(goalPosition);
        }        
        _goalPosition = goalPosition;
    }

    public void DestroyGoalIndicator()
    {
        UIManager.Instance.DestroyIndicatorUI(ref _goalIndicator);
    }

    // Mission Regions
    public Vector2 GetMissionRegion()
    {
        return _missionRegion;
    }

    public void RegistRegionMarkerFX(RegionMarkerFX regionMarkerFX)
    {
        _regionMarkerFXs.Add(regionMarkerFX);
    }

    public void ClearRegionMarkerFX()
    {
        _regionMarkerFXs.Clear();
    }

    public void UpdateRegionMarkerFXs()
    {
        if(0 < _regionMarkerFXs.Count)
        {
            _missionRegion.x = float.MaxValue;
            _missionRegion.y = float.MinValue;

            foreach(RegionMarkerFX regionMarkerFX in _regionMarkerFXs)
            {
                if(regionMarkerFX.transform.position.x < _missionRegion.x)
                {
                    _missionRegion.x = regionMarkerFX.transform.position.x;
                }

                if(_missionRegion.y < regionMarkerFX.transform.position.x)
                {
                    _missionRegion.y = regionMarkerFX.transform.position.x;
                }
            }
        }
        else
        {
            _missionRegion = Vector2.zero;
        }
    }

    // Level event
    public void BeforeCreateNewLevel()
    {
        ClearRegionMarkerFX();

        _missionRegion = Vector2.zero;
        _startPosition = Vector3.zero;
        _goalPosition = Vector3.zero;
    }

    public void OnStartLevel()
    {
        UpdateRegionMarkerFXs();
    }

    public void OnEndLevel()
    {
        DestroyGoalIndicator();
        ClearRegionMarkerFX();

        _missionRegion = Vector2.zero;
        _startPosition = Vector3.zero;
        _goalPosition = Vector3.zero;
    }

    public void ResetLevelManager()
    {
        _firstUpdate = true;

        // black screen
        float fadeInRatio = 0.51f;
        SetCurrentLevel(null, fadeInRatio);
    }

    void Update()
    {
        if(_firstUpdate)
        {
            SetCurrentLevel(_levelProfilePrefab);
            _firstUpdate = false;
        }

        if(null != _currentLevel)
        {
            // Test indicate region marker
            if(null != _goalIndicator)
            {
                _goalIndicator.SetIndicatorTargetPosition(GetGoalPosition());
            }

            LevelBase currentLevel = _currentLevel.GetComponent<LevelBase>();
            currentLevel.UpdateLevel();
            if(currentLevel.IsEndLevel())
            {
                SetCurrentLevel(_levelLobbyPrefab);
            }
        }
    }
}

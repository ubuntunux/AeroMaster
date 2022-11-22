using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum LevelEndTypes
{
    MissionSucess,
    MissionFailed,
    Silent
};

abstract public class LevelBase: MonoBehaviour
{
    abstract public int GetMissionTime();    
    abstract public void OnStartLevel();
    abstract public void OnExitLevel();
    abstract public void UpdateLevel();
    abstract public Light GetSun();
    abstract public Material GetSkybox();
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
    bool _levelEnded = false;
    float _levelExitTime = 0.0f;
    Color _initialFogColor;

    Vector3 _startPoint = Vector3.zero;

    IndicatorUI _goalIndicator = null;
    GoalPoint _goalPoint = null;

    Vector2 _missionRegion = Vector2.zero;

    List<RegionMarkerFX> _regionMarkerFXs = new List<RegionMarkerFX>();
    List<GameObject> _bullets = new List<GameObject>();

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

    void Awake()
    {
        _initialFogColor = RenderSettings.fogColor;
    }

    void Start()
    {
    }

    public void GoToLevelProfile()
    {
        GameManager.Instance.SetLevelEnd(LevelEndTypes.Silent);
        SetCurrentLevel(_levelProfilePrefab);
    }

    public void GoToLevelLobby()
    {
        GameManager.Instance.SetLevelEnd(LevelEndTypes.Silent);
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

    public bool IsMissionLevel()
    {
        return false == IsLevelProfile() && false == IsLevelLobby();
    }

    public int GetMissionTime()
    {
        return (null == _currentLevel) ? 0 : _currentLevel.GetComponent<LevelBase>().GetMissionTime();
    }

    // Start Point
    public Vector3 GetStartPoint()
    {
        return _startPoint;
    }

    public void RegistStartPoint(Vector3 startPoint)
    {
        _startPoint = startPoint;
    }

    // Goal Point
    public Vector3 GetGoalPoint()
    {
        return (null != _goalPoint) ? _goalPoint.transform.position : Vector3.zero;
    }

    public string GetGoalPointName()
    {
        return (null != _goalPoint) ? _goalPoint._goalPointName : "";
    }

    public void RegistGoalPoint(GoalPoint goalPoint)
    {
        if(null == _goalIndicator)
        {
            _goalIndicator = UIManager.Instance.CreateIndicatorUI(
                goalPoint.transform.position,
                goalPoint._goalPointName,
                new Color(0.5f, 0.5f, 1.0f)
            );
        }        
        _goalPoint = goalPoint;
    }

    public void DestroyGoalPoint()
    {
        UIManager.Instance.DestroyIndicatorUI(ref _goalIndicator);
        _goalPoint = null;
    }

    // Bullets
    public void RegistBulletObject(GameObject bullet)
    {
        _bullets.Add(bullet);
    }

    public void UnregistBulletObject(GameObject bullet)
    {
        _bullets.Remove(bullet);
        Destroy(bullet);
    }

    public void ClearBulletObjects()
    {
        foreach(GameObject bullet in _bullets)
        {
            Destroy(bullet);
        }
        _bullets.Clear();
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

    public void UpdateRenderSetting()
    {
        LevelBase currentLevel = GetCurrentLevel();
        if(null != currentLevel)
        {
            Light sun = currentLevel.GetSun();
            Color sunColor = sun.color;
            float sunIntensity = (sunColor.r + sunColor.g + sunColor.b) * 0.33333f * sun.intensity;                
            
            RenderSettings.fogColor = _initialFogColor * sunIntensity;

            RenderSettings.ambientIntensity = 1;
            RenderSettings.reflectionIntensity = sunIntensity;
            Material skybox = currentLevel.GetSkybox();
            RenderSettings.skybox = new Material(skybox);
            RenderSettings.skybox.SetFloat("_Exposure", 0.55f * sunIntensity);
        }
    }

    // Level event
    public void BeforeCreateNewLevel()
    {
        DestroyGoalPoint();
        ClearRegionMarkerFX();
        ClearBulletObjects();

        _missionRegion = Vector2.zero;
        _startPoint = Vector3.zero;
    }

    public void OnStartLevel()
    {
        UpdateRenderSetting();
        UpdateRegionMarkerFXs();

        _levelEnded = false;
        _levelExitTime = 0.0f;
    }

    public void OnEndLevel(LevelEndTypes type)
    {
        DestroyGoalPoint();
        ClearRegionMarkerFX();
        ClearBulletObjects();

        _missionRegion = Vector2.zero;
        _startPoint = Vector3.zero;

        _levelEnded = true;
        _levelExitTime = (LevelEndTypes.Silent == type) ? 0.0f : Constants.LEVEL_EXIT_TIME;
    }

    public bool IsLevelEnded()
    {
        return _levelEnded;
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
            // update indicate region marker
            if(null != _goalIndicator)
            {
                _goalIndicator.SetIndicatorTargetPosition(GetGoalPoint());
            }

            LevelBase currentLevel = _currentLevel.GetComponent<LevelBase>();
            currentLevel.UpdateLevel();

            if(_levelEnded)
            {
                _levelExitTime -= Time.deltaTime;
                if(_levelExitTime <= 0.0f)
                {
                    SetCurrentLevel(_levelLobbyPrefab);
                    _levelEnded = false;
                }
            }
        }
    }
}

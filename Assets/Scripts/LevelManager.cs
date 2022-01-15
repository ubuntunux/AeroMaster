using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

abstract public class LevelBase: MonoBehaviour
{
    abstract public void OnStartLevel();
    abstract public void OnExitLevel();
    abstract public bool IsEndLevel();
    abstract public void UpdateLevel();
}

public class LevelManager : MonoBehaviour
{
    public GameObject _levelProfilePrefab;
    public GameObject _levelLobbyPrefab;
    public GameObject _levelTutorialPrefab;
    
    private static LevelManager _instance;
    GameObject _previousLevelPrefab = null;
    GameObject _currentLevelPrefab = null;
    GameObject _currentLevel = null;
    bool _firstUpdate = true;

    // Singleton instantiation
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

    public void StartTutorial()
    {
        SetCurrentLevel(_levelTutorialPrefab);
    }

    public void SetCurrentLevel(GameObject levelPrefab)
    {
        if(levelPrefab != _currentLevelPrefab)
        {
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
                _currentLevel = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
                _currentLevel.transform.parent = transform;
                _currentLevel.SetActive(true);
                _currentLevel.GetComponent<LevelBase>().OnStartLevel();
            }

            GameManager.Instance.SetMissionComplete(false);
        }
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

    public void ResetLevelManager()
    {
        _firstUpdate = true;
        SetCurrentLevel(null);
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
            LevelBase currentLevel = _currentLevel.GetComponent<LevelBase>();
            currentLevel.UpdateLevel();
            if(currentLevel.IsEndLevel())
            {
                // TODO: failed to tutorial

                // End of Tutorial
                if(_levelTutorialPrefab == _currentLevelPrefab)
                {
                    SetCurrentLevel(_levelLobbyPrefab);
                }
            }
        }
    }
}

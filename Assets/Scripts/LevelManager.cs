using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class LevelBase: MonoBehaviour
{
    abstract public void OnStartLevel();
    abstract public void OnExitLevel();
    abstract public bool IsEndLevel();
    abstract public void UpdateLevel();
}

public class LevelManager : MonoBehaviour
{
    public GameObject _levelStart;
    public GameObject _levelTutorial;
    
    private static LevelManager _instance;
    GameObject _previousLevel = null;
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

    public void SetCurrentLevelToPreviousLevel()
    {
        SetCurrentLevel(_previousLevel);
    }

    public void SetCurrentLevel(GameObject level, bool force = false)
    {
        if(level != _currentLevel || force)
        {
            if(null != _currentLevel)
            {
                _currentLevel.GetComponent<LevelBase>().OnExitLevel();
                _currentLevel.SetActive(false);
            }
            _previousLevel = _currentLevel;
            _currentLevel = level;

            if(null != level)
            {
                level.SetActive(true);
                level.GetComponent<LevelBase>().OnStartLevel();
            }

            GameManager.Instance.SetMissionComplete(false);
        }
    }

    public bool IsStartLevel()
    {
        return _levelStart == _currentLevel;
    }

    public void StartTutorial()
    {
        SetCurrentLevel(_levelTutorial);
    }

    public bool IsEndCurrentLevel()
    {
        return (null != _currentLevel) ? _currentLevel.GetComponent<LevelBase>().IsEndLevel() : true;
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
            SetCurrentLevel(_levelStart);
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
                if(_levelTutorial == _currentLevel)
                {
                    SetCurrentLevel(_levelStart);
                }
            }
        }
    }
}

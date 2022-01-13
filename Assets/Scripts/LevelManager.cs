using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class LevelBase: MonoBehaviour
{
    abstract public void ResetLevel();
    abstract public bool IsEndLevel();
    abstract public void UpdateLevel();
}

public class LevelManager : MonoBehaviour
{
    public GameObject _levelStart;
    public GameObject _levelTutorial;
    
    private static LevelManager _instance;
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

    public void SetCurrentLevel(GameObject level)
    {
        if(level != _currentLevel)
        {
            if(null != _currentLevel)
            {
                _currentLevel.SetActive(false);
            }            
            _currentLevel = level;

            if(null != level)
            {
                level.SetActive(true);
                level.GetComponent<LevelBase>().ResetLevel();
            }

            GameManager.Instance.SetMissionComplete(false);
        }
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
                if(_levelTutorial == _currentLevel)
                {
                    SetCurrentLevel(_levelStart);
                }
            }
        }
    }
}

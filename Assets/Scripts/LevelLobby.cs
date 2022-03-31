using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelLobby : LevelBase
{
    public GameObject _textMissionTitle;
    public GameObject _textMissionBody;
    public GameObject _layerMissionDetail;

    bool _isFirstUpdate = true;
    GameObject _levelPrefab = null;

    override public string GetMissionTitle()
    {
        return "";
    }

    override public string GetMissionDetails()
    {
        return "";
    }

    public void OnClickTutorial()
    {
        OnClickLevel(LevelManager.Instance.GetLevelTutorialPrefab());
    }

    public void OnClickMission()
    {
        OnClickLevel(LevelManager.Instance.GetLevelMissionPrefab());
    }

    public void OnClickLevel(GameObject levelPrefab)
    {
        if(null != levelPrefab)
        {
            _layerMissionDetail.SetActive(true);

            _levelPrefab = levelPrefab;
            LevelBase level = levelPrefab.GetComponent<LevelBase>();
            _textMissionTitle.GetComponent<TextMeshProUGUI>().text = level.GetMissionTitle();
            _textMissionBody.GetComponent<TextMeshProUGUI>().text = level.GetMissionDetails();
        }
    }

    public void OnClickStartLevel()
    {
        if(null != _levelPrefab)
        {
            LevelManager.Instance.SetCurrentLevel(_levelPrefab);
        }
    }

    public void OnClickCancleLevel()
    {
        _layerMissionDetail.SetActive(false);
    }

    override public void OnStartLevel()
    {
        _layerMissionDetail.SetActive(false);
        
        bool controllable = false;
        bool invincibility = true;
        GameManager.Instance.SetLevelStart(controllable, invincibility);
        
        // Set Camera
        MainCamera.Instance.SetCameraPosition(new Vector3(-1.0f, 1.0f, -3.0f));
    }

    override public void OnExitLevel()
    {
    }

    override public bool IsEndLevel()
    {
        return false;
    }

    override public int GetMissionTime()
    {
        return 0; 
    }

    override public void UpdateLevel()
    {
        if(_isFirstUpdate)
        {
            _isFirstUpdate = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelLobby : LevelBase
{
    public GameObject _textMissionTitle;
    public GameObject _textMissionBody;
    public GameObject _layerMissionDetail;
    public GameObject _imageMission;
    public Light _sun = null;
    public Material _skyBox = null;

    bool _isFirstUpdate = true;
    GameObject _levelPrefab = null;

    public void OnClickLevel(MissionInfo missionInfo)
    {
        GameObject levelPrefab = missionInfo.GetLevelPrefab();
        if(null != levelPrefab)
        {
            _layerMissionDetail.SetActive(true);

            _textMissionTitle.GetComponent<TextMeshProUGUI>().text = missionInfo.GetMissionTitle();
            _textMissionBody.GetComponent<TextMeshProUGUI>().text = missionInfo.GetMissionDetail();
            
            // set mission image
            _imageMission.GetComponent<Image>().sprite = missionInfo.GetImageMission();
            float width = _imageMission.GetComponent<Image>().sprite.rect.width;
            float height = _imageMission.GetComponent<Image>().sprite.rect.height;
            Vector2 sizeDelta =_imageMission.GetComponent<RectTransform>().sizeDelta;
            sizeDelta.x = sizeDelta.y * width / height;
            _imageMission.GetComponent<RectTransform>().sizeDelta = sizeDelta;

            _levelPrefab = levelPrefab;
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

     override public Light GetSun()
    {
        return _sun;
    }

    override public Material GetSkybox()
    {
        return _skyBox;
    }

    override public void OnStartLevel()
    {        
        GameManager.Instance.SetLevelStart();

        UIManager.Instance.SetVisibleControllerUI(false);

        Player.Instance.SetControllable(false);
        Player.Instance.SetInvincibility(true);

        MainCamera.Instance.SetCameraPosition(new Vector3(-1.0f, 1.0f, -3.0f));

        _layerMissionDetail.SetActive(false);
    }

    override public void OnExitLevel()
    {
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

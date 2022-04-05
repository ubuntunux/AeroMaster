using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MissionInfo : MonoBehaviour
{
    public GameObject _levelPrefab;
    public GameObject _missionTitle;
    [TextArea]
    public string _textMissionDetail;
    public Sprite _imageMission;

    public GameObject GetLevelPrefab()
    {
        return _levelPrefab;
    }

    public string GetMissionTitle()
    {
        return _missionTitle.GetComponent<TextMeshProUGUI>().text;
    }

    public string GetMissionDetail()
    {
        return _textMissionDetail;
    }

    public Sprite GetImageMission()
    {
        return _imageMission;
    }
}

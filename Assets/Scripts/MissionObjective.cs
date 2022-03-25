using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MissionObjectiveState
{
    None,
    Success,
    Failed,
    Count
};

public class MissionObjective : MonoBehaviour
{
    public GameObject _imageSuccess;
    public GameObject _imageFailed;
    public MissionObjectiveState _state = MissionObjectiveState.None;

    public void SetMissionObjective(GameObject parent, float posX, float posY, string missionText)
    {
        transform.SetParent(parent.transform, false);
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
        GetComponent<TextMeshProUGUI>().text = missionText;
        SetMissionObjectiveState(MissionObjectiveState.None);
    }

    public void SetMissionObjectiveState(MissionObjectiveState state)
    {
        if(MissionObjectiveState.Success == state)
        {
            GetComponent<TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
            _imageSuccess.SetActive(true);
            _imageFailed.SetActive(false);
        }
        else if(MissionObjectiveState.Failed == state)
        {
            GetComponent<TextMeshProUGUI>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            //GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            _imageSuccess.SetActive(false);
            _imageFailed.SetActive(true);
        }
        else
        {
            GetComponent<TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            _imageSuccess.SetActive(false);
            _imageFailed.SetActive(false);
        }

        _state = state;
    }
}

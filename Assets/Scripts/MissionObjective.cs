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
    string _missionText;
    bool _hasTimer = false;
    float _timer = 0.0f;

    public void SetMissionObjective(GameObject parent, float posX, float posY, string missionText, float timer = 0.0f)
    {
        transform.SetParent(parent.transform, false);
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
        GetComponent<TextMeshProUGUI>().text = missionText;        
        SetMissionObjectiveState(MissionObjectiveState.None);

        _missionText = missionText;
        _hasTimer = 0.0f < timer;
        _timer = timer;
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

    public bool IsMissionObjectiveTimeUp()
    {
        return _hasTimer && _timer <= 0.0f;
    }

    void Update()
    {
        if(_hasTimer)
        {
            int prevTime = (int)_timer;
            _timer = Mathf.Max(0.0f, _timer - Time.deltaTime);
            int currTime = (int)_timer;
            int minute = currTime / 60;
            int second = currTime % 60;
            bool warning = currTime < 10;
            bool timeChanged = prevTime != currTime;

            if(timeChanged)
            {
                GetComponent<TextMeshProUGUI>().text = string.Format("{0} ({1:D2}:{2:D2})", _missionText, minute, second);
            }

            if(warning)
            {
                if(timeChanged)
                {
                    AudioManager.Instance._audioWarnning.Play();
                }

                bool pingpong = (int)(_timer * 4.0f) % 2 == 0;
                if(pingpong)
                {
                    GetComponent<TextMeshProUGUI>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                }
                else
                {
                    GetComponent<TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
            }
        }
    }
}

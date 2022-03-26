using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionObjectiveManager : MonoBehaviour
{
    public GameObject _missionObjectiveLayer;
    public GameObject _missionObjectivePrefab;
    Dictionary<string, MissionObjective> _missionObjectiveMaps = new Dictionary<string, MissionObjective>();
    List<MissionObjective> _missionObjectives = new List<MissionObjective>();
    const float PADDING = 5.0f;

    float GetMissionObjectiveHeight()
    {
        return _missionObjectivePrefab.GetComponent<RectTransform>().rect.height;
    }

    public void ClearMissionObjectives()
    {
        foreach(MissionObjective missionObjective in _missionObjectives)
        {
            GameObject.Destroy(missionObjective.gameObject);
        }
        _missionObjectives.Clear();
        _missionObjectiveMaps.Clear();
        
        ResizeMissionObjectiveLayer();
    }

    public void RegistMissionObjective(string key, string missionText)
    {
        float height = GetMissionObjectiveHeight() * (float)_missionObjectives.Count + PADDING;
        MissionObjective missionObjective = Instantiate(_missionObjectivePrefab).GetComponent<MissionObjective>();
        missionObjective.SetMissionObjective(_missionObjectiveLayer, -15.0f, -height, missionText);
        
        // regist
        _missionObjectives.Add(missionObjective);
        _missionObjectiveMaps.Add(key, missionObjective);

        // resize
        ResizeMissionObjectiveLayer();
    }

    public void UnregistMissionObjective(string key)
    {
        MissionObjective missionObjective = null;
        if(_missionObjectiveMaps.TryGetValue(key, out missionObjective))
        {
            _missionObjectiveMaps.Remove(key);
            _missionObjectives.Remove(missionObjective);
        }
    }

    public void SetMissionObjectiveState(string key, MissionObjectiveState state)
    {
        MissionObjective missionObjective = null;
        if(_missionObjectiveMaps.TryGetValue(key, out missionObjective))
        {
            missionObjective.SetMissionObjectiveState(state);
        }
    }

    public void ResizeMissionObjectiveLayer()
    {
        float height = GetMissionObjectiveHeight();
        int count = _missionObjectives.Count;        
        Vector2 layerSizeDelta = _missionObjectiveLayer.GetComponent<RectTransform>().sizeDelta;
        layerSizeDelta.y = height * (float)count + PADDING * 2.0f;
        _missionObjectiveLayer.GetComponent<RectTransform>().sizeDelta = layerSizeDelta;
        _missionObjectiveLayer.SetActive(0 < count);
    }

    // Start is called before the first frame update
    void Start()
    {
        ResizeMissionObjectiveLayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

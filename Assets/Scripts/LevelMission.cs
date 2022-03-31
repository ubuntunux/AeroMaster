using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum MissionPhase
{
    None,
    MissionObjective,
    Complete,
    Exit,
    End
};

public class LevelMission : LevelBase
{
    public GameObject _start;
    public GameObject _goal;
    [TextArea]
    public string _textMissionTitle;
    [TextArea]
    public string _textMissionDetail;
    [TextArea]
    public string _textScripts;

    MissionPhase _phase = MissionPhase.None;
    float _exitTime = 0.0f;
    float _missionTime = 0.0f;
    bool _missionFailed = false;

    override public string GetMissionTitle()
    {
        return _textMissionTitle;
    }

    override public string GetMissionDetails()
    {
        return _textMissionDetail;
    }

    override public void OnStartLevel()
    {
        bool controllable = true;
        bool invincibility = false;
        GameManager.Instance.SetLevelStart(controllable, invincibility);

        UIManager.Instance.SetInteractableButtonAll(false);        
        ActorScriptManager.Instance.GenerateActorScriptsPages(_textScripts);

        _exitTime = 0.0f;
        _phase = MissionPhase.None;
    }

    override public void OnExitLevel()
    {
    }

    override public bool IsEndLevel()
    {
        return MissionPhase.End == _phase;
    }

    override public int GetMissionTime()
    {
        return (int)_missionTime; 
    }
    
    void SetPhaseComplete()
    {
        GameManager.Instance.SetLevelEnd();
        _phase = MissionPhase.Complete;
    }

    void SetGameOver()
    {
        GameManager.Instance.SetLevelEnd(false);
        _phase = MissionPhase.Exit;
    }

    override public void UpdateLevel()
    {
        // check mission failed
        if(false == _missionFailed)
        {
            _missionFailed = GameManager.Instance.CheckMissionRegion();
            if(_missionFailed)
            {
                SetGameOver();
            }
        }

        if(MissionPhase.None == _phase)
        {
            // first update
            if(UIManager.Instance.CheckSubjectTextDone())
            {
                if(ActorScriptManager.Instance.SetPageAndCheckReadDone("Intro"))
                {
                    UIManager.Instance.SetInteractableButtonAll(true);
                    _phase = MissionPhase.MissionObjective;
                }
            }
        }
        else if(MissionPhase.MissionObjective == _phase)
        {
            if(Player.Instance.isAlive())
            {
                Vector3 playerPosition = Player.Instance.GetPosition();
                if(LevelManager.Instance.GetGoalPosition().x <= playerPosition.x)
                {
                    SetPhaseComplete();
                }
            }
            else
            {
                SetGameOver();
            }
        }
        else if(MissionPhase.Complete == _phase)
        {
            _phase = MissionPhase.Exit;
        }
        else if(MissionPhase.Exit == _phase)
        {
            if(Constants.LEVEL_EXIT_TIME <= _exitTime)
            {
                _phase = MissionPhase.End;
            }
            _exitTime += Time.deltaTime;
        }
        
        _missionTime += Time.deltaTime;
    }
}
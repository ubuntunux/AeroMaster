using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum MissionPhase
{
    None,
    Intro,
    MissionObjective,
    Complete,
    Failed,
};

public class LevelMission : LevelBase
{
    [TextArea]
    public string _textMissionTitle;
    [TextArea]
    public string _textMissionDetail;
    [TextArea]
    public string _textScripts;

    MissionPhase _phase = MissionPhase.None;
    float _missionTime = 0.0f;

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
        GameManager.Instance.SetLevelStart();

        UIManager.Instance.SetVisibleControllerUI(false);

        // set scripts
        ActorScriptManager.Instance.GenerateActorScriptsPages(_textScripts);

        _phase = MissionPhase.Intro;
    }

    override public void OnExitLevel()
    {
    }

    override public int GetMissionTime()
    {
        return (int)_missionTime; 
    }
    
    void SetMissionComplete()
    {
        GameManager.Instance.SetLevelEnd(LevelEndTypes.MissionSucess);
        _phase = MissionPhase.Complete;
    }

    void SetMissionFailed()
    {
        GameManager.Instance.SetLevelEnd(LevelEndTypes.MissionFailed);
        _phase = MissionPhase.Failed;
    }

    override public void UpdateLevel()
    {
        // check mission failed
        if(MissionPhase.Failed != _phase)
        {
            bool isTimeUp = UIManager.Instance.IsMissionObjectiveTimeUp("Landing");
            if(false == GameManager.Instance.CheckMissionRegion() || 
               false == Player.Instance.isAlive() || 
               isTimeUp)
            {
                SetMissionFailed();
            }
        }

        if(MissionPhase.Intro == _phase)
        {
            if(UIManager.Instance.CheckSubjectTextDone())
            {
                if(ActorScriptManager.Instance.SetPageAndCheckReadDone("Intro"))
                {
                    UIManager.Instance.SetVisibleControllerUI(true);
                    // set mission objectives
                    UIManager.Instance.RegistMissionObjective("Landing", "F.O.S.A의 해상 본부에 착륙", 60.0f);
                    _phase = MissionPhase.MissionObjective;
                }
            }
        }
        else if(MissionPhase.MissionObjective == _phase)
        {
            Vector3 goalPoint = LevelManager.Instance.GetGoalPoint();
            if(Player.Instance.IsLanded() && Player.Instance.CheckIsInTargetRange(goalPoint, Constants.GOAL_IN_DISTANCE))
            {
                UIManager.Instance.SetMissionObjectiveState("Landing", MissionObjectiveState.Success);
                SetMissionComplete();
            }
        }
        
        _missionTime += Time.deltaTime;
    }
}
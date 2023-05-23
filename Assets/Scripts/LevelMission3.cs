using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class LevelMission3 : LevelBase
{
    enum MissionPhase
    {
        None,
        Intro,
        MissionObjective,
        Complete,
        Failed,
    };

    [TextArea]
    public string _textScripts;
    public Light _sun = null;
    public Material _skyBox = null;

    MissionPhase _phase = MissionPhase.None;
    float _missionTime = 0.0f;

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

        //CharacterManager.Instance.GetPlayer().SetControllable(false);
        //CharacterManager.Instance.GetPlayer().SetInvincibility(true);
        CharacterManager.Instance.GetPlayer().SetAutoFlyingDirection(true);
        CharacterManager.Instance.GetPlayer().SetAnimationState(AnimationState.Flying, true);

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
               false == CharacterManager.Instance.GetPlayer().IsAlive() || 
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
            if(CharacterManager.Instance.GetPlayer().IsLanded() && CharacterManager.Instance.GetPlayer().CheckIsInTargetRange(goalPoint, Constants.GOAL_IN_DISTANCE))
            {
                UIManager.Instance.SetMissionObjectiveState("Landing", MissionObjectiveState.Success);
                SetMissionComplete();
            }
        }
        
        _missionTime += Time.deltaTime;
    }
}
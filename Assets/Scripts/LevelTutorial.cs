using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TutorialPhase
{
    None,
    Intro,
    Briefing,
    Acceleration,
    TakeOff,
    Turn,
    Landing,
    Complete,
    Failed,
    End
};

public class LevelTutorial : LevelBase
{
    public GameObject _startMarker;
    [TextArea]
    public string _textMissionTitle;
    [TextArea]
    public string _textMissionDetail;
    [TextArea]
    public string _textScripts;

    TutorialPhase _phase = TutorialPhase.None;
    
    override public string GetMissionTitle()
    {
        return _textMissionTitle;
    }

    override public string GetMissionDetails()
    {
        return _textMissionDetail;
    }

    public void CallbackOnClickGoRight()
    {
        UIManager.Instance.SetMissionObjectiveState("Acceleration", MissionObjectiveState.Success);
        Player.Instance.SetAccleration(true);
        Player.Instance.SetCallbackOnClickGoRight(null);
        UIManager.Instance.SetInteractableGoRightButton(false);
        UIManager.Instance.SetFingerTarget(FingerTarget.None);
    }

    public void CallbackOnClickGoLeft()
    {
        UIManager.Instance.SetMissionObjectiveState("Turn", MissionObjectiveState.Success);
        Player.Instance.SetAccleration(false);
        Player.Instance.SetCallbackOnClickGoLeft(null);
        UIManager.Instance.SetInteractableGoLeftButton(false);
        UIManager.Instance.SetFingerTarget(FingerTarget.None);
    }

    public void CallbackOnClickLanding()
    {
        UIManager.Instance.SetMissionObjectiveState("Landing", MissionObjectiveState.Success);
        Player.Instance.SetCallbackOnClickLanding(null);
        Player.Instance.SetLanding();
        UIManager.Instance.SetInteractableLandingButton(false);
        UIManager.Instance.SetFingerTarget(FingerTarget.None);
    }

    void CallbackSetPhaseTurn()
    {
        Player.Instance.SetCallbackOnClickGoLeft(CallbackOnClickGoLeft);
        UIManager.Instance.SetInteractableButtonAll(false);
        UIManager.Instance.SetInteractableGoLeftButton(true);
        UIManager.Instance.SetFingerTarget(FingerTarget.GoLeft);        
    }

    void SetMissionComplete()
    {
        GameManager.Instance.SetLevelEnd(LevelEndTypes.MissionSucess);
        _phase = TutorialPhase.Complete;        
    }

    void SetMissionFailed()
    {
        GameManager.Instance.SetLevelEnd(LevelEndTypes.MissionFailed);
        _phase = TutorialPhase.Failed;
    }

    override public void OnStartLevel()
    {
        GameManager.Instance.SetLevelStart();

        Player.Instance.SetControllable(false);
        Player.Instance.SetInvincibility(true);
        
        UIManager.Instance.SetVisibleControllerUI(false);

        // Set Mission Objectives
        UIManager.Instance.RegistMissionObjective("Acceleration", "기체를 출발 시키세요.");
        UIManager.Instance.RegistMissionObjective("TakeOff", "기체를 이륙 시키세요.");
        UIManager.Instance.RegistMissionObjective("Turn", "기체를 왼쪽으로 선회 시키세요.");
        UIManager.Instance.RegistMissionObjective("Landing", "기체를 착륙 시키세요.");

        // set scripts
        ActorScriptManager.Instance.GenerateActorScriptsPages(_textScripts);

        _phase = TutorialPhase.None;
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
        // update mission states
        if(TutorialPhase.None == _phase)
        {
            UIManager.Instance.SetSubjectText(GetMissionTitle());
            _phase = TutorialPhase.Intro;
        }
        else if(TutorialPhase.Intro == _phase)
        {
            if(UIManager.Instance.CheckSubjectTextDone())
            {
                ActorScriptManager.Instance.SetPage("Intro");
                _phase = TutorialPhase.Briefing;
            }
        }
        else if(TutorialPhase.Briefing == _phase)
        {
            if(ActorScriptManager.Instance.CheckPageReadDone("Intro"))
            {
                // TutorialPhase.Acceleration
                UIManager.Instance.SetVisibleControllerUI(true);
                UIManager.Instance.SetInteractableButtonAll(false);
                UIManager.Instance.SetInteractableGoRightButton(true);
                UIManager.Instance.SetFingerTarget(FingerTarget.GoRight);
                Player.Instance.SetCallbackOnClickGoRight(CallbackOnClickGoRight);
                _phase = TutorialPhase.Acceleration;
            }
        }
        else if(TutorialPhase.Acceleration == _phase)
        {
            if(1.0f == Player.Instance.GetAbsVelocityRatioX())
            {
                if(ActorScriptManager.Instance.SetPageAndCheckReadDone("TakeOff"))
                {
                    // TutorialPhase.TakeOff
                    UIManager.Instance.SetInteractableButtonAll(false);
                    UIManager.Instance.SetFingerTarget(FingerTarget.VerticalVelocity);
                    _phase = TutorialPhase.TakeOff;
                }
            }
        }
        else if(TutorialPhase.TakeOff == _phase)
        {
            if(Player.Instance.GetAutoTakeOff())
            {
                // Check plane altitude
                const float TAKE_OFF_ALTITUDE = 1.0f;
                if(TAKE_OFF_ALTITUDE <= Player.Instance.GetAltitude())
                {
                    UIManager.Instance.SetMissionObjectiveState("TakeOff", MissionObjectiveState.Success);
                    Player.Instance.SetAutoTakeOff(false);
                    ActorScriptManager.Instance.SetPage("Turn", CallbackSetPhaseTurn);
                    _phase = TutorialPhase.Turn;
                }
            }
            else
            {
                Vector2 input = Vector2.zero;
                Player.Instance.GetInputDelta(ref input);
                if(0.3f < input.y)
                {
                    // Set auto take off
                    UIManager.Instance.SetFingerTarget(FingerTarget.None);
                    Player.Instance.SetAutoTakeOff(true);
                }
            }
        }
        else if(TutorialPhase.Turn == _phase)
        {
            if(Player.Instance.GetFrontDirection() <= -0.9f)
            {
                if(ActorScriptManager.Instance.SetPageAndCheckReadDone("Landing"))
                {
                    // TutorialPhase.Landing
                    Player.Instance.SetCallbackOnClickLanding(CallbackOnClickLanding);
                    UIManager.Instance.SetInteractableButtonAll(false);
                    UIManager.Instance.SetInteractableLandingButton(true);
                    UIManager.Instance.SetFingerTarget(FingerTarget.Landing);
                    _phase = TutorialPhase.Landing;
                }
            }
        }
        else if(TutorialPhase.Landing == _phase)
        {
            if(0.0f == Player.Instance.GetAbsVelocityRatioX() && Player.Instance.GetIsGround())
            {
                if(ActorScriptManager.Instance.SetPageAndCheckReadDone("Done"))
                {
                    SetMissionComplete();
                }
            }
        }

        // check mission failed
        if(TutorialPhase.Failed != _phase)
        {
            if(false == GameManager.Instance.CheckMissionRegion())
            {
                SetMissionFailed();
            }
        }
    }
}
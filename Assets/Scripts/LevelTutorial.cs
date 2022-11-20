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
    [TextArea]
    public string _textScripts;
    public Light _sun = null;
    public Material _skyBox = null;

    TutorialPhase _phase = TutorialPhase.None;

    public void CallbackOnClickGoRight()
    {
        UIManager.Instance.SetMissionObjectiveState("Acceleration", MissionObjectiveState.Success);
        CharacterManager.Instance.GetPlayer().SetAccleration(true);
        CharacterManager.Instance.GetPlayer().SetCallbackOnClickGoRight(null);
        UIManager.Instance.SetInteractableGoRightButton(false);
        UIManager.Instance.SetFingerTarget(FingerTarget.None);
    }

    public void CallbackOnClickGoLeft()
    {
        UIManager.Instance.SetMissionObjectiveState("Turn", MissionObjectiveState.Success);
        CharacterManager.Instance.GetPlayer().SetAccleration(false);
        CharacterManager.Instance.GetPlayer().SetCallbackOnClickGoLeft(null);
        UIManager.Instance.SetInteractableGoLeftButton(false);
        UIManager.Instance.SetFingerTarget(FingerTarget.None);
    }

    public void CallbackOnClickLanding()
    {
        UIManager.Instance.SetMissionObjectiveState("Landing", MissionObjectiveState.Success);
        CharacterManager.Instance.GetPlayer().SetCallbackOnClickLanding(null);
        CharacterManager.Instance.GetPlayer().SetLanding();
        UIManager.Instance.SetInteractableLandingButton(false);
        UIManager.Instance.SetFingerTarget(FingerTarget.None);
    }

    void CallbackSetPhaseTurn()
    {
        CharacterManager.Instance.GetPlayer().SetCallbackOnClickGoLeft(CallbackOnClickGoLeft);
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

        CharacterManager.Instance.GetPlayer().SetControllable(false);
        CharacterManager.Instance.GetPlayer().SetInvincibility(true);
        
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
            UIManager.Instance.SetSubjectText("RAVEN'S NEST BOOT CAMP");
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
                CharacterManager.Instance.GetPlayer().SetCallbackOnClickGoRight(CallbackOnClickGoRight);
                _phase = TutorialPhase.Acceleration;
            }
        }
        else if(TutorialPhase.Acceleration == _phase)
        {
            if(1.0f == CharacterManager.Instance.GetPlayer().GetAbsVelocityRatioX())
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
            if(CharacterManager.Instance.GetPlayer().GetAutoTakeOff())
            {
                // Check plane altitude
                const float TAKE_OFF_ALTITUDE = 1.0f;
                if(TAKE_OFF_ALTITUDE <= CharacterManager.Instance.GetPlayer().GetAltitude())
                {
                    UIManager.Instance.SetMissionObjectiveState("TakeOff", MissionObjectiveState.Success);
                    CharacterManager.Instance.GetPlayer().SetAutoTakeOff(false);
                    ActorScriptManager.Instance.SetPage("Turn", CallbackSetPhaseTurn);
                    _phase = TutorialPhase.Turn;
                }
            }
            else
            {
                Vector2 input = Vector2.zero;
                GameManager.Instance.GetInputDelta(ref input);
                if(0.3f < input.y)
                {
                    // Set auto take off
                    UIManager.Instance.SetFingerTarget(FingerTarget.None);
                    CharacterManager.Instance.GetPlayer().SetAutoTakeOff(true);
                }
            }
        }
        else if(TutorialPhase.Turn == _phase)
        {
            if(CharacterManager.Instance.GetPlayer().GetFrontDirection() <= -0.9f)
            {
                if(ActorScriptManager.Instance.SetPageAndCheckReadDone("Landing"))
                {
                    // TutorialPhase.Landing
                    CharacterManager.Instance.GetPlayer().SetCallbackOnClickLanding(CallbackOnClickLanding);
                    UIManager.Instance.SetInteractableButtonAll(false);
                    UIManager.Instance.SetInteractableLandingButton(true);
                    UIManager.Instance.SetFingerTarget(FingerTarget.Landing);
                    _phase = TutorialPhase.Landing;
                }
            }
        }
        else if(TutorialPhase.Landing == _phase)
        {
            if(0.0f == CharacterManager.Instance.GetPlayer().GetAbsVelocityRatioX() && CharacterManager.Instance.GetPlayer().GetIsGround())
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
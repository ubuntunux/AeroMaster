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
    Exit,
    End
};

public class LevelTutorial : LevelBase
{
    public GameObject _startMarker;
    public GameObject[] _regionMarkers;
    public GameObject _panelPause;
    public GameObject _textTutorial;
    [TextArea]
    public string _textMissionTitle;
    [TextArea]
    public string _textMissionDetail;
    [TextArea]
    public string _textScripts;

    TutorialPhase _phase = TutorialPhase.None;
    float _exitTime = 0.0f;
    Vector2 _region = Vector2.zero;
    bool _missionFailed = false;    

    override public string GetMissionTitle()
    {
        return _textMissionTitle;
    }

    override public string GetMissionDetails()
    {
        return _textMissionDetail;
    }

    public Vector3 GetStartPoint()
    {
        float heightHalf = _startMarker.GetComponent<MeshRenderer>().bounds.size.y * 0.5f;
        Vector3 position = _startMarker.transform.position;
        position.x -= 2.0f;
        position.y -= heightHalf;
        return position;
    }

    void UpdateRegions()
    {
        _region.x = float.MaxValue;
        _region.y = float.MinValue;

        for(int i = 0; i < _regionMarkers.Length; ++i)
        {
            if(_regionMarkers[i].transform.position.x < _region.x)
            {
                _region.x = _regionMarkers[i].transform.position.x;
            }

            if(_region.y < _regionMarkers[i].transform.position.x)
            {
                _region.y = _regionMarkers[i].transform.position.x;
            }
        }
    }

    public void CallbackOnClickGoRight()
    {
        Player.Instance.SetAccleration(true);
        Player.Instance.SetCallbackOnClickGoRight(null);
        UIManager.Instance.SetInteractableGoRightButton(false);
        UIManager.Instance.SetFingerTarget(FingerTarget.None);
        _textTutorial.SetActive(false);
    }

    public void CallbackOnClickGoLeft()
    {
        Player.Instance.SetAccleration(false);
        Player.Instance.SetCallbackOnClickGoLeft(null);
        UIManager.Instance.SetInteractableGoLeftButton(false);
        UIManager.Instance.SetFingerTarget(FingerTarget.None);
        _textTutorial.SetActive(false);
    }

    public void CallbackOnClickLanding()
    {
        Player.Instance.SetCallbackOnClickLanding(null);
        Player.Instance.SetLanding();
        UIManager.Instance.SetInteractableLandingButton(false);
        UIManager.Instance.SetFingerTarget(FingerTarget.None);
        _textTutorial.SetActive(false);
    }

    void CallbackSetPhaseTurn()
    {
        Player.Instance.SetCallbackOnClickGoLeft(CallbackOnClickGoLeft);
        UIManager.Instance.SetInteractableButtonAll(false);
        UIManager.Instance.SetInteractableGoLeftButton(true);
        UIManager.Instance.SetFingerTarget(FingerTarget.GoLeft);        
    }

    override public void OnStartLevel()
    {
        bool controllable = false;
        bool invincibility = true;
        GameManager.Instance.SetLevelStart(controllable, invincibility, GetStartPoint());
        
        UIManager.Instance.SetInteractableButtonAll(false);        
        ActorScriptManager.Instance.GenerateActorScriptsPages(_textScripts);

        UpdateRegions();

        _missionFailed = false;
        _exitTime = 0.0f;
        _panelPause.SetActive(false);
        _textTutorial.SetActive(false);
        _phase = TutorialPhase.None;
    }

    override public void OnExitLevel()
    {
        UIManager.Instance.SetInteractableButtonAll(true);
        UIManager.Instance.SetFingerTarget(FingerTarget.None);
    }

    override public bool IsEndLevel()
    {
        return TutorialPhase.End == _phase;
    }

    override public int GetMissionTime()
    {
        return 0; 
    }

    override public Vector2 GetMissionRegion()
    {
        return _region; 
    }
    
    override public void UpdateLevel()
    {
        // check mission failed
        if(false == _missionFailed)
        {
            _missionFailed = GameManager.Instance.CheckMissionRegion();
            if(_missionFailed)
            {
                _phase = TutorialPhase.Complete;
            }
        }

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
                // TutorialPhase.Complete                
                ActorScriptManager.Instance.SetPage("Done");
                _phase = TutorialPhase.Complete;
            }
        }
        else if(TutorialPhase.Complete == _phase)
        {
            // Mission complete or faild
            GameManager.Instance.SetLevelEnd(!_missionFailed);
            _phase = TutorialPhase.Exit;
        }
        else if(TutorialPhase.Exit == _phase)
        {
            if(Constants.LEVEL_EXIT_TIME <= _exitTime)
            {
                if(ActorScriptManager.Instance.CheckPageReadDone("Done"))
                {
                    _phase = TutorialPhase.End;
                }
            }
            else
            {
                _exitTime += Time.deltaTime;
            }
        }
    }
}
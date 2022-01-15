using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TutorialPhase
{
    None,
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
    public GameObject _start;
    public GameObject _panelPause;
    public GameObject _textTutorial;

    TutorialPhase _phase = TutorialPhase.None;
    float _exitTime = 0.0f;

    public Vector3 GetStartPoint()
    {
        float heightHalf = _start.GetComponent<MeshRenderer>().bounds.size.y * 0.5f;
        Vector3 position = _start.transform.position;
        position.x -= 2.0f;
        position.y -= heightHalf;
        return position;
    }

    public void CallbackOnClickGoRight()
    {
        Player.Instance.SetAccleration(true);
        Player.Instance.SetCallbackOnClickGoRight(null);
        UIManager.Instance.SetInteractableGoRightButton(false);
        _textTutorial.SetActive(false);
    }

    public void CallbackOnClickGoLeft()
    {
        Player.Instance.SetAccleration(false);
        Player.Instance.SetCallbackOnClickGoLeft(null);
        UIManager.Instance.SetInteractableGoLeftButton(false);
        _textTutorial.SetActive(false);
    }

    public void CallbackOnClickLanding()
    {
        Player.Instance.SetCallbackOnClickLanding(null);
        Player.Instance.SetLanding();        
        UIManager.Instance.SetInteractableLandingButton(false);
        _textTutorial.SetActive(false);
    }

    void SetPhaseAcceleration()
    {
        Player.Instance.SetCallbackOnClickGoRight(CallbackOnClickGoRight);        
        UIManager.Instance.SetInteractableButtonAll(false);
        UIManager.Instance.SetInteractableGoRightButton(true);
        _textTutorial.SetActive(true);
        _textTutorial.GetComponent<TextMeshProUGUI>().text = "Acceleration";
        _phase = TutorialPhase.Acceleration;
    }

    void SetPhaseTakeOff()
    {
        UIManager.Instance.SetInteractableButtonAll(false);
        _textTutorial.SetActive(true);
        _textTutorial.GetComponent<TextMeshProUGUI>().text = "Take Off";
        _phase = TutorialPhase.TakeOff;
    }

    void SetPhaseTurn()
    {
        Player.Instance.SetCallbackOnClickGoLeft(CallbackOnClickGoLeft);
        UIManager.Instance.SetInteractableButtonAll(false);
        UIManager.Instance.SetInteractableGoLeftButton(true);
        _textTutorial.SetActive(true);
        _textTutorial.GetComponent<TextMeshProUGUI>().text = "Turn";
        _phase = TutorialPhase.Turn;
    }

    void SetPhaseLanding()
    {
        Player.Instance.SetCallbackOnClickLanding(CallbackOnClickLanding);
        UIManager.Instance.SetInteractableButtonAll(false);
        UIManager.Instance.SetInteractableLandingButton(true);
        _textTutorial.SetActive(true);
        _textTutorial.GetComponent<TextMeshProUGUI>().text = "Landing";
        _phase = TutorialPhase.Landing;
    }

    void SetPhaseComplete()
    {
        _phase = TutorialPhase.Complete;
    }

    override public void OnStartLevel()
    {
        _exitTime = 0.0f;
        _panelPause.SetActive(false);
        _textTutorial.SetActive(false);
        _phase = TutorialPhase.None;

        bool controllable = false;
        bool invincibility = true;
        GameManager.Instance.ResetOnChangeLevel(controllable, invincibility, GetStartPoint());
    }

    override public void OnExitLevel()
    {
    }

    override public bool IsEndLevel()
    {
        return TutorialPhase.End == _phase;
    }

    override public void UpdateLevel()
    {
        if(TutorialPhase.None == _phase)
        {
            // first update
            SetPhaseAcceleration();
        }
        else if(TutorialPhase.Acceleration == _phase)
        {
            if(1.0f == Player.Instance.GetAbsVelocityRatioX())
            {
                SetPhaseTakeOff();
            }
        }
        else if(TutorialPhase.TakeOff == _phase)
        {
            if(Player.Instance.GetAutoTakeOff())
            {
                const float TAKE_OFF_ALTITUDE = 1.0f;
                if(TAKE_OFF_ALTITUDE <= Player.Instance.GetAltitude())
                {
                    Player.Instance.SetAutoTakeOff(false);
                    SetPhaseTurn();
                }
            }
            else
            {
                Vector2 input = Vector2.zero;
            #if UNITY_ANDROID
                input = GameManager.Instance.GetAltitudeTouchDelta();
            #elif UNITY_IPHONE
                // todo
            #else
                input.x = Input.GetAxis("Horizontal");
                input.y = Input.GetAxis("Vertical");
            #endif

                if(0.0f < input.y)
                {
                    // TakeOff
                    Player.Instance.SetAutoTakeOff(true);
                }
            }
        }
        else if(TutorialPhase.Turn == _phase)
        {
            if(Player.Instance.GetFrontDirection() <= -0.9f)
            {
                SetPhaseLanding();
            }
        }
        else if(TutorialPhase.Landing == _phase)
        {
            if(0.0f == Player.Instance.GetAbsVelocityRatioX() && Player.Instance.GetIsGround())
            {
                SetPhaseComplete();
            }
        }
        else if(TutorialPhase.Complete == _phase)
        {
            // Mission Complete
            GameManager.Instance.SetMissionComplete(true);
            _phase = TutorialPhase.Exit;
        }
        else if(TutorialPhase.Exit == _phase)
        {
            const float EXIT_TIME = 3.0f;
            if(EXIT_TIME <= _exitTime)
            {
                _phase = TutorialPhase.End;
            }
            _exitTime += Time.deltaTime;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum CutSceneTestPhase
{
    None,
    Description,
    Acceleration,
    TakeOff,
    Turn,
    Landing,
    Complete,
    Exit,
    End
};

public class LevelCutSceneTest : LevelBase
{
    public GameObject _start;
    public GameObject _panelPause;
    public GameObject _textTutorial;
    public GameObject _imageInside;
    public GameObject _imageOutside;
    BezierShakeObject _cameraHandMove = new BezierShakeObject();

    CutSceneTestPhase _phase = CutSceneTestPhase.None;
    float _exitTime = 0.0f;

    ActorScriptsPages _actorScriptsPages = new ActorScriptsPages();

    [TextArea]
    public string _textMissionTitle;
    [TextArea]
    public string _textMissionDetail;
    [TextArea]
    public string _textScripts;

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

    void SetPhaseAcceleration()
    {
        UIManager.Instance.SetFadeInOut(CallbackPhaseAcceleration);
        _phase = CutSceneTestPhase.Acceleration;
    }

    void CallbackPhaseAcceleration()
    {
        _imageInside.SetActive(false);
        _imageOutside.SetActive(false);
        Player.Instance.SetCallbackOnClickGoRight(CallbackOnClickGoRight);
        UIManager.Instance.SetVisibleControllerUI(true);
        UIManager.Instance.SetInteractableButtonAll(false);
        UIManager.Instance.SetInteractableGoRightButton(true);
        UIManager.Instance.SetFingerTarget(FingerTarget.GoRight);
        // _textTutorial.SetActive(true);
        // _textTutorial.GetComponent<TextMeshProUGUI>().text = "Acceleration";
    }

    void SetPhaseTakeOff()
    {
        UIManager.Instance.SetInteractableButtonAll(false);
        UIManager.Instance.SetFingerTarget(FingerTarget.VerticalVelocity);
        // _textTutorial.SetActive(true);
        // _textTutorial.GetComponent<TextMeshProUGUI>().text = "Take Off";
        _phase = CutSceneTestPhase.TakeOff;
    }

    void SetPhaseTurn()
    {
        Player.Instance.SetCallbackOnClickGoLeft(CallbackOnClickGoLeft);
        UIManager.Instance.SetInteractableButtonAll(false);
        UIManager.Instance.SetInteractableGoLeftButton(true);
        UIManager.Instance.SetFingerTarget(FingerTarget.GoLeft);
        // _textTutorial.SetActive(true);
        // _textTutorial.GetComponent<TextMeshProUGUI>().text = "Turn";
        _phase = CutSceneTestPhase.Turn;
    }

    void SetPhaseLanding()
    {
        Player.Instance.SetCallbackOnClickLanding(CallbackOnClickLanding);
        UIManager.Instance.SetInteractableButtonAll(false);
        UIManager.Instance.SetInteractableLandingButton(true);
        UIManager.Instance.SetFingerTarget(FingerTarget.Landing);
        // _textTutorial.SetActive(true);
        // _textTutorial.GetComponent<TextMeshProUGUI>().text = "Landing";
        _phase = CutSceneTestPhase.Landing;
    }

    void SetPhaseComplete()
    {
        _phase = CutSceneTestPhase.Complete;
    }

    override public void OnStartLevel()
    {
        bool controllable = false;
        bool invincibility = true;
        GameManager.Instance.SetLevelStart(controllable, invincibility, GetStartPoint());
        
        UIManager.Instance.SetInteractableButtonAll(false);
        UIManager.Instance.SetVisibleControllerUI(false);
        _actorScriptsPages.GenerateActorScriptsPages(_textScripts);

        _cameraHandMove.ResetShakeObject();
        _cameraHandMove.SetShake(0.0f, 10.0f, 10.0f);
        _imageInside.SetActive(true);
        _imageOutside.SetActive(true);

        _exitTime = 0.0f;
        _panelPause.SetActive(false);
        _textTutorial.SetActive(false);
        _phase = CutSceneTestPhase.None;
    }

    override public void OnExitLevel()
    {
        UIManager.Instance.SetInteractableButtonAll(true);
        UIManager.Instance.SetFingerTarget(FingerTarget.None);
    }

    override public bool IsEndLevel()
    {
        return CutSceneTestPhase.End == _phase;
    }

    override public int GetMissionTime()
    {
        return 0; 
    }
    
    override public void UpdateLevel()
    {
        // Image layer move for cutScene
        if(_imageInside.activeSelf)
        {
            Vector3 cameraOffset = Vector3.zero;
            _cameraHandMove.UpdateShakeObject(ref cameraOffset);
            _imageInside.GetComponent<RectTransform>().anchoredPosition = new Vector2(cameraOffset.x, cameraOffset.y);
            _imageOutside.GetComponent<RectTransform>().anchoredPosition = new Vector2(cameraOffset.x * 2.0f, cameraOffset.y * 2.0f);
        }

        if(CutSceneTestPhase.None == _phase)
        {
            // first update
            UIManager.Instance.SetSubjectText(GetMissionTitle());
            _phase = CutSceneTestPhase.Description;
        }
        else if(CutSceneTestPhase.Description == _phase)
        {
            if(UIManager.Instance.CheckSubjectTextDone())
            {
                if(_actorScriptsPages.CheckCurrentScriptReadDoneAndUpdateScript())
                {                    
                    SetPhaseAcceleration();
                }
            }
        }
        else if(CutSceneTestPhase.Acceleration == _phase)
        {
            if(1.0f == Player.Instance.GetAbsVelocityRatioX())
            {
                SetPhaseTakeOff();
            }
        }
        else if(CutSceneTestPhase.TakeOff == _phase)
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
                Player.Instance.GetInputDelta(ref input);
                if(0.0f < input.y)
                {
                    // TakeOff
                    Player.Instance.SetAutoTakeOff(true);
                }
            }
        }
        else if(CutSceneTestPhase.Turn == _phase)
        {
            if(Player.Instance.GetFrontDirection() <= -0.9f)
            {
                SetPhaseLanding();
            }
        }
        else if(CutSceneTestPhase.Landing == _phase)
        {
            if(0.0f == Player.Instance.GetAbsVelocityRatioX() && Player.Instance.GetIsGround())
            {
                SetPhaseComplete();
            }
        }
        else if(CutSceneTestPhase.Complete == _phase)
        {
            // Mission Complete
            GameManager.Instance.SetLevelEnd();
            _phase = CutSceneTestPhase.Exit;
        }
        else if(CutSceneTestPhase.Exit == _phase)
        {
            if(Constants.LEVEL_EXIT_TIME <= _exitTime)
            {
                _phase = CutSceneTestPhase.End;
            }
            _exitTime += Time.deltaTime;
        }
    }
}
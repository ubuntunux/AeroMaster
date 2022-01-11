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
    Complete
};

public class LevelTutorial : LevelBase
{
    public GameObject _start;
    public GameObject _goal;
    public GameObject _panelPause;
    public GameObject _textTutorial;

    private TutorialPhase _phase = TutorialPhase.None;

    // Start is called before the first frame update
    void Start()
    {   
    }

    override public Vector3 GetStartPoint()
    {
        float heightHalf = _start.GetComponent<MeshRenderer>().bounds.size.y * 0.5f;
        Vector3 position = _start.transform.position;
        position.x -= 2.0f;
        position.y -= heightHalf;
        return position;
    }

    override public Vector3 GetGoalPosition()
    {
        float heightHalf = _goal.GetComponent<MeshRenderer>().bounds.size.y * 0.5f;
        Vector3 position = _goal.transform.position;
        position.x -= 2.0f;
        position.y -= heightHalf;
        return position;
    }

    override public void Reset()
    {
        _phase = TutorialPhase.None;
        _panelPause.SetActive(false);
        _textTutorial.SetActive(false);
        Player.Instance._callbackOnClickGoRight = CallbackOnClickGoRight;
    }

    void SetPhaseAcceleration()
    {
        GameManager.Instance.SetPause(true);
        _phase = TutorialPhase.Acceleration;
        _panelPause.SetActive(true);
        _textTutorial.SetActive(true);
        _textTutorial.GetComponent<TextMeshProUGUI>().text = "Acceleration";
    }

    public void CallbackOnClickGoRight()
    {
        GameManager.Instance.SetPause(false);
        _panelPause.SetActive(false);
        _textTutorial.SetActive(false);
        _phase = TutorialPhase.TakeOff;
    }

    // Update is called once per frame
    void Update()
    {
        if(TutorialPhase.None == _phase)
        {
            SetPhaseAcceleration();
        }

        Vector3 goalPoint = GetGoalPosition();
        float playerPosX = Player.Instance.transform.position.x;        
        if(goalPoint.x <= playerPosX)
        {
            GameManager.Instance.SetMissionComplete(true);
        }
    }
}
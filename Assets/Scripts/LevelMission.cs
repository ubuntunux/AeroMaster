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

    MissionPhase _phase = MissionPhase.None;
    float _exitTime = 0.0f;
    float _missionTime = 0.0f;

    override public void OnStartLevel()
    {
        bool controllable = true;
        bool invincibility = false;
        GameManager.Instance.SetLevelStart(controllable, invincibility, GetStartPoint());

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

    public Vector3 GetStartPoint()
    {
        float heightHalf = _start.GetComponent<MeshRenderer>().bounds.size.y * 0.5f;
        Vector3 position = _start.transform.position;
        position.x -= 2.0f;
        position.y -= heightHalf;
        return position;
    }

    public Vector3 GetGoalPoint()
    {
        float heightHalf = _goal.GetComponent<MeshRenderer>().bounds.size.y * 0.5f;
        Vector3 position = _goal.transform.position;
        position.x -= 2.0f;
        position.y -= heightHalf;
        return position;
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
        if(MissionPhase.None == _phase)
        {
            _phase = MissionPhase.MissionObjective;
        }
        else if(MissionPhase.MissionObjective == _phase)
        {
            if(Player.Instance.isAlive())
            {
                //if(0.0f == Player.Instance.GetAbsVelocityRatioX() && Player.Instance.GetIsGround())
                Vector3 playerPosition = Player.Instance.GetPosition();
                if(GetGoalPoint().x <= playerPosition.x)
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
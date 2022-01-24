using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProfile : LevelBase
{
    bool _isFirstUpdate = true;

    public void OnClickStart()
    {
        LevelManager.Instance.GoToLevelLobby();
    }

    override public void OnStartLevel()
    {
        bool controllable = false;
        bool invincibility = true;
        float altitude = Constants.GROUND_HEIGHT + Constants.TAKE_OFF_HEIGHT + 1.0f;
        Vector3 startPosition = new Vector3(0.0f, altitude, 0.0f);
        bool isFlying = true;
        bool autoFlyingToRight = true;
        GameManager.Instance.SetLevelStart(controllable, invincibility, startPosition, isFlying, autoFlyingToRight);
    }

    override public void OnExitLevel()
    {
    }

    override public bool IsEndLevel()
    {
        return false;
    }

    override public int GetMissionTime()
    {
        return 0; 
    }

    override public void UpdateLevel()
    {
        if(_isFirstUpdate)
        {
            _isFirstUpdate = false;
        }
    }
}

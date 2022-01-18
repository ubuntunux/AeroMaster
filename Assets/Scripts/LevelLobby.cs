using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLobby : LevelBase
{
    bool _isFirstUpdate = true;

    public void OnClickTutorial()
    {
        LevelManager.Instance.StartTutorial();
    }

    public void OnClickMission()
    {
        LevelManager.Instance.StartMission();
    }

    override public void OnStartLevel()
    {
        bool controllable = false;
        bool invincibility = true;
        Vector3 startPosition = new Vector3(0.0f, 0.0f, 0.0f);
        GameManager.Instance.SetLevelStart(controllable, invincibility, startPosition);
        
        // Set Camera
        MainCamera.Instance.SetTrackingPlayer(false);
        MainCamera.Instance.SetCameraPosition(new Vector3(-1.0f, 1.0f, -3.0f));
        MainCamera.Instance.SetCameraHandMove(0.0f, 0.1f, 5.0f);
    }

    override public void OnExitLevel()
    {
    }

    override public bool IsEndLevel()
    {
        return false;
    }

    override public void UpdateLevel()
    {
        if(_isFirstUpdate)
        {
            _isFirstUpdate = false;
        }
    }
}

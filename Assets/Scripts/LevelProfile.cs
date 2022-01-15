using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProfile : LevelBase
{
    bool _isFirstUpdate = true;
    bool _backUpShowControllerUI = false;

    public void OnClickStart()
    {
        LevelManager.Instance.GoToLevelLobby();
    }

    override public void OnStartLevel()
    {
        bool controllable = false;
        bool invincibility = true;
        Vector3 startPosition = new Vector3(0.0f, 10.0f, 0.0f);
        bool isFlying = true;
        bool autoFlyingToRight = true;
        GameManager.Instance.ResetOnChangeLevel(controllable, invincibility, startPosition, isFlying, autoFlyingToRight);
        _backUpShowControllerUI = UIManager.Instance.GetVisibleControllerUI();
        UIManager.Instance.SetVisibleControllerUI(false);
    }

    override public void OnExitLevel()
    {
        UIManager.Instance.SetVisibleControllerUI(_backUpShowControllerUI);
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

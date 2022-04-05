using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProfile : LevelBase
{
    bool _isFirstUpdate = true;
    int _profileIndex = 0;

    public void OnClickProfile0()
    {
        OnClickProfile(0);
    }

    public void OnClickProfile1()
    {
        OnClickProfile(1);
    }

    public void OnClickProfile(int profileIndex)
    {
        _profileIndex = profileIndex;
        LevelManager.Instance.GoToLevelLobby();
    }

    override public void OnStartLevel()
    {
        GameManager.Instance.SetLevelStart();

        UIManager.Instance.SetVisibleControllerUI(false);
        UIManager.Instance.ShowMiniMap(false);

        Player.Instance.SetControllable(false);
        Player.Instance.SetInvincibility(true);
        Player.Instance.SetAutoFlyingDirection(true);
        Player.Instance.SetAnimationState(AnimationState.Flying);
    }

    override public void OnExitLevel()
    {
        Player.Instance.SetPlayerShipModel(_profileIndex);
        SaveData.Instance.Save(Constants.DefaultDataName);
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

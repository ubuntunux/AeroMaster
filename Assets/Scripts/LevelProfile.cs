using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProfile : LevelBase
{
    public Light _sun = null;
    public Material _skyBox = null;

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

        UIManager.Instance.SetVisibleControllerUI(false);

        CharacterManager.Instance.GetPlayer().SetControllable(false);
        CharacterManager.Instance.GetPlayer().SetInvincibility(true);
        CharacterManager.Instance.GetPlayer().SetAutoFlyingDirection(true);
        CharacterManager.Instance.GetPlayer().SetAnimationState(AnimationState.Flying);
    }

    override public void OnExitLevel()
    {
        CharacterManager.Instance.GetPlayer().SetPlayerShipModel(_profileIndex);
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

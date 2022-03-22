using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProfile : LevelBase
{
    bool _isFirstUpdate = true;
    int _profileIndex = 0;

    override public string GetMissionTitle()
    {
        return "";
    }

    override public string GetMissionDetails()
    {
        return "";
    }

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
        bool controllable = false;
        bool invincibility = true;
        float altitude = Constants.CLOUD_ALTITUDE;
        Vector3 startPosition = new Vector3(0.0f, altitude, 0.0f);
        bool isFlying = true;
        bool autoFlyingToRight = true;
        GameManager.Instance.SetLevelStart(controllable, invincibility, startPosition, isFlying, autoFlyingToRight);
    }

    override public void OnExitLevel()
    {
        Player.Instance.SetPlayerShipModel(_profileIndex);
        SaveData.Instance.Save(Constants.DefaultDataName);
    }

    override public bool IsEndLevel()
    {
        return false;
    }

    override public int GetMissionTime()
    {
        return 0; 
    }

    override public Vector2 GetMissionRegion()
    {
        return Vector2.zero; 
    }

    override public void UpdateLevel()
    {
        if(_isFirstUpdate)
        {
            _isFirstUpdate = false;
        }
    }
}

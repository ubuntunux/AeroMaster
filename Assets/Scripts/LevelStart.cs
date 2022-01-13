using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : LevelBase
{
    public void OnClickStart()
    {
        GameManager.Instance.StartGame();
    }

    override public void ResetLevel()
    {
        bool controllable = false;
        bool invincibility = true;
        GameManager.Instance.ResetOnChangeLevel(controllable, invincibility, Vector3.zero);
    }

    override public bool IsEndLevel()
    {
        return false;
    }

    override public void UpdateLevel()
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : AirCraftBase
{
    int _playerModelIndex = 0;
    
    public delegate void Callback();
    Callback _callbackOnClickGoRight = null;
    Callback _callbackOnClickGoLeft = null;
    Callback _callbackOnClickLanding = null;

    public override bool IsPlayer()
    {
        return true;
    }

    public void SetCallbackOnClickGoRight(Callback callback)
    {
        _callbackOnClickGoRight = callback;
    }

    public void SetCallbackOnClickGoLeft(Callback callback)
    {
        _callbackOnClickGoLeft = callback;
    }

    public void SetCallbackOnClickLanding(Callback callback)
    {
        _callbackOnClickLanding = callback;
    }

    public void OnClickGoRight()
    {
        if(null != _callbackOnClickGoRight)
        {
            _callbackOnClickGoRight();
        }

        if(GetControllable())
        {
            SetAccleration(true);
        }
    }

    public void OnClickGoLeft()
    {
        if(null != _callbackOnClickGoLeft)
        {
            _callbackOnClickGoLeft();
        }

        if(GetControllable())
        {
            SetAccleration(false);
        }
    }

    public void OnClickLanding()
    {
        if(null != _callbackOnClickLanding)
        {
            _callbackOnClickLanding();
        }

        if(GetControllable())
        {
            SetLanding();
        }
    }

    public void AddScore(int score)
    {
        SaveData.Instance._playerData._score += score;
    }

    public int GetPlayerShipModel()
    {
        return _playerModelIndex;
    }

    public void LoadPlayerData(PlayerData playerData)
    {
        SetPlayerShipModel(playerData._playerModelIndex);
    }
    
    public void SetPlayerShipModel(int index)
    {
        if(index < CharacterManager.Instance.GetCharacterModelCount())
        {
            _playerModelIndex = index;
            SaveData.Instance._playerData._playerModelIndex = index;
            LoadPlayerShipModel();
        }
    }

    public void LoadPlayerShipModel()
    {
        if(_playerModelIndex < CharacterManager.Instance.GetCharacterModelCount())
        {
            GameObject model = CharacterManager.Instance.CreateCharacterModel(_playerModelIndex);
            CreateMeshObject(model);
        }
    }
}

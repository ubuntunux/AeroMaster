using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : AirCraftUnit
{
    int _playerModelIndex = 0;
    bool _autoTakeOff = false;
    bool _controllable = false;
    
    public delegate void Callback();
    Callback _callbackOnClickGoRight = null;
    Callback _callbackOnClickGoLeft = null;
    Callback _callbackOnClickLanding = null;

    public override bool IsPlayer()
    {
        return true;
    }

    public bool GetControllable()
    {
        return _isAlive && _controllable && UIManager.Instance.GetVisibleControllerUI();
    }

    public void SetControllable(bool controllable)
    {
        _controllable = controllable;
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

    public bool GetAutoTakeOff()
    {
        return _autoTakeOff;
    }

    public void SetAutoTakeOff(bool autoTakeOff)
    {
        if(false == _isAlive)
        {
            return;
        }

        _autoTakeOff = autoTakeOff;
    }

    public void AddScore(int score)
    {
        SaveData.Instance._playerData._score += score;
    }

    public int GetUnitModelIndex()
    {
        return _playerModelIndex;
    }

    public void SetUnitModelIndex(int index)
    {
        if(index < CharacterManager.Instance.GetCharacterModelCount())
        {
            _playerModelIndex = index;
            SaveData.Instance._playerData._playerModelIndex = index;
            LoadUnitModel();
        }
    }

    public void LoadPlayerData(PlayerData playerData)
    {
        SetUnitModelIndex(playerData._playerModelIndex);
    }

    public void LoadUnitModel()
    {
        if(_playerModelIndex < CharacterManager.Instance.GetCharacterModelCount())
        {
            GameObject model = CharacterManager.Instance.CreateCharacterModel(_playerModelIndex);
            CreateModelObject(model);
        }
    }

    public void ResetPlayer(Vector3 startPoint)
    {
        ResetUnit();

        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);        
        SetPosition(startPoint);
        SetControllable(true);
        _autoTakeOff = false;
    }

    public override void UpdateControllerInput(ref Vector2 input)
    {
        if(_autoTakeOff)
        {
            input.y = 1.0f;
        }
        else if(GetControllable())
        {
            GameManager.Instance.GetInputDelta(ref input);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if("StarOrder" == other.gameObject.tag)
        {
            CharacterManager.Instance.GetPlayer().AddScore(1);
            other.gameObject.GetComponent<StarOrder>().GetStarOrder();
        }
    }
}

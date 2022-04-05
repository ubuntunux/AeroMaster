using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum DestroyType
{
    Explosion,
    ImpactWater
}

public enum AnimationState
{
    None,
    Idle,
    Flying,
    TakeOff,
    Landing
};

public class Player : MonoBehaviour
{
    public GameObject[] _meshObjects;
    int _playerModelIndex = 0;
    GameObject _meshObject;
    Animator _animator;
    AnimationState _animationState = AnimationState.None;
    float _landingGearRatio = 0.0f;

    public AudioSource _jetEngineStart;
    public AudioSource _jetEngineEnd;
    public AudioSource _flyingLoop;
    public AudioSource _radioLoop;
    public AudioSource _jetFlyby;
    public GameObject _destroyFX;
    public GameObject _impactWaterFX;
    public GameObject _sliderVerticalVelocity;

    public delegate void Callback();
    Callback _callbackOnClickGoRight = null;
    Callback _callbackOnClickGoLeft = null;
    Callback _callbackOnClickLanding = null;

    float _absVelocityX = 0.0f;    
    float _velocityY = 0.0f;
    float _absVelocityRatioX = 0.0f;
    float _absVelocityRatioY = 0.0f;
    float _inputY = 0.0f;
    bool _isAcceleration = false;
    bool _isLanding = true;
    bool _goalFrontDirectionFlag = true; 
    float _frontDirection = 1.0f;
    float _flyingTime = 0.0f;
    bool _isAlive = false;
    bool _isGround = false;
    bool _controllable = false;
    bool _autoTakeOff = false;
    bool _invincibility = false;

    // Singleton instantiation
    static Player _instance;
    public static Player Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.FindObjectOfType<Player>();
            }
            return _instance;
        }
    }

    public bool isAlive()
    {
        return _isAlive;
    }

    public bool GetIsGround()
    {
        return _isGround;
    }

    public float GetLandingGearRatio()
    {
        return _landingGearRatio;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public float GetAltitude()
    {
        return GetPosition().y - Constants.GROUND_HEIGHT;
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

    public void SetInvincibility(bool invincibility)
    {
        _invincibility = invincibility;
    }

    public void SetAutoFlyingDirection(bool isRightDirection)
    {
        if(false == _isAlive)
        {
            return;
        }

        _isAcceleration = true;
        _isLanding = false;        
        _absVelocityX = Constants.VELOCITY_LIMIT_X;
        _absVelocityRatioX = 1.0f;
        _goalFrontDirectionFlag = isRightDirection;
        _frontDirection = isRightDirection ? 1.0f : -1.0f;
    }

    public void SetAccleration(bool isRightDirection)
    {
        if(false == _isAlive)
        {
            return;
        }

        if(isRightDirection != _goalFrontDirectionFlag && (-1.0f == _frontDirection || 1.0f == _frontDirection))
        {
            _goalFrontDirectionFlag = isRightDirection;
            _jetFlyby.Play();
        }

        if(false == _isAcceleration)
        {
            _isAcceleration = true;
            _isLanding = false;
            _jetEngineStart.Play();
            _jetEngineEnd.Stop();
        }
    }

    public void SetLanding()
    {
        if(false == _isAlive)
        {
            return;
        }

        if(false == _isLanding)
        {
            _isAcceleration = false;
            _isLanding = true;
            _jetEngineStart.Stop();
            _jetEngineEnd.Play();
        }
    }

    public bool CheckIsInTargetRange(Vector3 targetPostion, float targetRadius)
    {
        return (targetPostion - GetPosition()).magnitude <= targetRadius;
    }

    public bool IsLanded()
    {
        return _isLanding && _isGround && 0.0f == _absVelocityX;
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

    public float GetInputY()
    {
        return _inputY;
    }

    public float GetVelocityRatio()
    {
        return Mathf.Min(1.0f, (_absVelocityX * _absVelocityX + _velocityY * _velocityY) / (Constants.VELOCITY_LIMIT_X * Constants.VELOCITY_LIMIT_X + Constants.VELOCITY_LIMIT_Y * Constants.VELOCITY_LIMIT_Y));
    }

    public float GetMaxVelocityRatio()
    {
        return Mathf.Max(_absVelocityRatioX, _absVelocityRatioY);
    }

    public float GetAbsVelocityRatioX()
    {
        return _absVelocityRatioX;
    }

    public float GetAbsVelocityRatioY()
    {
        return _absVelocityRatioY;
    }

    public float GetAbsVelocityX()
    {
        return _absVelocityX;
    }

    public float GetVelocityY()
    {
        return _velocityY;
    }

    public void SetVelocityY(float velocityY)
    {
        _velocityY = velocityY;
    }

    public float GetFrontDirection()
    {
        return _frontDirection;
    }

    public void SetFrontDirection(bool isRight)
    {
        _frontDirection = isRight ? 1.0f : -1.0f;
        _goalFrontDirectionFlag = isRight;
    }

    void SetVisible(bool show)
    {
        _meshObject.SetActive(show);
    }

    bool GetAfterBurnerEmission()
    {
        return _meshObject.GetComponent<PlayerShip>().GetAfterBurnerEmission();
    }

    void SetAfterBurnerEmission(bool emission)
    {
        _meshObject.GetComponent<PlayerShip>().SetAfterBurnerEmission(emission);
    }

    void StopAllSound()
    {
        _jetEngineStart.Stop();
        _jetEngineEnd.Stop();
        _flyingLoop.Stop();
        _radioLoop.Pause();
        _jetFlyby.Stop();
    }

    public void LoadPlayerData(PlayerData playerData)
    {
        SetPlayerShipModel(playerData._playerModelIndex);
    }

    public void ResetPlayer(Vector3 startPoint)
    {
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);        
        SetPosition(startPoint);
        
        SetControllable(true);
        SetInvincibility(false);
        SetVisible(true);
        SetAfterBurnerEmission(false);
        SetAnimationState(AnimationState.Idle);
        StopAllSound();

        _inputY = 0.0f;
        _flyingTime = 0.0f;
        _landingGearRatio = 0.0f;        
        _autoTakeOff = false;
        _absVelocityX = 0.0f;
        _absVelocityRatioX = 0.0f;
        _velocityY = 0.0f;
        _absVelocityRatioY = 0.0f;
        _isAcceleration = false;
        _isLanding = true;
        _goalFrontDirectionFlag = true;
        _frontDirection = 1.0f;
        _isGround = true;
        _isAlive = true;
    }

    // physics collide
    public void OnTriggerEnter(Collider other)
    {
        if("StarOrder" == other.gameObject.tag)
        {
            Player.Instance.AddScore(1);
            other.gameObject.GetComponent<StarOrder>().GetStarOrder();
        }
        else if("Wall" == other.gameObject.tag)
        {
            SetDestroy(DestroyType.Explosion);
        }
        else if("Water" == other.gameObject.tag)
        {
            SetDestroy(DestroyType.ImpactWater);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if("Ground" == other.gameObject.tag)
        {
            if(_velocityY < 0.0f)
            {
                if(_velocityY < Constants.SPEED_FOR_DESTROY || _landingGearRatio < 0.5f)
                {
                    SetDestroy(DestroyType.Explosion);
                }
                _velocityY = 0.0f;
            }

            _flyingTime = 0.0f;
            _isGround = true;            
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if("Ground" == other.gameObject.tag)
        {
            _isGround = false;
        }
    }

    //
    public void AddScore(int score)
    {
        SaveData.Instance._playerData._score += score;
    }

    //
    public int GetPlayerShipModel()
    {
        return _playerModelIndex;
    }
    
    public void SetPlayerShipModel(int index)
    {
        if(index < _meshObjects.Length && null != _meshObjects[index])
        {
            _playerModelIndex = index;
            SaveData.Instance._playerData._playerModelIndex = index;
            LoadPlayerShipModel();
        }
    }

    public void LoadPlayerShipModel()
    {
        if(_playerModelIndex < _meshObjects.Length)
        {
            if(null != _meshObject)
            {
                Destroy(_meshObject);
            }

            _meshObject = Instantiate(_meshObjects[_playerModelIndex]);            
            _meshObject.transform.SetParent(transform, false);
            _meshObject.transform.localPosition = Vector3.zero;
            _animator = _meshObject.GetComponent<Animator>();
        }
    }

    void UpdateAudios(float absVelocityRatioX)
    {
        float flyingAudioVolume = _isAlive ? absVelocityRatioX : 0.0f;
        if(false == _flyingLoop.isPlaying && 0.0f < flyingAudioVolume)
        {
            _flyingLoop.Play();
            _radioLoop.Play();
        }
        else if(_flyingLoop.isPlaying && 0.0f == flyingAudioVolume)
        {
            _flyingLoop.Stop();
            _radioLoop.Pause();
        }
        
        _flyingLoop.volume = flyingAudioVolume;
        _radioLoop.volume = flyingAudioVolume;
        _jetFlyby.volume = flyingAudioVolume;
    }

    void UpdateParticles(float absVelocityRatioX)
    {
        bool setActive = 0.0f != absVelocityRatioX && _isAlive;
        bool actived = GetAfterBurnerEmission();
        if(setActive && false == actived)
        {
            SetAfterBurnerEmission(true);
        }
        else if(false == setActive && actived)
        {
            SetAfterBurnerEmission(false);
        }
    }

    public bool GetControllable()
    {
        return _controllable && UIManager.Instance.GetVisibleControllerUI();
    }

    public void SetControllable(bool controllable)
    {
        _controllable = controllable;
    }

    public void SetForceDestroy()
    {
        _invincibility = false;
        SetDestroy(DestroyType.Explosion);
    }

    public void SetDestroy(DestroyType destroyType)
    {
        if(false == _invincibility && _isAlive)
        {
            MainCamera.Instance.SetCameraShakeByDestroy();
            
            GameObject destroyFX_Prefab = null;
            if(DestroyType.Explosion == destroyType)
            {
                destroyFX_Prefab = _destroyFX;
            }
            else if(DestroyType.ImpactWater == destroyType)
            {
                destroyFX_Prefab = _impactWaterFX;
            }

            if(null != destroyFX_Prefab)
            {
                GameObject destroyFX = (GameObject)GameObject.Instantiate(destroyFX_Prefab);
                destroyFX.transform.SetParent(transform, false);
            }
            
            SetVisible(false);
            SetControllable(false);
            StopAllSound();
            _isAlive = false;
        }
    }

    void ControllShip()
    {
        Vector2 input = Vector2.zero;

        // input
        if(_autoTakeOff)
        {
            input.y = 1.0f;
        }
        else if(GetControllable())
        {
            GameManager.Instance.GetInputDelta(ref input);
        }

        // limited altitude control
        if(Constants.LIMITED_ALTITUDE <= GetPosition().y && 0.0f < input.y)
        {
            input.y = 0.0f;
        }

        // make input smooth
        float inputYVelocity = (0.0f == input.y ? Constants.INPUT_Y_DAMPING : Constants.INPUT_Y_VELOCITY) * Time.deltaTime;
        if(_inputY < input.y)
        {
            _inputY = Mathf.Min(input.y, _inputY + inputYVelocity);
        }
        else if(input.y < _inputY)
        {
            _inputY = Mathf.Max(input.y, _inputY - inputYVelocity);
        }
    
        // clamp input
        if(1.0f < _inputY) _inputY = 1.0f;
        else if(_inputY < -1.0f) _inputY = -1.0f;

        // Acceleration
        if(_isAcceleration)
        {
            _absVelocityX = Mathf.Min(Constants.VELOCITY_LIMIT_X, _absVelocityX + Constants.ACCEL_X * Time.deltaTime);
        }

        // Landing
        if(_isLanding)
        {
            float damping = Constants.ACCEL_X * (_isGround ? 1.0f : 0.5f);
            _absVelocityX = Mathf.Max(0.0f, _absVelocityX - damping * Time.deltaTime);
        }
        _absVelocityRatioX = _absVelocityX / Constants.VELOCITY_LIMIT_X;
        float double_absVelocityRatioX = _absVelocityRatioX * _absVelocityRatioX;
        
        // Front direction
        _frontDirection += (_goalFrontDirectionFlag ? Time.deltaTime : -Time.deltaTime) * _absVelocityRatioX * Constants.TURN_SPEED;
        _frontDirection = Mathf.Min(1.0f, Mathf.Max(-1.0f, _frontDirection));

        // control vertical velocity
        if(0.0f != _inputY || (1.0f == _absVelocityRatioX && 0.0f != _velocityY))
        {
            float goalVelocity = Constants.VELOCITY_LIMIT_Y * double_absVelocityRatioX * _inputY;
            float velocityDiff = goalVelocity - _velocityY;
            if(0.0f != velocityDiff)
            {
                float velocityAccel = Mathf.Abs(Constants.ACCEL_Y * double_absVelocityRatioX * Time.deltaTime);
                velocityAccel = Mathf.Min(Mathf.Abs(velocityDiff), velocityAccel);
                _velocityY += (0.0f < velocityDiff) ? velocityAccel : -velocityAccel;
            }
        }

        // apply flying gravity
        if(_absVelocityRatioX < 1.0f && false == _isGround)
        {
            _velocityY -= Constants.GRAVITY * (1.0f - double_absVelocityRatioX) * Time.deltaTime;
        }

        _absVelocityRatioY = Mathf.Min(1.0f, Mathf.Max(0.0f, Mathf.Abs(_velocityY / Constants.VELOCITY_LIMIT_Y)));
    }

    public void SetAnimationState(AnimationState state)
    {
        if(state != _animationState)
        {
            switch(state)
            {
                case AnimationState.Flying:
                {
                    _animator.SetTrigger("Flying");
                } break;
                case AnimationState.TakeOff:
                {
                    _animator.SetTrigger("TakeOff");
                } break;
                case AnimationState.Landing:
                {
                    _animator.SetTrigger("Landing");
                } break;
                default:
                {
                    _animator.SetTrigger("Idle");
                } break;
            }
            _animationState = state;
        }
    }

    void UpdateAnimationController(Vector3 prevPosition, Vector3 position, bool isGround)
    {
        if(0 < _animator.layerCount)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            //AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(0);

            // update landing gear ratio
            if(AnimationState.Landing == _animationState)
            {
                _landingGearRatio = Mathf.Min(1.0f, (float)stateInfo.normalizedTime / (float)stateInfo.length);
            }
            else if(AnimationState.TakeOff == _animationState)
            {
                _landingGearRatio = 1.0f - Mathf.Min(1.0f, (float)stateInfo.normalizedTime / (float)stateInfo.length);
            }
            else
            {
                _landingGearRatio = (AnimationState.Flying == _animationState) ? 0.0f : 1.0f;
            }

            // update animation state
            if(AnimationState.None == _animationState)
            {
                SetAnimationState(isGround ? AnimationState.Idle : AnimationState.Flying);
            }
            else if(AnimationState.Flying == _animationState || AnimationState.TakeOff == _animationState)
            {
                if(_isLanding)
                {
                    SetAnimationState(AnimationState.Landing);
                }
                else if(AnimationState.TakeOff == _animationState && _landingGearRatio == 0.0f)
                {
                    SetAnimationState(AnimationState.Flying);
                }
            }
            else if(AnimationState.None == _animationState || AnimationState.Idle == _animationState || AnimationState.Landing == _animationState)
            {
                if(false == _isLanding && Constants.TAKE_OFF_FLYING_TIME <= _flyingTime)
                {
                    SetAnimationState(AnimationState.TakeOff);
                }
            }
        }
    }

    void Update()
    {
        if(_isAlive)
        {
            ControllShip();
        }
        else
        {
            _absVelocityX = Mathf.Max(0.0f, _absVelocityX - Constants.ACCEL_X * Time.deltaTime);
            _absVelocityRatioX = _absVelocityX / Constants.VELOCITY_LIMIT_X;
            if(false == _isGround)
            {
                _velocityY -= Constants.GRAVITY * Time.deltaTime;
            }
        }

        // check ground
        if(_isGround)
        {
            if(_velocityY <= 0.0f)
            {
                _velocityY = 0.0f;
            }            
        }
        else
        {
            _flyingTime += Time.deltaTime;
        }

        // apply velocity
        Vector3 position = transform.position;
        position.x += _absVelocityX * _frontDirection * Time.deltaTime;
        position.y += _velocityY * Time.deltaTime;

        UpdateAudios(_absVelocityRatioX);
        UpdateParticles(_absVelocityRatioX);
        UpdateAnimationController(transform.position, position, _isGround);

        // update transform
        float invGroundRatio = 1.0f - _landingGearRatio;// Mathf.Max(0.0f, Mathf.Min(1.0f, (position.y - Constants.GROUND_HEIGHT) * 0.2f));

        float velocityRatioY = Mathf.Max(-1.0f, Mathf.Min(1.0f, _inputY));
        float pitch = _absVelocityRatioX * velocityRatioY * invGroundRatio * 25.0f;
        float yaw = (_frontDirection * 0.5f + 0.5f) * (_goalFrontDirectionFlag ? -180.0f : 180.0f) + 180.0f;
        float roll = Mathf.Cos(_frontDirection * Mathf.PI * 0.5f) * 90.0f * invGroundRatio;
        transform.localRotation = Quaternion.Euler(roll, yaw, pitch);
        transform.position = position;
    }
}

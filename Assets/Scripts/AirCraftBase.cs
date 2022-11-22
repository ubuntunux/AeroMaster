using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum AnimationState
{
    None,
    Idle,
    Flying,
    TakeOff,
    Landing
};

public class AirCraftBase : UnitBase
{
    AnimationState _animationState = AnimationState.None;
    float _landingGearRatio = 0.0f;

    public AudioSource _jetEngineStart;
    public AudioSource _jetEngineEnd;
    public AudioSource _flyingLoop;
    public AudioSource _radioLoop;
    public AudioSource _jetFlyby;

    float _velocity_limit_x = Constants.VELOCITY_LIMIT_X;
    float _velocity_limit_y = Constants.VELOCITY_LIMIT_Y;

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

    public override UnitType GetUnitType()
    {
        return UnitType.AirCraft;
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

    public void SetAutoFlyingDirection(bool isRightDirection)
    {
        if(false == _isAlive)
        {
            return;
        }

        _isAcceleration = true;
        _isLanding = false;        
        _absVelocityX = _velocity_limit_x;
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

            if(IsPlayer())
            {
                AudioManager.Instance.PlayAudio(_jetFlyby);
            }
        }

        if(false == _isAcceleration)
        {
            _isAcceleration = true;
            _isLanding = false;

            if(IsPlayer())
            {
                AudioManager.Instance.PlayAudio(_jetEngineStart);
                AudioManager.Instance.StopAudio(_jetEngineEnd);
            }
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

            if(IsPlayer())
            {
                AudioManager.Instance.StopAudio(_jetEngineStart);
                AudioManager.Instance.PlayAudio(_jetEngineEnd);
            }
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

    public float GetInputY()
    {
        return _inputY;
    }

    public float GetVelocityRatio()
    {
        return Mathf.Min(1.0f, (_absVelocityX * _absVelocityX + _velocityY * _velocityY) / (_velocity_limit_x * _velocity_limit_x + _velocity_limit_y * _velocity_limit_y));
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

    bool GetAfterBurnerEmission()
    {
        return _modelObject.GetComponent<AirCraftModel>().GetAfterBurnerEmission();
    }

    void SetAfterBurnerEmission(bool emission)
    {
        _modelObject.GetComponent<AirCraftModel>().SetAfterBurnerEmission(emission);
    }

    public override void StopAllSound()
    {
        if(IsPlayer())
        {
            AudioManager.Instance.StopAudio(_jetEngineStart);
            AudioManager.Instance.StopAudio(_jetEngineEnd);
            AudioManager.Instance.StopAudio(_flyingLoop);
            AudioManager.Instance.StopAudio(_jetFlyby);
            AudioManager.Instance.PauseAudio(_radioLoop);
        }
    }

    public override void CreateModelObject(GameObject prefab)
    {
        base.CreateModelObject(prefab);

        AirCraftModel model =  _modelObject.GetComponent<AirCraftModel>();
        _velocity_limit_x = Constants.VELOCITY_LIMIT_X * model.GetSpeed();
        _velocity_limit_y = Constants.VELOCITY_LIMIT_Y * model.GetSpeed();
    }

    public override void ResetUnit()
    {   
        base.ResetUnit();

        SetAfterBurnerEmission(false);
        SetAnimationState(AnimationState.Idle);
        StopAllSound();

        _hpBar.GetComponent<HPBar>().InitializeHPBar(transform, SaveData.Instance._playerData._hp);
        _inputY = 0.0f;
        _flyingTime = 0.0f;
        _landingGearRatio = 0.0f;
        _absVelocityX = 0.0f;
        _absVelocityRatioX = 0.0f;
        _velocityY = 0.0f;
        _absVelocityRatioY = 0.0f;
        _isAcceleration = false;
        _isLanding = true;        
        _goalFrontDirectionFlag = true;
        _frontDirection = 1.0f;

        // Set FlyingState
        if(false == IsPlayer())
        {
            SetAfterBurnerEmission(true);
            SetAutoFlyingDirection(true);
            SetAnimationState(AnimationState.Flying);
        }
    }

    // physics collide
    public override void OnTriggerStay(Collider other)
    {
        if("Ground" == other.gameObject.tag)
        {
            bool damaged = false;

            if(_velocityY < 0.0f)
            {
                if(_velocityY < Constants.SPEED_FOR_DESTROY || _landingGearRatio < 0.5f)
                {
                    SetDamage(Mathf.Abs(_velocityY) * 10.0f + 10.0f);
                    damaged = true;
                }                

                _velocityY = (IsAlive() && damaged) ? (_velocityY * -0.5f) : 0.0f;
            }

            if(false == damaged && IsAlive())
            {
                _flyingTime = 0.0f;
                _isGround = true;
            }
        }
    }

    void UpdateAudios(float absVelocityRatioX)
    {
        if(IsPlayer())
        {
            float flyingAudioVolume = _isAlive ? absVelocityRatioX : 0.0f;
            if(false == AudioManager.Instance.IsPlayingAudio(_flyingLoop) && 0.0f < flyingAudioVolume)
            {
                AudioManager.Instance.PlayAudio(_flyingLoop);
                AudioManager.Instance.PlayAudio(_radioLoop);
            }
            else if(_flyingLoop.isPlaying && 0.0f == flyingAudioVolume)
            {
                AudioManager.Instance.StopAudio(_flyingLoop);
                AudioManager.Instance.PauseAudio(_radioLoop);
            }
            
            AudioManager.Instance.SetAudioVolume(_flyingLoop, flyingAudioVolume);
            AudioManager.Instance.SetAudioVolume(_radioLoop, flyingAudioVolume);
            AudioManager.Instance.SetAudioVolume(_jetFlyby, flyingAudioVolume);
        }
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

    public virtual void UpdateControllerInput(ref Vector2 input)
    {
        // NPC
        SetAccleration(true);
        //input.y = 1.0f;
    }

    void ControllShip()
    {
        Vector2 input = Vector2.zero;

        // input
        UpdateControllerInput(ref input);

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
            _absVelocityX = Mathf.Min(_velocity_limit_x, _absVelocityX + Constants.ACCEL_X * Time.deltaTime);
        }

        // Landing
        if(_isLanding)
        {
            float damping = Constants.ACCEL_X * (_isGround ? 1.0f : 0.5f);
            _absVelocityX = Mathf.Max(0.0f, _absVelocityX - damping * Time.deltaTime);
        }
        _absVelocityRatioX = _absVelocityX / _velocity_limit_x;
        float double_absVelocityRatioX = _absVelocityRatioX * _absVelocityRatioX;
        
        // Front direction
        _frontDirection += (_goalFrontDirectionFlag ? Time.deltaTime : -Time.deltaTime) * _absVelocityRatioX * Constants.TURN_SPEED;
        _frontDirection = Mathf.Min(1.0f, Mathf.Max(-1.0f, _frontDirection));

        // control vertical velocity
        if(false == _isLanding && (0.0f != _inputY || (1.0f == _absVelocityRatioX && 0.0f != _velocityY)))
        {
            float goalVelocity = _velocity_limit_y * double_absVelocityRatioX * _inputY;
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

        _absVelocityRatioY = Mathf.Min(1.0f, Mathf.Max(0.0f, Mathf.Abs(_velocityY / _velocity_limit_y)));
    }

    void FixedUpdate()
    {
        if(_isAlive)
        {
            ControllShip();
        }
        else
        {
            _absVelocityX = Mathf.Max(0.0f, _absVelocityX - Constants.ACCEL_X * Time.deltaTime);
            _absVelocityRatioX = _absVelocityX / _velocity_limit_x;
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

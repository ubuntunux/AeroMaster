using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public GameObject _meshObject;

    public AudioSource _jetEngineStart;
    public AudioSource _jetEngineEnd;
    public AudioSource _flyingLoop;
    public AudioSource _radioLoop;
    public AudioSource _jetFlyby;
    public GameObject _afterBurnerParticle;
    public GameObject _destroyFX;
    public GameObject _sliderVerticalVelocity;

    private static Player _instance;

    public delegate void Callback();
    private Callback _callbackOnClickGoRight = null;
    private Callback _callbackOnClickGoLeft = null;
    private Callback _callbackOnClickLanding = null;

    private float _absVelocityX = 0.0f;
    private float _absVelocityRatioX = 0.0f;
    private float _velocityY = 0.0f;
    private bool _isAcceleration = false;
    private bool _isLanding = true;
    private bool _goalFrontDirectionFlag = true; 
    private float _frontDirection = 1.0f;
    private bool _isAlive = false;
    private bool _isGround = false;
    private bool _controllable = false;
    private bool _autoTakeOff = false;
    private bool _invincibility = false;

    // Singleton instantiation
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

    public void SetAutoFlying(bool isRightDirection)
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

        if(_controllable)
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

        if(_controllable)
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

        if(_controllable)
        {
            SetLanding();
        }
    }

    public float GetAbsVelocityRatioX()
    {
        return _absVelocityRatioX;
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
        _meshObject.GetComponent<MeshRenderer>().enabled = show;
    }

    void SetAfterBurnerEmission(bool emission)
    {
        _afterBurnerParticle.GetComponent<ParticleScript>().SetEmission(emission);
    }

    void StopAllSound()
    {
        _jetEngineStart.Stop();
        _jetEngineEnd.Stop();
        _flyingLoop.Stop();
        _radioLoop.Pause();
        _jetFlyby.Stop();
    }

    public void ResetPlayer()
    {
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        SetControllable(true);
        SetInvincibility(false);
        SetVisible(true);
        SetAfterBurnerEmission(false);
        StopAllSound();

        _autoTakeOff = false;
        _absVelocityX = 0.0f;
        _absVelocityRatioX = 0.0f;
        _velocityY = 0.0f;
        _isAcceleration = false;
        _isLanding = true;
        _goalFrontDirectionFlag = true;
        _frontDirection = 1.0f;
        _isGround = true;
        _isAlive = true;
    }
    
    void Start()
    { 
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
        bool actived = _afterBurnerParticle.GetComponent<ParticleScript>().isEmission();
        if(setActive && false == actived)
        {
            SetAfterBurnerEmission(true);
        }
        else if(false == setActive && actived)
        {
            SetAfterBurnerEmission(false);
        }
    }

    public void SetControllable(bool controllable)
    {
        _controllable = controllable;
    }

    public void SetForceDestroy()
    {
        _invincibility = false;
        SetDestroy();
    }

    public void SetDestroy()
    {
        if(false == _invincibility && _isAlive)
        {
            MainCamera.Instance.SetCameraShakeByDestroy();
            GameObject destroyFX = (GameObject)GameObject.Instantiate(_destroyFX);
            destroyFX.transform.SetParent(transform, false);
            SetVisible(false);
            SetControllable(false);
            StopAllSound();
            _isAlive = false;
        }
    }

    void ControllShip()
    {
        Vector2 input = Vector2.zero;
        if(_controllable)
        {
        #if UNITY_ANDROID
            input = GameManager.Instance.GetAltitudeTouchDelta() * Constants.TOUCH_DELTA;
        #elif UNITY_IPHONE
            // todo
        #else
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
        #endif
        }

        if(_autoTakeOff)
        {
            input.y = 1.0f;
        }
    
        // clamp input
        if(1.0f < input.x) input.x = 1.0f;
        else if(input.x < -1.0f) input.x = -1.0f;        
        if(1.0f < input.y) input.y = 1.0f;
        else if(input.y < -1.0f) input.y = -1.0f;

        // apply to ui
        _sliderVerticalVelocity.GetComponent<Slider>().value = input.y;

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

        // Front direction
        _frontDirection += (_goalFrontDirectionFlag ? Time.deltaTime : -Time.deltaTime) * _absVelocityRatioX * Constants.TURN_SPEED;
        _frontDirection = Mathf.Min(1.0f, Mathf.Max(-1.0f, _frontDirection));

        // apply flying gravity
        if(_absVelocityRatioX < 1.0f && false == _isGround)
        {
            _velocityY -= Constants.GRAVITY * (1.0f - _absVelocityRatioX) * Time.deltaTime;
        }

        // control vertical velocity
        if(0.0f != input.y)
        {
            // flying
            _velocityY += Constants.ACCEL_Y * _absVelocityRatioX * input.y * Time.deltaTime;
            if(Constants.VELOCITY_LIMIT_Y < _velocityY)
            {
                _velocityY = Constants.VELOCITY_LIMIT_Y;
            }
        }
        else if(0.0f != _velocityY && false == _isLanding)
        {
            // maintain altitude
            float sign = Mathf.Sign(_velocityY);
            float absVelocityY = Mathf.Abs(_velocityY);

            absVelocityY -= Constants.DAMPING_Y * _absVelocityRatioX * Time.deltaTime;
            if(absVelocityY < 0.0f)
            {
                absVelocityY = 0.0f;
            }

            _velocityY = (sign < 0.0f) ? -absVelocityY : absVelocityY;
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
            _velocityY -= Constants.GRAVITY * Time.deltaTime;
        }

        // apply velocity
        Vector3 position = transform.position;
        position.x += _absVelocityX * _frontDirection * Time.deltaTime;
        position.y += _velocityY * Time.deltaTime;

        // check ground
        if(position.y <= Constants.GROUND_HEIGHT)
        {
            position.y = Constants.GROUND_HEIGHT;

            if(_velocityY < 0.0f)
            {
                if(_velocityY < Constants.SPEED_FOR_DESTROY)
                {
                    SetDestroy();
                }

                if(_isAlive)
                {
                    _velocityY = 0.0f;
                }
                else
                {
                    // bounce
                    _velocityY = -_velocityY * 0.5f;
                    if(_velocityY < 1.0f)
                    {
                        _velocityY = 0.0f;
                    }
                }
            }
        }
        _isGround = Constants.GROUND_HEIGHT == position.y && 0.0f == _velocityY;

        UpdateAudios(_absVelocityRatioX);
        UpdateParticles(_absVelocityRatioX);

        // update transform
        float invGroundRatio = Mathf.Max(0.0f, Mathf.Min(1.0f, (position.y - Constants.GROUND_HEIGHT) * 0.2f));
        float velocityRatioY = Mathf.Max(-1.0f, Mathf.Min(1.0f, _velocityY / Constants.VELOCITY_LIMIT_Y));
        float pitch = _absVelocityRatioX * velocityRatioY * invGroundRatio * 25.0f;
        float yaw = (_frontDirection * 0.5f + 0.5f) * (_goalFrontDirectionFlag ? -180.0f : 180.0f) + 180.0f;
        float roll = Mathf.Cos(_frontDirection * Mathf.PI * 0.5f) * 90.0f * invGroundRatio;
        transform.localRotation = Quaternion.Euler(roll, yaw, pitch);
        transform.position = position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public GameObject _textVelocityX;
    public GameObject _textVelocityY;
    public GameObject _meshObject;

    public AudioSource _jetEngineStart;
    public AudioSource _jetEngineEnd;
    public AudioSource _flyingLoop;
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
    private bool _isBreaking = true;
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

    public bool GetIsGround()
    {
        return _isGround;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
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
            _isBreaking = false;
            _jetEngineStart.Play();
            _jetEngineEnd.Stop();
        }
    }

    public void SetBreaking()
    {
        if(false == _isAlive)
        {
            return;
        }

        if(false == _isBreaking)
        {
            _isAcceleration = false;
            _isBreaking = true;
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

    public void OnClickBreak()
    {
        if(null != _callbackOnClickLanding)
        {
            _callbackOnClickLanding();
        }

        if(_controllable)
        {
            SetBreaking();
        }
    }

    public float GetAbsVelocityRatioX()
    {
        return _absVelocityRatioX;
    }

    public float GetFrontDirection()
    {
        return _frontDirection;
    }

    void SetVisible(bool show)
    {
        _meshObject.GetComponent<MeshRenderer>().enabled = show;
    }

    void SetAfterBurnerEmission(bool emission)
    {
        _afterBurnerParticle.GetComponent<ParticleScript>().SetEmission(emission);
    }

    public void ResetPlayer(Vector3 startPoint)
    {
        transform.position = startPoint;
        SetControllable(true);
        SetInvincibility(false);
        SetVisible(true);
        SetAfterBurnerEmission(false);
        _jetEngineStart.Stop();
        _jetEngineEnd.Stop();
        _flyingLoop.Stop();
        _jetFlyby.Stop();

        _autoTakeOff = false;
        _absVelocityX = 0.0f;
        _absVelocityRatioX = 0.0f;
        _velocityY = 0.0f;
        _isAcceleration = false;
        _isBreaking = true;
        _goalFrontDirectionFlag = true;
        _frontDirection = 1.0f;
        _isGround = true;
        _isAlive = true;
    }
    
    // Start is called before the first frame update
    void Start()
    { 
    }

    void UpdateAudios(float _absVelocityRatioX)
    {
        float flyingAudioVolume = _absVelocityRatioX;
        if(false == _flyingLoop.isPlaying && 0.0f < flyingAudioVolume)
        {
            _flyingLoop.Play();
        }
        else if(_flyingLoop.isPlaying && 0.0f == flyingAudioVolume)
        {
            _flyingLoop.Stop();
        }
        
        _flyingLoop.volume = flyingAudioVolume;
        _jetFlyby.volume = flyingAudioVolume;
    }

    void UpdateParticles(float _absVelocityRatioX)
    {
        bool setActive = 0.0f != _absVelocityRatioX && _isAlive;
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

    void SetDestroy()
    {
        if(false == _invincibility && _isAlive)
        {
            GameObject destroyFX = (GameObject)GameObject.Instantiate(_destroyFX);
            destroyFX.transform.SetParent(transform, false);
            SetVisible(false);
            SetControllable(false);
            _isAlive = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_isAlive)
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

            _sliderVerticalVelocity.GetComponent<Slider>().value = input.y;

            // Acceleration
            if(_isAcceleration)
            {
                _absVelocityX = Mathf.Min(Constants.VELOCITY_LIMIT_X, _absVelocityX + Constants.ACCEL_X * Time.deltaTime);
            }

            // Break
            if(_isBreaking)
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
            else if(0.0f != _velocityY && false == _isBreaking)
            {
                // maintain altitude
                float sign = Mathf.Sign(_velocityY);
                float velocityY = Mathf.Abs(_velocityY);

                velocityY -= Constants.DAMPING_Y * _absVelocityRatioX * Time.deltaTime;
                if(velocityY < 0.0f)
                {
                    velocityY = 0.0f;
                }
                else
                {
                    velocityY *= sign;
                }
                _velocityY = velocityY;
            }
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
        float pitch = _absVelocityRatioX * velocityRatioY * invGroundRatio * 35.0f;
        float yaw = (_frontDirection * 0.5f + 0.5f) * (_goalFrontDirectionFlag ? -180.0f : 180.0f) + 180.0f;
        float roll = Mathf.Cos(_frontDirection * Mathf.PI * 0.5f) * 90.0f * invGroundRatio;
        transform.localRotation = Quaternion.Euler(roll, yaw, pitch);
        transform.position = position;

        // update text
        _textVelocityX.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(_absVelocityX).ToString();
        _textVelocityY.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(_velocityY).ToString();
    }
}

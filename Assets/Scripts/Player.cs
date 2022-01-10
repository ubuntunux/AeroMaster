using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private static Player _instance;

    private float _absVelocityX = 0.0f;
    private float _absVelocityRatioX = 0.0f;
    private float _velocityY = 0.0f;
    private bool _isAcceleration = false;
    private bool _isBreaking = false;   
    private bool _goalFrontDirectionFlag = true; 
    private float _frontDirection = 1.0f;

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

    void SetAccleration(bool isRightDirection)
    {
        if(isRightDirection == _goalFrontDirectionFlag || 0.0f == _absVelocityRatioX)
        {
            if(false == _isAcceleration)
            {
                _isAcceleration = true;
                _isBreaking = false;
                _jetEngineStart.Play();
                _jetEngineEnd.Stop();
            }
        }
        else
        {
            if(0.0f != _absVelocityRatioX && (-1.0f == _frontDirection || 1.0f == _frontDirection))
            {
                _goalFrontDirectionFlag = isRightDirection;
                _jetFlyby.Play();
            }
        }
    }

    public void OnClickGoRight()
    {
        SetAccleration(true);
    }

    public void OnClickGoLeft()
    {
        SetAccleration(false);
    }

    public void OnClickBreak()
    {
        if(false == _isBreaking)
        {
            _isAcceleration = false;
            _isBreaking = true;
            _jetEngineStart.Stop();
            _jetEngineEnd.Play();
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

    public void ResetPlayer(Vector3 startPoint)
    {
        transform.position = startPoint;

        _absVelocityX = 0.0f;
        _absVelocityRatioX = 0.0f;
        _velocityY = 0.0f;
        _isAcceleration = false;
        _isBreaking = false;
        _goalFrontDirectionFlag = true;
        _frontDirection = 1.0f;
    }
    
    // Start is called before the first frame update
    void Start()
    { 
        _jetEngineStart.Stop();
        _jetEngineEnd.Stop();
        _flyingLoop.Stop();
        _jetFlyby.Stop();
        _afterBurnerParticle.GetComponent<ParticleScript>().SetEmission(false);
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
        _jetEngineStart.volume = 1.0f - flyingAudioVolume;
        _jetEngineEnd.volume = flyingAudioVolume;
    }

    void UpdateParticles(float _absVelocityRatioX)
    {
        bool setActive = 0.0f != _absVelocityRatioX;
        bool actived = _afterBurnerParticle.GetComponent<ParticleScript>().isEmission();
        if(setActive && false == actived)
        {
            _afterBurnerParticle.GetComponent<ParticleScript>().SetEmission(true);
        }
        else if(false == setActive && actived)
        {
            _afterBurnerParticle.GetComponent<ParticleScript>().SetEmission(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = Vector2.zero;        
        #if UNITY_ANDROID
            input = GameManager.Instance.GetAltitudeTouchDelta() * Constants.TOUCH_DELTA;
        #elif UNITY_IPHONE
            // todo
        #else
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
        #endif

        // clamp input
        if(1.0f < input.x) input.x = 1.0f;
        else if(input.x < -1.0f) input.x = -1.0f;        
        if(1.0f < input.y) input.y = 1.0f;
        else if(input.y < -1.0f) input.y = -1.0f;

        // Front direction
        _frontDirection += (_goalFrontDirectionFlag ? Time.deltaTime : -Time.deltaTime) * _absVelocityRatioX * Constants.TURN_SPEED;
        _frontDirection = Mathf.Min(1.0f, Mathf.Max(-1.0f, _frontDirection));

        // Acceleration
        if(_isAcceleration)
        {
            _absVelocityX = Mathf.Min(Constants.VELOCITY_LIMIT_X, _absVelocityX + Constants.ACCEL_X * Time.deltaTime);
        }

        // Break
        if(_isBreaking)
        {
            _absVelocityX = Mathf.Max(0.0f, _absVelocityX - Constants.ACCEL_X * Time.deltaTime);
        }
        _absVelocityRatioX = _absVelocityX / Constants.VELOCITY_LIMIT_X;

        // apply gravity
        if(_absVelocityRatioX < 1.0f)
        {
            _velocityY -= Constants.GRAVITY * (1.0f - _absVelocityRatioX) * Time.deltaTime;
        }

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

        // apply velocity
        Vector3 position = transform.position;
        position.x += _absVelocityX * _frontDirection * Time.deltaTime;
        position.y += _velocityY * Time.deltaTime;

        // check ground
        if(position.y < Constants.GROUND_HEIGHT)
        {
            position.y = Constants.GROUND_HEIGHT;

            if(_velocityY < 0.0f)
            {
                _velocityY = 0.0f;
            }
        }

        UpdateAudios(_absVelocityRatioX);
        UpdateParticles(_absVelocityRatioX);

        // update transform
        float invGroundRatio = Mathf.Max(0.0f, Mathf.Min(1.0f, (position.y - Constants.GROUND_HEIGHT) * 0.2f));
        float velocityRatioY = Mathf.Max(-1.0f, Mathf.Min(1.0f, _velocityY / Constants.VELOCITY_LIMIT_Y));
        float pitch = _absVelocityRatioX * velocityRatioY * invGroundRatio * 30.0f;
        float yaw = (_frontDirection * 0.5f + 0.5f) * (_goalFrontDirectionFlag ? -180.0f : 180.0f) + 180.0f;
        float roll = Mathf.Cos(_frontDirection * Mathf.PI * 0.5f) * 90.0f * invGroundRatio;
        transform.localRotation = Quaternion.Euler(roll, yaw, pitch);
        transform.position = position;

        // update text
        _textVelocityX.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(_absVelocityX).ToString();
        _textVelocityY.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(_velocityY).ToString();
    }
}

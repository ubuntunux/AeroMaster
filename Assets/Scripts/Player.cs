using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public GameObject _textVelocityX;
    public GameObject _textVelocityY;
    public GameObject _meshObject;

    private static Player _instance;

    private Vector2 _velocity = new Vector2(0.0f, 0.0f);
    private bool _isAcceleration = false;
    private bool _isBreaking = false;   
    private bool _frontDirectionFlag = true; 
    public float _velocityRatioX = 0.0f;
    public float _velocityRatioY = 0.0f;

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

    public void OnClickAcceleration()
    {
        _isAcceleration = true;
        _isBreaking = false;
    }

    public void OnReleaseAcceleration()
    {
        _isAcceleration = false;
    }

    public void OnClickBreak()
    {
        _isAcceleration = false;
        _isBreaking = true;
    }

    public void OnReleasekBreak()
    {
        _isBreaking = false;
    }

    public void OnClickTurn()
    {
        _frontDirectionFlag = !_frontDirectionFlag;
        _velocity.x = -_velocity.x;
    }

    public void ResetPlayer(Vector3 startPoint)
    {
        transform.position = startPoint;

        _velocity = new Vector2(0.0f, 0.0f);
        _isAcceleration = false;
        _isBreaking = false;
        _frontDirectionFlag = true;
    }
    
    // Start is called before the first frame update
    void Start()
    { 
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

        // Acceleration
        if(_isAcceleration)
        {
            float velocityX = Constants.ACCEL_X * Time.deltaTime;
            _velocity.x += _frontDirectionFlag ? velocityX : -velocityX;
            
            if(Constants.VELOCITY_LIMIT_X < Mathf.Abs(_velocity.x))
            {
                _velocity.x = Mathf.Sign(_velocity.x) * Constants.VELOCITY_LIMIT_X;
            }
        }

        // Break
        if(_isBreaking)
        {
            float damping = Constants.ACCEL_X * Time.deltaTime;
            float sign = Mathf.Sign(_velocity.x);
            float velocityX = Mathf.Abs(_velocity.x);
            if(velocityX <= damping)
            {
                _velocity.x = 0.0f;
            }
            else
            {
                _velocity.x = (velocityX - damping) * sign;
            }
        }

        // apply gravity
        float velocityRatioX = _velocity.x / Constants.VELOCITY_LIMIT_X;
        float absVelocityRatioX = Mathf.Min(1.0f, Mathf.Abs(_velocity.x) / Constants.VELOCITY_LIMIT_X);
        if(absVelocityRatioX < 1.0f)
        {
            _velocity.y -= Constants.GRAVITY * (1.0f - absVelocityRatioX) * Time.deltaTime;
        }

        if(0.0f != input.y)
        {
            // flying
            _velocity.y += Constants.ACCEL_Y * absVelocityRatioX * input.y * Time.deltaTime;

            if(Constants.VELOCITY_LIMIT_Y < _velocity.y)
            {
                _velocity.y = Constants.VELOCITY_LIMIT_Y;
            }
        }
        else if(0.0f != _velocity.y)
        {
            // maintain altitude
            float sign = Mathf.Sign(_velocity.y);
            float velocityY = Mathf.Abs(_velocity.y);

            velocityY -= Constants.DAMPING_Y * absVelocityRatioX * Time.deltaTime;
            if(velocityY < 0.0f)
            {
                velocityY = 0.0f;
            }
            else
            {
                velocityY *= sign;
            }
            _velocity.y = velocityY;
        }

        float velocityRatioY = _velocity.y / Constants.VELOCITY_LIMIT_Y;

        // apply velocity
        Vector3 position = transform.position;
        position.x += _velocity.x * Time.deltaTime;
        position.y += _velocity.y * Time.deltaTime;

        // check ground
        if(position.y < Constants.GROUND_HEIGHT)
        {
            position.y = Constants.GROUND_HEIGHT;

            if(_velocity.y < 0.0f)
            {
                _velocity.y = 0.0f;
            }
        }

        float pitch = absVelocityRatioX * velocityRatioY * 30.0f;
        float yaw = _frontDirectionFlag ? 0.0f : 180.0f;
        transform.rotation = Quaternion.Euler(0.0f, yaw, pitch);
        transform.position = position;

        _velocityRatioX = velocityRatioX;
        _velocityRatioY = velocityRatioY;

        // update text
        _textVelocityX.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(_velocity.x).ToString();
        _textVelocityY.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(_velocity.y).ToString();
    }
}

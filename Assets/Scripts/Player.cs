using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public GameObject _textVelocityX;
    public GameObject _textVelocityY;

    private static Player _instance;

    private Vector2 _velocity = new Vector2(0.0f, 0.0f);
    private bool _isAcceleration = false;
    private bool _isBreaking = false;   
    private bool _frontDirectionFlag = true; 

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
    }

    public void OnReleaseAcceleration()
    {
        _isAcceleration = false;
    }

    public void OnClickBreak()
    {
        _isBreaking = true;
    }

    public void OnReleaseBreak()
    {
        _isBreaking = false;
    }

    public void OnClickTurn()
    {
        _frontDirectionFlag = !_frontDirectionFlag;
        _velocity.x = -_velocity.x;
    }

    public void Reset(Vector3 startPoint)
    {
        float heightHalf = GetComponent<MeshRenderer>().bounds.size.y * 0.5f;
        startPoint.y += heightHalf;
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
            if(0 < Input.touchCount)
            {
                input = GameManager.Instance.GetAltitudeTouchDelta() * Constants.TOUCH_DELTA;
            }
        #elif UNITY_IPHONE
            // todo
        #else
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
        #endif

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
        float velocityRatioX = Mathf.Min(1.0f, Mathf.Abs(_velocity.x) / Constants.VELOCITY_LIMIT_X);
        if(velocityRatioX < 1.0f)
        {
            _velocity.y -= Constants.GRAVITY * (1.0f - velocityRatioX) * Time.deltaTime;
        }

        if(0.0f != input.y)
        {
            // flying
            _velocity.y += Constants.ACCEL_Y * input.y * Time.deltaTime;

            if(Constants.VELOCITY_LIMIT_Y < _velocity.y)
            {
                _velocity.y = Constants.VELOCITY_LIMIT_Y;
            }
        }
        else if(0.0f != _velocity.y && 1.0f <= velocityRatioX)
        {
            // maintain altitude
            float sign = Mathf.Sign(_velocity.y);
            float velocityY = Mathf.Abs(_velocity.y);

            velocityY -= Constants.DAMPING_Y * Time.deltaTime;
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

        //transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        transform.localScale = new Vector3(_frontDirectionFlag ? 2.0f : -2.0f, 1.0f, 1.0f);
        transform.position = position;

        // update text
        _textVelocityX.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(_velocity.x).ToString();
        _textVelocityY.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(_velocity.y).ToString();
    }
}

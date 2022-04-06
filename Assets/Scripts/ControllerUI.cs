using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ControllerUI : MonoBehaviour
{
    public GameObject _controlAccelLeft;
    public GameObject _controlAccelRight;
    public GameObject _controlLanding;
    public GameObject _controlVerticalVelocity;
    public GameObject _controlVerticalVelocityTop;
    public GameObject _controlVerticalVelocityBottom;
    
    public Sprite _top_on;
    public Sprite _top_off;
    public Sprite _bottom_on;
    public Sprite _bottom_off;

    public Sprite _accel_left_on;
    public Sprite _accel_left_off;
    public Sprite _accel_right_on;
    public Sprite _accel_right_off;
    public Sprite _landing_on;
    public Sprite _landing_off;

    float _prevVerticalVelocity = 0.0f;

    // Singleton instantiation
    private static ControllerUI _instance;
    public static ControllerUI Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.FindObjectOfType<ControllerUI>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    public void OnClickAccelLeft()
    {
        _controlAccelLeft.GetComponent<Image>().sprite = _accel_left_on;
        _controlAccelRight.GetComponent<Image>().sprite = _accel_right_off;
        _controlLanding.GetComponent<Image>().sprite = _landing_off;

        Player.Instance.OnClickGoLeft();
    }

    public void OnClickAccelRight()
    {
        _controlAccelLeft.GetComponent<Image>().sprite = _accel_left_off;
        _controlAccelRight.GetComponent<Image>().sprite = _accel_right_on;
        _controlLanding.GetComponent<Image>().sprite = _landing_off;

        Player.Instance.OnClickGoRight();
    }

    public void OnClickLanding()
    {
        _controlAccelLeft.GetComponent<Image>().sprite = _accel_left_off;
        _controlAccelRight.GetComponent<Image>().sprite = _accel_right_off;
        _controlLanding.GetComponent<Image>().sprite = _landing_on;

        Player.Instance.OnClickLanding();
    }

    public void ResetControllerUI()
    {
        _controlAccelLeft.GetComponent<Image>().sprite = _accel_left_off;
        _controlAccelRight.GetComponent<Image>().sprite = _accel_right_off;
        _controlLanding.GetComponent<Image>().sprite = _landing_off;
        _controlVerticalVelocityTop.GetComponent<Image>().sprite = _top_off;
        _controlVerticalVelocityBottom.GetComponent<Image>().sprite = _bottom_off;
        _controlVerticalVelocity.transform.localPosition = Vector3.zero;
        _prevVerticalVelocity = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(LevelManager.Instance.IsMissionLevel())
        {
            Vector2 input = Vector2.zero;
            GameManager.Instance.GetInputDelta(ref input);

        #if UNITY_ANDROID            
            // nothing
        #elif UNITY_IPHONE
            // nothing
        #else
            if(input.x < 0.0f)
            {
                OnClickAccelLeft();
            }
            else if(0.0f < input.x)
            {
                OnClickAccelRight();
            }
            else if(Input.GetButtonDown("Jump"))
            {
                OnClickLanding();
            }
        #endif

            // vertical speed
            float verticalVelocity = Player.Instance.GetInputY() * 55.0f;
            _controlVerticalVelocity.transform.localPosition = new Vector3(0.0f, verticalVelocity, 0.0f);

            if(0.0f < verticalVelocity && _prevVerticalVelocity <= verticalVelocity)
            {
                _controlVerticalVelocityTop.GetComponent<Image>().sprite = _top_on;
                _controlVerticalVelocityBottom.GetComponent<Image>().sprite = _bottom_off;
            }
            else if(verticalVelocity < 0.0f && verticalVelocity <= _prevVerticalVelocity)
            {
                _controlVerticalVelocityTop.GetComponent<Image>().sprite = _top_off;
                _controlVerticalVelocityBottom.GetComponent<Image>().sprite = _bottom_on;
            }
            else
            {
                _controlVerticalVelocityTop.GetComponent<Image>().sprite = _top_off;
                _controlVerticalVelocityBottom.GetComponent<Image>().sprite = _bottom_off;
            }

            _prevVerticalVelocity = verticalVelocity;

        }
    }
}

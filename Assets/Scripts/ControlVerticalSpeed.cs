using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ControlVerticalSpeed : MonoBehaviour
{
    public GameObject _controlVerticalVelocity;
    public GameObject _controlVerticalVelocityTop;
    public GameObject _controlVerticalVelocityBottom;
    
    public Sprite _top_on;
    public Sprite _top_off;
    public Sprite _bottom_on;
    public Sprite _bottom_off;

    float _prevVerticalVelocity = 0.0f;

    // Update is called once per frame
    void Update()
    {
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

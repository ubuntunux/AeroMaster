using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarOrder : MonoBehaviour
{
    public bool _enableRotate = false;

    float _initialScale = 1.0f;
    bool _disappear = false;
    float _disappearTimer = 0.0f;
    const float DISAPPEAR_TIME = 1.0f;

    public void GetStarOrder()
    {
        AudioManager.Instance._audioStarOrder.Play();
        _disappearTimer = DISAPPEAR_TIME;
        _disappear = true;        
    }

    void Start()
    {
        _initialScale = transform.localScale.x;
    }

    void Update()
    {
        if(_enableRotate)
        {
            transform.rotation = Quaternion.Euler(0.0f, (Time.time % 1.0f) * 360.0f, 0.0f);
        }

        if(_disappear)
        {
            _disappearTimer -= Time.deltaTime;
            if(_disappearTimer <= 0.0f)
            {
                Destroy(gameObject);
            }
            else
            {
                float scale = _disappearTimer / DISAPPEAR_TIME;
                scale = scale * scale * _initialScale * 1.5f;                
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}

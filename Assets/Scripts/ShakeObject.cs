using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearShakeObject
{
    bool _enableShake = false;
    Vector3 _shakeOffset0;
    Vector3 _shakeOffset1;
    float _shakeDuration = 0.0f;
    float _shakeTime = 0.0f;    
    float _shakeRandomTerm = 0.01f;
    float _shakeRandomTime = 0.0f;
    float _shakeIntensity = 1.0f;

    public void ResetShakeObject()
    {
        SetShakeEnable(false);
        _shakeOffset0 = new Vector3(0.0f, 0.0f, 0.0f);
        _shakeOffset1 = new Vector3(0.0f, 0.0f, 0.0f);
        _shakeDuration = 0.0f;
        _shakeTime = 0.0f;
        _shakeRandomTerm = 0.01f;
        _shakeRandomTime = 0.0f;
    }

    public void SetShakeEnable(bool enableShake)
    {
        _enableShake = enableShake;
    }

    public void SetShake(float shakeDuration, float shakeIntensity, float shakeRandomTerm)
    {
        SetShakeEnable(true);
        _shakeDuration = shakeDuration;
        _shakeTime = shakeDuration;
        _shakeIntensity = shakeIntensity;
        _shakeRandomTerm = shakeRandomTerm;
        _shakeRandomTime = 0.0f;
        _shakeOffset0 = new Vector3(0.0f, 0.0f, 0.0f);
        _shakeOffset1 = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void UpdateShakeObject(ref Vector3 outShakeOffset, float intesntiy = 1.0f)
    {
        if(_enableShake && 0.0f != _shakeIntensity)
        {
            bool isLoop = _shakeDuration <= 0.0f;
            if (isLoop || 0.0f < _shakeTime)
            {
                if(_shakeRandomTime <= 0.0f)
                {
                    _shakeOffset1 = _shakeOffset0;
                    _shakeOffset0 = (Vector3)Random.insideUnitCircle * _shakeIntensity;
                    _shakeRandomTime = _shakeRandomTerm;
                }

                float shakeIntensityByTime = isLoop ? 1.0f : (_shakeTime / _shakeDuration);
                float t = (0.0f == _shakeRandomTerm) ? 1.0f : (1.0f - _shakeRandomTime / _shakeRandomTerm);
                outShakeOffset += Vector3.Lerp(_shakeOffset1, _shakeOffset0, t) * shakeIntensityByTime * intesntiy;
                _shakeRandomTime -= Time.deltaTime;

                if(false == isLoop)
                {
                    _shakeTime -= Time.deltaTime;
                }
            }
        }
    }
}

public class BezierShakeObject
{
    bool _enableShake = false;
    Vector3 _shakeOffset0;
    Vector3 _shakeOffset1;
    Vector3 _shakeOffset2;
    Vector3 _shakeOffset3;
    float _shakeDuration = 0.0f;
    float _shakeTime = 0.0f;    
    float _shakeRandomTerm = 0.01f;
    float _shakeRandomTime = 0.0f;
    float _shakeIntensity = 1.0f;

    public void ResetShakeObject()
    {
        SetShakeEnable(false);
        _shakeOffset0 = new Vector3(0.0f, 0.0f, 0.0f);
        _shakeOffset1 = new Vector3(0.0f, 0.0f, 0.0f);
        _shakeOffset2 = new Vector3(0.0f, 0.0f, 0.0f);
        _shakeOffset3 = new Vector3(0.0f, 0.0f, 0.0f);
        _shakeDuration = 0.0f;
        _shakeTime = 0.0f;
        _shakeRandomTerm = 0.01f;
        _shakeRandomTime = 0.0f;
    }

    public void SetShakeEnable(bool enableShake)
    {
        _enableShake = enableShake;
    }

    public void SetShake(float shakeDuration, float shakeIntensity, float shakeRandomTerm)
    {
        SetShakeEnable(true);
        _shakeDuration = shakeDuration;
        _shakeTime = shakeDuration;
        _shakeIntensity = shakeIntensity;
        _shakeRandomTerm = shakeRandomTerm;
        _shakeRandomTime = 0.0f;
        _shakeOffset0 = new Vector3(0.0f, 0.0f, 0.0f);
        _shakeOffset1 = (Vector3)Random.insideUnitCircle * shakeIntensity;
        _shakeOffset2 = (Vector3)Random.insideUnitCircle * shakeIntensity;
        _shakeOffset3 = (Vector3)Random.insideUnitCircle * shakeIntensity;
    }

    public void UpdateShakeObject(ref Vector3 outShakeOffset, float intesntiy = 1.0f)
    {
        if(_enableShake && 0.0f != _shakeIntensity)
        {
            bool isLoop = _shakeDuration <= 0.0f;
            if (isLoop || 0.0f < _shakeTime)
            {
                if(_shakeRandomTime <= 0.0f)
                {
                    _shakeOffset0 = _shakeOffset3;
                    _shakeOffset1 = _shakeOffset3 + _shakeOffset3 - _shakeOffset2;
                    _shakeOffset2 = (Vector3)Random.insideUnitCircle * _shakeIntensity;
                    _shakeOffset3 = (Vector3)Random.insideUnitCircle * _shakeIntensity;
                    _shakeRandomTime = _shakeRandomTerm;
                }

                float shakeIntensityByTime = isLoop ? 1.0f : (_shakeTime / _shakeDuration);
                float t = (0.0f == _shakeRandomTerm) ? 1.0f : (1.0f - _shakeRandomTime / _shakeRandomTerm);                
                float invt = 1.0f - t;
                float t0 = invt * invt * invt;
                float t1 = 3.0f * invt * invt * t;
                float t2 = 3.0f  * invt * t * t;
                float t3 = t * t * t;
                outShakeOffset += (_shakeOffset0 * t0 + _shakeOffset1 * t1 + _shakeOffset2 * t2 + _shakeOffset3 * t3) * shakeIntensityByTime * intesntiy;
                _shakeRandomTime -= Time.deltaTime;

                if(false == isLoop)
                {
                    _shakeTime -= Time.deltaTime;
                }
            }
        }
    }
}

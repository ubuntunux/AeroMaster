using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject
{
    bool _enableShake = false;
    Vector3 _shakeOffset;
    Vector3 _prevShakeOffset;
    float _shakeDuration = 0.0f;
    float _shakeTime = 0.0f;    
    float _shakeRandomTerm = 0.01f;
    float _shakeRandomTime = 0.0f;
    float _shakeIntensity = 1.0f;

    public void ResetShakeObject()
    {
        _enableShake = false;
        _shakeOffset = new Vector3(0.0f, 0.0f, 0.0f);
        _prevShakeOffset = new Vector3(0.0f, 0.0f, 0.0f);
        _shakeDuration = 0.0f;
        _shakeTime = 0.0f;
        _shakeRandomTerm = 0.01f;
        _shakeRandomTime = 0.0f;
    }

    public void SetShake(float shakeDuration, float shakeIntensity, float shakeRandomTerm)
    {
        _enableShake = true;
        _shakeDuration = shakeDuration;
        _shakeTime = shakeDuration;
        _shakeIntensity = shakeIntensity;
        _shakeRandomTerm = shakeRandomTerm;
        _shakeRandomTime = 0.0f;
        _shakeOffset = new Vector3(0.0f, 0.0f, 0.0f);
        _prevShakeOffset = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void UpdateShakeObject(ref Vector3 outShakeOffset)
    {
        if(_enableShake)
        {
            bool isLoop = _shakeDuration <= 0.0f;
            if (isLoop || 0.0f < _shakeTime)
            {
                if(_shakeRandomTime <= 0.0f)
                {
                    _prevShakeOffset = _shakeOffset;
                    _shakeOffset = (Vector3)Random.insideUnitCircle * _shakeIntensity;
                    _shakeRandomTime = _shakeRandomTerm;
                }

                float shakeIntensityByTime = isLoop ? 1.0f : (_shakeTime / _shakeDuration);
                float t = (0.0f == _shakeRandomTerm) ? 1.0f : (1.0f - _shakeRandomTime / _shakeRandomTerm);
                outShakeOffset += Vector3.Lerp(_prevShakeOffset, _shakeOffset, t) * shakeIntensityByTime;
                _shakeRandomTime -= Time.deltaTime;

                if(false == isLoop)
                {
                    _shakeTime -= Time.deltaTime;
                }
            }
        }
    }
}

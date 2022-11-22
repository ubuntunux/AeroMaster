using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitModelBase : MonoBehaviour
{
    public int _initialHP = 50;
    public float _speed = 1.0f;
    
    UnitBase _unitObject = null;

    public UnitBase GetUnitObject()
    {
        return _unitObject;
    }

    public void SetUnitObject(UnitBase unitObject)
    {
        _unitObject = unitObject;
    }

    public int GetInitialHP()
    {
        return _initialHP;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    void OnTriggerEnter(Collider other)
    {
        _unitObject.OnTriggerEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        _unitObject.OnTriggerStay(other);
    }

    void OnTriggerExit(Collider other)
    {
        _unitObject.OnTriggerExit(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
    }
}

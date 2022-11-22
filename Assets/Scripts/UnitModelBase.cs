using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitModelBase : MonoBehaviour
{
    UnitBase _unitObject = null;

    public UnitBase GetUnitObject()
    {
        return _unitObject;
    }

    public void SetUnitObject(UnitBase unitObject)
    {
        _unitObject = unitObject;
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

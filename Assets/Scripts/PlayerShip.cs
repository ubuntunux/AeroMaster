using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    public float _speed = 1.0f;
    public GameObject _afterBurnerParticle;
    UnitBase _unitObject = null;

    public UnitBase GetUnitObject()
    {
        return _unitObject;
    }

    public void SetUnitObject(UnitBase unitObject)
    {
        _unitObject = unitObject;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public bool GetAfterBurnerEmission()
    {
        return _afterBurnerParticle.GetComponent<ParticleScript>().isEmission();
    }

    public void SetAfterBurnerEmission(bool emission)
    {
        _afterBurnerParticle.GetComponent<ParticleScript>().SetEmission(emission);
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

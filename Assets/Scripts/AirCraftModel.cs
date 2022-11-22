using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCraftModel : UnitModelBase
{
    public float _speed = 1.0f;
    public GameObject _afterBurnerParticle;
    
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
}

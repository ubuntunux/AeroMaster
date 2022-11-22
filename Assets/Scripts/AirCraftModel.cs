using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCraftModel : UnitModelBase
{
    public GameObject _afterBurnerParticle;

    public bool GetAfterBurnerEmission()
    {
        return _afterBurnerParticle.GetComponent<ParticleScript>().isEmission();
    }

    public void SetAfterBurnerEmission(bool emission)
    {
        _afterBurnerParticle.GetComponent<ParticleScript>().SetEmission(emission);
    }
}

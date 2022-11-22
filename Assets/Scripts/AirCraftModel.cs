using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCraftModel : UnitModelBase
{
    public GameObject _afterBurnerParticle;
    public GameObject _weaponSlotVulcan0;
    public GameObject _weaponSlotVulcan1;

    public bool GetAfterBurnerEmission()
    {
        return _afterBurnerParticle.GetComponent<ParticleScript>().isEmission();
    }

    public void SetAfterBurnerEmission(bool emission)
    {
        _afterBurnerParticle.GetComponent<ParticleScript>().SetEmission(emission);
    }

    public GameObject WeaponSlotVulcan0()
    {
        return _weaponSlotVulcan0;
    }

    public GameObject WeaponSlotVulcan1()
    {
        return _weaponSlotVulcan1;
    }
}

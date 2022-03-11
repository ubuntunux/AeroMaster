using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

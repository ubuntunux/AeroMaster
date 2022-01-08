using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    public ParticleSystem[] _particles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool isEmission()
    {
        foreach(ParticleSystem particle in _particles)
        {
            var em = particle.emission;
            return em.enabled;
        }
        return false;
    }

    public void SetEmission(bool emission)
    {
        foreach(ParticleSystem particle in _particles)
        {
            var em = particle.emission;
            em.enabled = emission;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

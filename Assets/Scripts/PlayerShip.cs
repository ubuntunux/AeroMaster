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

    void OnTriggerEnter(Collider other)
    {
        Player.Instance.OnTriggerEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        Player.Instance.OnTriggerStay(other);
    }

    void OnTriggerExit(Collider other)
    {
        Player.Instance.OnTriggerExit(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
    }
}

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
        CharacterManager.Instance.GetPlayer().OnTriggerEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        CharacterManager.Instance.GetPlayer().OnTriggerStay(other);
    }

    void OnTriggerExit(Collider other)
    {
        CharacterManager.Instance.GetPlayer().OnTriggerExit(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
    }
}

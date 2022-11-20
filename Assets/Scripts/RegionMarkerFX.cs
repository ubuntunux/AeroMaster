using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionMarkerFX : MonoBehaviour
{
    void Awake()
    {
        LevelManager.Instance.RegistRegionMarkerFX(this);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        position.y = CharacterManager.Instance.GetPlayer().GetPosition().y;
        transform.position = position;
    }
}

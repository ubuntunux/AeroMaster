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
        float heightHalf = GetComponent<MeshRenderer>().bounds.size.y * 0.5f;
        Vector3 position = Player.Instance.GetPosition();
        position.x = transform.position.x;
        position.y -= heightHalf;
        position.z = transform.position.z;
        transform.position = position;
    }
}

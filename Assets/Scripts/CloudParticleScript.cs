using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudParticleScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = Player.Instance.GetPosition();
        position.x += Player.Instance.GetFrontDirection() * 50.0f;        
        position.y = Constants.CLOUD_ALTITUDE;
        position.z += 10.0f;
        transform.position = position;
    }
}

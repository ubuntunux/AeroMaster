using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private static MainCamera _instance;

    // Singleton instantiation
    public static MainCamera Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.FindObjectOfType<MainCamera>();
            }
            return _instance;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Reset()
    {
        TrakingPlayer();
    }

    void TrakingPlayer()
    {
        Vector3 cameraPosition = Player.Instance.transform.position;
        const float CAMERA_PADDING = 1.0f;
        cameraPosition.x += Player.Instance._velocityRatioX * CAMERA_PADDING;
        cameraPosition.y += Player.Instance._velocityRatioY * CAMERA_PADDING + Constants.CAMERA_OFFSET_Y;
        cameraPosition.z = transform.position.z;

        transform.position = cameraPosition;
    }

    // Update is called once per frame
    void Update()
    {   
        TrakingPlayer();        
    }
}

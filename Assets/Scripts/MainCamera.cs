using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private static MainCamera instance;

    // Singleton instantiation
    public static MainCamera Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<MainCamera>();
            return instance;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        Vector3 cameraPosition = Player.Instance.transform.position;
        cameraPosition.y += Constants.CAMERA_OFFSET_Y;
        cameraPosition.z = transform.position.z;
        transform.position = cameraPosition;
    }
}

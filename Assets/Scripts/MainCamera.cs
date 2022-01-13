using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private static MainCamera _instance;

    private float _initialPosZ = 0.0f;
    private bool _trackingPlayer = false;

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

    public void ResetMainCamera()
    {
        SetTrackingPlayer(true);
        TrakingPlayer();
    }

    public void SetTrackingPlayer(bool trackingPlayer)
    {
        _trackingPlayer = trackingPlayer;
    }

    void TrakingPlayer()
    {
        if(_trackingPlayer)
        {
            Vector3 cameraPosition = Player.Instance.GetPosition();
            const float CAMERA_OFFSET_X = 5.0f;
            const float CAMERA_OFFSET_Y = 2.0f;
            const float CAMERA_OFFSET_Z = 6.0f;
            float frontDirection = Player.Instance.GetFrontDirection();
            float AbsVelocityRatioX = Player.Instance.GetAbsVelocityRatioX();
            float groundRatio = Mathf.Max(0.0f, Mathf.Min(1.0f, 1.0f - (Player.Instance.GetPosition().y - Constants.GROUND_HEIGHT) * 0.2f));

            cameraPosition.x += AbsVelocityRatioX * frontDirection * CAMERA_OFFSET_X;
            cameraPosition.y += 1.0f + AbsVelocityRatioX * groundRatio * CAMERA_OFFSET_Y;
            cameraPosition.z = _initialPosZ - (4.0f + AbsVelocityRatioX * CAMERA_OFFSET_Z);

            transform.position = cameraPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        TrakingPlayer();        
    }
}

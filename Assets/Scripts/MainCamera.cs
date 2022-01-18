using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private static MainCamera _instance;

    float _initialPosZ = 0.0f;
    bool _trackingPlayer = false;
    ShakeObject _cameraShake = new ShakeObject();
    ShakeObject _cameraHandMove = new ShakeObject();
    Vector3 _cameraPosition = Vector3.zero;

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
        SetCameraPosition(Vector3.zero);
        SetTrackingPlayer(true);
        if(_trackingPlayer)
        {
            TrakingPlayer(Vector3.zero);
        }

        _cameraShake.ResetShakeObject();
        _cameraHandMove.ResetShakeObject();
        
        SetCameraHandMove();
    }

    public void SetCameraPosition(Vector3 cameraPosition)
    {
        _cameraPosition = cameraPosition;
    }

    public void SetEnableCameraHandMove(bool enable)
    {
        _cameraHandMove.SetShakeEnable(enable);
    }

    public void SetCameraHandMove(float shakeDuration = 0.0f, float shakeIntensity = 0.5f, float shakeRandomTerm = 3.0f)
    {
        _cameraHandMove.SetShake(shakeDuration, shakeIntensity, shakeRandomTerm);
    }

    public void SetCameraShakeByDestroy()
    {
        const float CameraShakeDuration = 1.0f;
        const  float CameraShakeRandomTerm = 0.01f;
        const  float CameraShakeIntensity = 1.0f;
        _cameraShake.SetShake(CameraShakeDuration, CameraShakeIntensity, CameraShakeRandomTerm);
    }

    public void SetTrackingPlayer(bool trackingPlayer)
    {
        _trackingPlayer = trackingPlayer;
    }

    void TrakingPlayer(Vector3 cameraOffset)
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

        _cameraPosition = cameraPosition + cameraOffset;
    }

    // Update is called once per frame
    void Update()
    {   
        Vector3 cameraOffset = Vector3.zero;
        _cameraShake.UpdateShakeObject(ref cameraOffset);
        _cameraHandMove.UpdateShakeObject(ref cameraOffset);

        if(_trackingPlayer)
        {
            TrakingPlayer(cameraOffset);
        }

        transform.position = _cameraPosition + cameraOffset;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private static MainCamera _instance;

    float _initialPosZ = -4.0f;
    bool _trackingPlayer = false;
    BezierShakeObject _cameraShake = new BezierShakeObject();
    BezierShakeObject _cameraHandMove = new BezierShakeObject();
    Vector3 _cameraPosition = Vector3.zero;
    Quaternion _cameraRotation = Quaternion.identity;

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
        const float SCALE = 0.5f;
        #if UNITY_ANDROID
            GetComponent<DownscaleRT>().CreateRT(SCALE);
        #elif UNITY_IPHONE
            GetComponent<DownscaleRT>().CreateRT(SCALE);
        #else
        #endif        
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
        
        SetCameraHandMove(0.0f, 0.5f, 5.0f);
    }

    public void SetCameraPosition(Vector3 cameraPosition)
    {
        _cameraPosition = cameraPosition;
    }

    public void SetEnableCameraHandMove(bool enable)
    {
        _cameraHandMove.SetShakeEnable(enable);
    }

    public void SetCameraHandMove(float shakeDuration, float shakeIntensity, float shakeRandomTerm)
    {
        _cameraHandMove.SetShake(shakeDuration, shakeIntensity, shakeRandomTerm);
    }

    public void SetCameraShakeByDestroy(float CameraShakeIntensity = 1.0f)
    {
        const float CameraShakeDuration = 1.0f;
        const float CameraShakeRandomTerm = 0.01f;
        _cameraShake.SetShake(CameraShakeDuration, CameraShakeIntensity, CameraShakeRandomTerm);
    }

    public void SetTrackingPlayer(bool trackingPlayer)
    {
        _trackingPlayer = trackingPlayer;
    }

    void TrakingPlayer(Vector3 cameraOffset)
    {
        Vector3 cameraPosition = Player.Instance.GetPosition();

        if(LevelManager.Instance.IsLevelLobby())
        {
            cameraPosition.x += 2.0f;
            cameraPosition.y += 2.0f;
            cameraPosition.z -= 2.0f;
            _cameraRotation = Quaternion.Euler(25.0f, -45.0f, 0.0f);
        }
        else
        {
            const float CAMERA_OFFSET_X = 5.0f;
            const float CAMERA_OFFSET_Y = 2.0f;
            const float CAMERA_OFFSET_Z = 6.0f;
            float frontDirection = Player.Instance.GetFrontDirection();
            float AbsVelocityRatioX = Player.Instance.GetMaxVelocityRatio();
            // float groundRatio = Mathf.Max(0.0f, Mathf.Min(1.0f, 1.0f - (Player.Instance.GetPosition().y - Constants.GROUND_HEIGHT) * 0.2f));
            float groundRatio = Player.Instance.GetLandingGearRatio();

            cameraPosition.x += AbsVelocityRatioX * frontDirection * CAMERA_OFFSET_X;
            cameraPosition.y += 1.0f + AbsVelocityRatioX * groundRatio * CAMERA_OFFSET_Y;
            cameraPosition.z = _initialPosZ - AbsVelocityRatioX * CAMERA_OFFSET_Z;
            _cameraRotation = Quaternion.identity;
        }
        _cameraPosition = cameraPosition + cameraOffset;
    }

    // Update is called once per frame
    void Update()
    {   
        Vector3 cameraOffset = Vector3.zero;
        _cameraShake.UpdateShakeObject(ref cameraOffset);

        float handMoveIntensity = Player.Instance.GetMaxVelocityRatio() * 0.8f + 0.2f;
        _cameraHandMove.UpdateShakeObject(ref cameraOffset, handMoveIntensity);

        if(_trackingPlayer)
        {
            TrakingPlayer(cameraOffset);
        }

        transform.position = _cameraPosition + cameraOffset;
        transform.rotation = _cameraRotation;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public AudioSource _audioSource; //A primary audioSource a large portion of game sounds are passed through

    // Singleton instantiation
    public static GameManager Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

#if UNITY_ANDROID
    public Vector2 GetAltitudeTouchDelta()
    {
        for(int i=0; i<Input.touchCount; ++i)
        {
            Touch touch = Input.GetTouch(i);
            if(touch.position.x < (Screen.width / 2.0f))
            {
                return touch.deltaPosition;
            }
        }
        return Vector2.zero;
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        Reset();
    }

    public void Reset()
    {
        LevelManager.Instance.Reset();
        Vector3 startPoint = LevelManager.Instance.GetStartPoint();
        Player.Instance.ResetPlayer(startPoint);
        MainCamera.Instance.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

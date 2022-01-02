using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    // Singleton instantiation
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<GameManager>();
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

#if UNITY_ANDROID
    Vector2 GetAltitudeTouchDelta()
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

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    float _heightRatio = 1.0f;
    Vector2 _padding;
    Vector2 _half_size;
    Vector2 _mini_map_distance;
    Vector2 _world_to_minimap;

    public GameObject _layerCharacter = null;
    public GameObject _layerBackground = null;    

    // Singleton instantiation
    private static MiniMap _instance;
    public static MiniMap Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.FindObjectOfType<MiniMap>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    public Vector2 GetMiniMapDistance()
    {
        return _mini_map_distance;
    }

    public Vector2 GetHalfSize()
    {
        return _half_size;
    }

    public Vector2 GetWorldToMiniMap()
    {
        return _world_to_minimap;
    }

    // Start is called before the first frame update
    void Start()
    {
        _heightRatio = GetComponent<RectTransform>().sizeDelta.y / GetComponent<RectTransform>().sizeDelta.x;
        _padding = new Vector2(10.0f, 10.0f);
        _half_size = GetComponent<RectTransform>().sizeDelta * 0.5f - _padding;
        _mini_map_distance.x = Constants.LIMITED_ALTITUDE / _heightRatio;
        _mini_map_distance.y = Constants.LIMITED_ALTITUDE;
        _world_to_minimap = _half_size / _mini_map_distance;
    }
}

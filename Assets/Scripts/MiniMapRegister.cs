using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapRegister : MonoBehaviour
{
    public GameObject _miniMapObject;
    
    void Start()
    {
        _miniMapObject.GetComponent<MiniMapObject>().Initialize(transform);
    }

    void OnDestroy()
    {
        Destroy(_miniMapObject);
    }
}

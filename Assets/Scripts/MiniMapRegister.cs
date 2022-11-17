using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapRegister : MonoBehaviour
{
    public GameObject _miniMapObject = null;
    public bool _renderByCollider = false;
    
    void Start()
    {
        if(null != _miniMapObject)
        {
            _miniMapObject.GetComponent<MiniMapObject>().Initialize(transform,  _renderByCollider ? GetComponent<BoxCollider>() : null);
        }
    }

    public void DestroyMiniMapObject()
    {
        if(null != _miniMapObject)
        {
            Destroy(_miniMapObject);
            _miniMapObject = null;
        }
    }

    void OnDestroy()
    {
        DestroyMiniMapObject();
    }
}

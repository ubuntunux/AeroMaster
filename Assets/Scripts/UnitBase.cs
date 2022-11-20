using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public GameObject _prefabMeshObject;
    protected GameObject _meshObject;
    protected Animator _animator;

    void Awake()
    {
        if(null != _prefabMeshObject)
        {
            CreateMeshObject(_prefabMeshObject);
        }
    }

    public void CreateMeshObject(GameObject prefab)
    {
        if(null != _meshObject)
        {
            Destroy(_meshObject);
        }

        _meshObject = Instantiate(prefab);
        _meshObject.transform.SetParent(transform, false);
        _meshObject.transform.localPosition = Vector3.zero;
        _animator = _meshObject.GetComponent<Animator>();
    }

    public virtual bool IsPlayer()
    {
        return false;
    }

    public virtual UnitType GetUnit()
    {
        return UnitType.None;
    }
}

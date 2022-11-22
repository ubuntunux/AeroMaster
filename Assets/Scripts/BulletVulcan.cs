using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletVulcan : BulletBase
{
    public GameObject _destoryFX;
    float _damage = 10.0f;
    float _speed = 10.0f;
    float _moveDistance = 0.0f;
    float _limitDistance = 20.0f;

    public void CreateBulletVulcan(bool isOwnerPlayer, Transform weaponTransform, float damage, float speed)
    {
        base.CreateBulletObject(isOwnerPlayer);

        gameObject.transform.position = weaponTransform.position;
        gameObject.transform.rotation = weaponTransform.rotation;
        _damage = damage;
        _speed = speed;
        _moveDistance = 0.0f;
    }

    public override float GetDamage()
    {
        return _damage;
    }

    public override void SetDestroy(bool silentDestroy)
    {
        if(false == silentDestroy && null != _destoryFX)
        {
            GameObject destoryFX = Instantiate(_destoryFX);
            destoryFX.transform.position = transform.position;
        }

        base.SetDestroy(silentDestroy);
    }

    void Update()
    {
        float dist = _speed * Time.deltaTime;
        _moveDistance += dist;
        if(_moveDistance < _limitDistance)
        {
            Vector3 forward = transform.localRotation * -Vector3.forward;
            transform.position = transform.position + forward * dist;
        }
        else
        {
            SetDestroy(true);
        }
    }
}

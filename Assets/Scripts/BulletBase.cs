using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    bool _isOwnerPlayer = false;

    public bool GetIsOwnerPlayer()
    {
        return _isOwnerPlayer;
    }

    public void SetIsOwnerPlayer(bool isOwnerPlayer)
    {
        _isOwnerPlayer = isOwnerPlayer;
    }

    public virtual float GetDamage()
    {
        return 0.0f;
    }

    public void CreateBulletObject(bool isOwnerPlayer)
    {
        SetIsOwnerPlayer(isOwnerPlayer);
        LevelManager.Instance.RegistBulletObject(gameObject);
        gameObject.transform.SetParent(LevelManager.Instance.transform, false);
    }

    public virtual void SetDestroy(bool silentDestroy)
    {
        LevelManager.Instance.UnregistBulletObject(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if("Bullet" == other.gameObject.tag)
        {
            return;
        }
        else if("Unit" == other.gameObject.tag)
        {
            UnitBase unit = other.gameObject.GetComponent<UnitModelBase>().GetUnitObject();
            if(unit.IsPlayer() != GetIsOwnerPlayer())
            {
                unit.SetDamage(GetDamage(), false);
            }
            else
            {
                return;
            }
        }

        // anyway destroy
        SetDestroy(false);
    }
}

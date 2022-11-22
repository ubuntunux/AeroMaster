using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    UnitBase _ownerUnit;

    public bool GetIsOwnerPlayer()
    {
        return (null != _ownerUnit) ? _ownerUnit.IsPlayer() : false;
    }

    public UnitBase GetOwnerUnit()
    {
        return _ownerUnit;
    }

    public void SetOwnerUnit(UnitBase ownerUnit)
    {
        _ownerUnit = ownerUnit;
    }

    public virtual void InitializeWeaponObject(UnitBase ownerUnit, GameObject weaponSlot)
    {
        SetOwnerUnit(ownerUnit);
        gameObject.transform.SetParent(weaponSlot.transform, false);
    }

    public virtual void SetFire(bool fire, bool playSound)
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public GameObject _prefabModelObject;
    public GameObject _damageFX;
    public GameObject _destroyFX;
    public GameObject _impactWaterFX;
    public GameObject _hpBar;

    protected GameObject _modelObject;
    protected Animator _animator;    
    protected bool _isAlive = false;
    protected bool _isGround = false;    
    protected bool _invincibility = false;

    public void Awake()
    {
        if(null != _prefabModelObject)
        {
            CreateModelObject(_prefabModelObject);
        }
    }

    public void Start()
    {
        ResetUnit();
    }

    public virtual UnitType GetUnitType()
    {
        return UnitType.None;
    }

    public virtual bool IsPlayer()
    {
        return false;
    }

    public bool IsAlive()
    {
        return _isAlive;
    }

    public bool GetIsGround()
    {
        return _isGround;
    }

    public virtual void CreateModelObject(GameObject prefab)
    {
        if(null != _modelObject)
        {
            Destroy(_modelObject);
        }

        _modelObject = Instantiate(prefab);
        _modelObject.transform.SetParent(transform, false);
        _modelObject.transform.localPosition = Vector3.zero;
        _modelObject.GetComponent<UnitModelBase>().SetUnitObject(this);
        _animator = _modelObject.GetComponent<Animator>();
    }

    public void SetVisible(bool show)
    {
        _modelObject.SetActive(show);
    }

    public bool GetInvincibility()
    {
        return _invincibility;
    }

    public void SetInvincibility(bool invincibility)
    {
        _invincibility = invincibility;
    }

    public virtual void StopAllSound()
    {
    }

    public void SetForceDestroy()
    {
        SetInvincibility(false);
        SetDestroy(DestroyType.Explosion);
    }

    public virtual void SetDestroy(DestroyType destroyType)
    {
        if(false == GetInvincibility() && IsAlive())
        {
            MainCamera.Instance.SetCameraShakeByDestroy();
            
            GameObject destroyFX_Prefab = null;
            if(DestroyType.Explosion == destroyType)
            {
                destroyFX_Prefab = _destroyFX;
            }
            else if(DestroyType.ImpactWater == destroyType)
            {
                destroyFX_Prefab = _impactWaterFX;
            }

            if(null != destroyFX_Prefab)
            {
                GameObject destroyFX = (GameObject)GameObject.Instantiate(destroyFX_Prefab);
                destroyFX.transform.SetParent(transform, false);
            }
            
            SetVisible(false);
            StopAllSound();
            _isAlive = false;
        }
    }

    public virtual void SetDamage(float damage)
    {
        if(false == _invincibility && IsAlive())
        {
            _hpBar.GetComponent<HPBar>().SetDamage(damage);

            if(_hpBar.GetComponent<HPBar>().IsAlive())
            {
                MainCamera.Instance.SetCameraShakeByDestroy(0.1f);
                GameObject damageFX = (GameObject)GameObject.Instantiate(_damageFX);
                damageFX.transform.SetParent(transform, false);
            }
            else
            {
                SetDestroy(DestroyType.Explosion);
            }
        }
    }

    // begin physics collide
    public virtual void OnTriggerEnter(Collider other)
    {
        if("Wall" == other.gameObject.tag)
        {
            SetDestroy(DestroyType.Explosion);
        }
        else if("Water" == other.gameObject.tag)
        {
            SetDestroy(DestroyType.ImpactWater);
        }
        else if("Unit" == other.gameObject.tag)
        {
            if(IsPlayer())
            {
                UnitBase unit = other.gameObject.GetComponent<UnitModelBase>().GetUnitObject();                        
                unit.SetDestroy(DestroyType.Explosion);
                SetDestroy(DestroyType.Explosion);
            }
        }
    }

    public virtual void OnTriggerStay(Collider other)
    {
        if("Ground" == other.gameObject.tag)
        {
            _isGround = true;
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if("Ground" == other.gameObject.tag)
        {
            _isGround = false;
        }
    }
    // end of physics collide

    public virtual void ResetUnit()
    {
        int initialHP = IsPlayer() ? SaveData.Instance._playerData._hp : _modelObject.GetComponent<UnitModelBase>().GetInitialHP();
        _hpBar.GetComponent<HPBar>().InitializeHPBar(transform, initialHP);

        SetInvincibility(false);
        SetVisible(true);

        _isGround = false;
        _isAlive = true;
    }
}

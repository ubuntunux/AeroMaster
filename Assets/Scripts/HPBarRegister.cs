using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarRegister : MonoBehaviour
{
    public GameObject _hpBar = null;
    
    void Start()
    {
        if(null != _hpBar)
        {
            _hpBar.GetComponent<HPBar>().InitializeHPBar(transform);
        }
    }

    public void DestroyHPBar()
    {
        if(null != _hpBar)
        {
            Destroy(_hpBar);
            _hpBar = null;
        }
    }

    void OnDestroy()
    {
        DestroyHPBar();
    }
}

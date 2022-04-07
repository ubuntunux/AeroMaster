using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    public GameObject _hpBar;

    public float _initialHP = 100.0f;
    float _hp = 0.0f;
    float _initialHPBarWidth = 100.0f;
    Transform _targetTransform = null;

    void Start()
    {
        _initialHPBarWidth = _hpBar.GetComponent<RectTransform>().sizeDelta.x;
    }

    public void InitializeHPBar(Transform targetTransform, int hp = 0)
    {
        if(null != targetTransform)
        {
            _targetTransform = targetTransform;
            transform.SetParent(UIManager.Instance._canvasNoRayCast.transform, false);
        }
        
        if(hp <= 0)
        {
            _hp = _initialHP;
        }
        else
        {
            _initialHP = hp;
            _hp = hp;
        }
        
        UpdateHP();
    }

    public void ResetHP()
    {
        _hp = _initialHP;
        UpdateHP();
    }

    public float GetHP()
    {
        return _hp;
    }

    public void SetDamage(float damage)
    {
        _hp -= damage;
        UpdateHP();
    }

    public bool IsAlive()
    {
        return 0.0f < _hp;
    }

    public void UpdateHP()
    {
        float hpRatio = Mathf.Max(0.0f, Mathf.Min(1.0f, _hp / _initialHP));
        Vector2 sizeDelta = _hpBar.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x = _initialHPBarWidth * hpRatio;
        _hpBar.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }

    public void ShowHP(bool show)
    {
        gameObject.SetActive(show);
    }

    void Update()
    {
        if(null != _targetTransform && gameObject.activeSelf)
        {
            Vector2 WorldObject_ScreenPosition = UIManager.Instance.WorldToScreen(_targetTransform.position);
            GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
        }
    }
}

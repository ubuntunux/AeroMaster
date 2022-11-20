using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IndicatorUI : MonoBehaviour
{
    public GameObject _indicator;
    public GameObject _targetDistance;
    public Color _color;
    public string _targetName;
    
    Vector3 _targetPosition = Vector3.zero;    
    bool _show = true;

    public void SetIndicatorColor(Color color)
    {
        _indicator.GetComponent<RawImage>().color = color;
        _targetDistance.GetComponent<TextMeshProUGUI>().color = color;
        _color = color;        
    }

    public void SetIndicatorTargetName(string name)
    {
        _targetName = name;
    }

    public void SetIndicatorTargetPosition(Vector3 position)
    {
        _targetPosition = position;

         if(false == gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        _show = true;
    }

    void Start()
    {
        gameObject.SetActive(false);
        SetIndicatorColor(_color);
    }

    void Update()
    {
        if(_show)
        {
            LevelBase level = LevelManager.Instance.GetCurrentLevel();
            if(null != level)
            {
                Vector2 WorldObject_ScreenPosition = UIManager.Instance.WorldToScreen(_targetPosition);
                Vector2 screenSize = UIManager.Instance.GetScreenSize();
                float padding = 80.0f;
                float lengthRatio = Mathf.Max(
                    Mathf.Abs(WorldObject_ScreenPosition.x / (screenSize.x * 0.5f - padding)), 
                    Mathf.Abs(WorldObject_ScreenPosition.y / (screenSize.y * 0.5f - padding))
                );

                if(1.0f < lengthRatio)
                {
                    gameObject.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition / lengthRatio;
                }
                else
                {
                    gameObject.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
                }                

                // rotate arrow
                float angle = Mathf.Atan2(WorldObject_ScreenPosition.y, WorldObject_ScreenPosition.x) * Mathf.Rad2Deg;
                _indicator.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, angle - 90.0f);

                Vector3 toTarget = _targetPosition - CharacterManager.Instance.GetPlayer().GetPosition();
                int dist = (int)Mathf.Sqrt(toTarget.x * toTarget.x + toTarget.y * toTarget.y) * 5;
                if(0 < _targetName.Length)
                {
                    _targetDistance.GetComponent<TextMeshProUGUI>().text = _targetName + "\n" + dist.ToString() + "m";
                }
                else
                {
                    _targetDistance.GetComponent<TextMeshProUGUI>().text = dist.ToString() + "m";
                }
            }
            
            _show = false;
        }
        else if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}

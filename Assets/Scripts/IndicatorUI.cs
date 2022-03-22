using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IndicatorUI : MonoBehaviour
{
    public GameObject _canvas;
    public GameObject _indicator;
    public GameObject _targetDistance;
    
    Vector3 _targetPosition = Vector3.zero;
    bool _show = false;

    public void SetIndicatorTargetPosition(Vector3 position)
    {
        _show = true;
        _targetPosition = position;
    }

    void Start()
    {
        _indicator.SetActive(false);
        _targetDistance.SetActive(false);
    }

    void Update()
    {
        // Test indicate region marker
        {
            Vector3 playerPosition = Player.Instance.GetPosition();
            Vector2 region = LevelManager.Instance.GetMissionRegion();
            float minToPlayer = playerPosition.x - region.x;
            float playerToMax = region.y - playerPosition.x;
            float posX = (Mathf.Abs(minToPlayer) < Mathf.Abs(playerToMax)) ? region.x : region.y;
            SetIndicatorTargetPosition(new Vector3(posX, 0.0f, playerPosition.z));
        }        

        if(_show)
        {
            LevelBase level = LevelManager.Instance.GetCurrentLevel();
            if(null != level)
            {
                if(false == _indicator.activeSelf)
                {
                    _indicator.SetActive(true);
                    _targetDistance.SetActive(true);
                }

                RectTransform CanvasRect = _canvas.GetComponent<RectTransform>();
                float halfScreenSizeX = CanvasRect.sizeDelta.x * 0.5f;
                float halfScreenSizeY = CanvasRect.sizeDelta.y * 0.5f;
                Vector2 ViewportPosition = MainCamera.Instance.GetComponent<Camera>().WorldToViewportPoint(_targetPosition);
                Vector2 WorldObject_ScreenPosition = new Vector2(
                    ((ViewportPosition.x * CanvasRect.sizeDelta.x) - halfScreenSizeX),
                    ((ViewportPosition.y * CanvasRect.sizeDelta.y) - halfScreenSizeY)
                );
                float padding = 40.0f;
                float lengthRatio = Mathf.Max(
                    Mathf.Abs(WorldObject_ScreenPosition.x / (halfScreenSizeX - padding)), 
                    Mathf.Abs(WorldObject_ScreenPosition.y / (halfScreenSizeY - padding))
                );
                float angle = Mathf.Atan2(WorldObject_ScreenPosition.y, WorldObject_ScreenPosition.x) * Mathf.Rad2Deg;

                _indicator.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition / lengthRatio;
                _indicator.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, angle - 90.0f);
                _targetDistance.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition / lengthRatio;

                Vector3 toTarget = _targetPosition - Player.Instance.GetPosition();
                int dist = (int)Mathf.Sqrt(toTarget.x * toTarget.x + toTarget.y * toTarget.y);
                _targetDistance.GetComponent<TextMeshProUGUI>().text = dist.ToString() + "m";
            }
            
            _show = false;
        }
        else if(_indicator.activeSelf)
        {
            _indicator.SetActive(false);
            _targetDistance.SetActive(false);
        }
    }
}

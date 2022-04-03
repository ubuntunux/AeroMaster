using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MiniMapObject : MonoBehaviour
{
    Transform _targetTransform = null;

    public void Initialize(Transform targetTransform)
    {
        if(null != targetTransform)
        {
            transform.SetParent(MiniMap.Instance.gameObject.transform, false);
        }
        _targetTransform = targetTransform;
    }

    void Update()
    {
        if(null != _targetTransform)
        {
            Vector3 playerPosition = Player.Instance.GetPosition();
            Vector2 v = (_targetTransform.position - playerPosition) / MiniMap.Instance.GetMiniMapDistance();
            if(Mathf.Max(Mathf.Abs(v.x), Mathf.Abs(v.y)) <= 1.0f)
            {
                if(false == GetComponent<Image>().enabled)
                {
                    GetComponent<Image>().enabled = true;
                }

                GetComponent<RectTransform>().anchoredPosition = v * MiniMap.Instance.GetHalfSize();
            }
            else
            {
                if(GetComponent<Image>().enabled)
                {
                    GetComponent<Image>().enabled = false;
                }
            }
        }
    }
}

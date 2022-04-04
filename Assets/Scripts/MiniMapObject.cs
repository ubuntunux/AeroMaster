using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MiniMapObject : MonoBehaviour
{
    Transform _targetTransform = null;
    BoxCollider _collider = null;

    public void Initialize(Transform targetTransform, BoxCollider collider = null)
    {
        _collider = collider;

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
            Vector2 v = _targetTransform.position - playerPosition;
            GetComponent<RectTransform>().anchoredPosition = v * MiniMap.Instance.GetWorldToMiniMap();
            if(null != _collider)
            {
                Vector3 locaBound = _targetTransform.InverseTransformVector(_collider.bounds.size);
                locaBound.x *= _targetTransform.localScale.x;
                locaBound.y *= _targetTransform.localScale.y;
                locaBound.z *= _targetTransform.localScale.z;

                GetComponent<RectTransform>().sizeDelta = locaBound * MiniMap.Instance.GetWorldToMiniMap();
                Vector3 angles = _targetTransform.rotation.eulerAngles;
                GetComponent<RectTransform>().rotation = Quaternion.Euler(angles.x, angles.y, angles.z);
            }
        }
    }
}

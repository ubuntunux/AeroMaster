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
            gameObject.tag = targetTransform.gameObject.tag;
            GameObject layer = null;
            if(GameManager.Instance.isBackgroundTag(targetTransform.gameObject.tag))
            {
                layer = MiniMap.Instance._layerBackground;
            }
            else
            {
                layer = MiniMap.Instance._layerCharacter;
            }
            transform.SetParent(layer.transform, false);
        }

        _targetTransform = targetTransform;
    }

    void Update()
    {
        if(null != _targetTransform)
        {
            Vector3 playerPosition = Player.Instance.GetPosition();
            Vector3 position;
            if(null != _collider)
            {
                Vector3 localPosition = _targetTransform.InverseTransformPoint(_collider.bounds.center);
                position = _targetTransform.position + localPosition - playerPosition;

                Vector3 localBound = _targetTransform.InverseTransformVector(_collider.bounds.size);
                localBound.x *= _targetTransform.localScale.x;
                localBound.y *= _targetTransform.localScale.y;
                localBound.z *= _targetTransform.localScale.z;
                GetComponent<RectTransform>().sizeDelta = localBound * MiniMap.Instance.GetWorldToMiniMap();

                Vector3 angles = _targetTransform.rotation.eulerAngles;
                GetComponent<RectTransform>().rotation = Quaternion.Euler(angles.x, angles.y, angles.z);
            }
            else
            {
                position = _targetTransform.position - playerPosition;
            }

            GetComponent<RectTransform>().anchoredPosition = position * MiniMap.Instance.GetWorldToMiniMap();
        }
    }
}

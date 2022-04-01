using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    void Awake()
    {
        LevelManager.Instance.RegistStartPoint(transform.position);
    }

    void Start()
    {
        gameObject.SetActive(false);
    }
}

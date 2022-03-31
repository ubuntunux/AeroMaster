using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    void Awake()
    {
        LevelManager.Instance.RegistStartPosition(transform.position);
    }

    void Start()
    {
        gameObject.SetActive(false);
    }
}

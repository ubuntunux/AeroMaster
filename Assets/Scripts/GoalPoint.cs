using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    void Awake()
    {
        LevelManager.Instance.RegistGoalPoint(transform.position);
    }
}

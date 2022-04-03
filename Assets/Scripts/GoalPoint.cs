using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    public string _goalPointName;

    void Awake()
    {
        LevelManager.Instance.RegistGoalPoint(transform, _goalPointName);
    }
}

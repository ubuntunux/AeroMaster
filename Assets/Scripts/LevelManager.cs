using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBase: MonoBehaviour
{
    virtual public Vector3 GetStartPoint()
    {
        return Vector3.zero;
    }

    virtual public Vector3 GetGoalPosition()
    {
        return Vector3.zero;
    }
    
    virtual public void Reset()
    {
    }

    virtual public void OnExit()
    {

    }

    public void Update()
    {
    }
}

public class LevelManager : MonoBehaviour
{
    public GameObject _levelTutorial;

    private static LevelManager _instance;    

    // Singleton instantiation
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.FindObjectOfType<LevelManager>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Reset()
    {
        _levelTutorial.GetComponent<LevelBase>().Reset();
    }

    public Vector3 GetStartPoint()
    {
        return _levelTutorial.GetComponent<LevelBase>().GetStartPoint();
    }

    public Vector3 GetGoalPosition()
    {
        return _levelTutorial.GetComponent<LevelBase>().GetGoalPosition();
    }

    // Update is called once per frame
    void Update()
    {
        _levelTutorial.GetComponent<LevelBase>().Update();
    }
}

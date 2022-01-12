using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject _btnGoRight;
    public GameObject _btnGoLeft;
    public GameObject _btnLanding;
    public GameObject _slideVerticalVelocity;

    private static UIManager _instance;

    // Singleton instantiation
    public static UIManager Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.FindObjectOfType<UIManager>();
            }
            return _instance;
        }
    }

    public void SetInteractableGoRightButton(bool interactable)
    {
        _btnGoRight.GetComponent<Button>().interactable = interactable;
    }

    public void SetInteractableGoLeftButton(bool interactable)
    {
        _btnGoLeft.GetComponent<Button>().interactable = interactable;
    }

    public void SetInteractableLandingButton(bool interactable)
    {
        _btnLanding.GetComponent<Button>().interactable = interactable;
    }

    public void SetInteractableButtonAll(bool interactable)
    {
        SetInteractableGoRightButton(interactable);
        SetInteractableGoLeftButton(interactable);
        SetInteractableLandingButton(interactable);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Reset()
    {
        SetInteractableButtonAll(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

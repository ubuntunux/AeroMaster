using System.Collections;
using System.Collections.Generic;
using System.Globalization; 
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject _btnGoRight;
    public GameObject _btnGoLeft;
    public GameObject _btnLanding;
    public GameObject _slideVerticalVelocity;    
    public GameObject _imageMissionComplete;
    public GameObject _imageMissionFailed;
    public GameObject _layerControllerUI;
    public GameObject _layeyExit;
    public GameObject _textScore;
    public GameObject _textTime;

    // debug
    public GameObject _debugTextVelocityX;
    public GameObject _debugTextVelocityY;
    public GameObject _debugTextAltitude;
    public GameObject _debugTextLanguage;    

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

    public bool GetVisibleControllerUI()
    {
        return _layerControllerUI.activeSelf;
    }

    public void SetVisibleControllerUI(bool show)
    {
        _layerControllerUI.SetActive(show);
    }

    public void ShowMissionCompleteOrFailed(bool show, bool isMissionSuccess)
    {
        _imageMissionComplete.SetActive(show && isMissionSuccess);
        _imageMissionFailed.SetActive(show && false == isMissionSuccess);
    }

    void PopupExit(bool open)
    {
        if(open && LevelManager.Instance.IsLevelLobby())
        {
            GameManager.Instance.RunBack();
            return;
        }

        _layeyExit.SetActive(open);
        GameManager.Instance.SetPause(open);
    }

    void TogglePopupExit()
    {
        PopupExit(!_layeyExit.activeSelf);
    }

    public void OnClickBack()
    {
        PopupExit(true);
    }

    public void OnClickBackYes()
    {
        PopupExit(false);
        GameManager.Instance.RunBack();
    }

    public void OnClickBackNo()
    {
        PopupExit(false);
    }

    void Start()
    {
        _layeyExit.SetActive(false);        
    }

    public void ResetUIManager()
    {
        _layeyExit.SetActive(false);        
        SetInteractableButtonAll(true);
        ShowMissionCompleteOrFailed(false, false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePopupExit();
        }

        _textScore.GetComponent<TextMeshProUGUI>().text = "Score: " + SaveData.Instance._playerData._score.ToString();
        _textTime.GetComponent<TextMeshProUGUI>().text = "Time: " + LevelManager.Instance.GetMissionTime().ToString();

        // debug
        _debugTextLanguage.GetComponent<TextMeshProUGUI>().text = "Language: " + CultureInfo.CurrentCulture.Name;
        _debugTextVelocityX.GetComponent<TextMeshProUGUI>().text = string.Format("Horizontal Speed: {0:F1}", Player.Instance.GetAbsVelocityX());
        _debugTextVelocityY.GetComponent<TextMeshProUGUI>().text = string.Format("Vertical Speed: {0:F1}", Player.Instance.GetVelocityY());
        _debugTextAltitude.GetComponent<TextMeshProUGUI>().text = string.Format("Altitude: {0:F1}", Player.Instance.GetAltitude());        
    }
}

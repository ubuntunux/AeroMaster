using System.Collections;
using System.Collections.Generic;
using System.Globalization; 
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum FingerTarget
{
    None,
    GoLeft,
    Landing,
    GoRight,
    VerticalVelocity,
};

public class UIManager : MonoBehaviour
{
    public GameObject _canvas;
    public GameObject _canvasNoRayCast;

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
    public GameObject _panelFadeInOut;
    public GameObject _textWindow;
    public GameObject _imageFinger;
    public GameObject _miniMap;
    public GameObject _hpBar;

    // indicator
    public GameObject _indicatorPrefab;
    public GameObject _missionRegionIndicator;

    // 
    bool _visibleLayerControllerUI = false;
    bool _visibleLayerControllerUIByActorScript = false;

    // subject text
    public GameObject _textSubject;
    float _totalSubjectTextTime = 3.0f;
    float _subjectTime = 0.0f;
    
    // debug
    public GameObject _debugTextVelocityX;
    public GameObject _debugTextVelocityY;
    public GameObject _debugTextAltitude;
    public GameObject _debugTextLanguage;
    public GameObject _debugTextFps;

    // fps
    int _frameCount = 0;
    float _frameTime = 0.0f;

    GameObject _fingerTarget = null;
    const float FADE_TIME = 0.5f;
    const float HALF_FADE_TIME = FADE_TIME / 2.0f;
    
    float _fadeTime = 0.0f;
    GameObject _nextLevelPrefab = null;

    public delegate void Callback();
    private Callback _callbackOnFadeOut = null;

    Vector3 _targetPosition = Vector3.zero;

    // Singleton instantiation
    private static UIManager _instance;
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

    public void SetFingerTarget(FingerTarget target)
    {
        _imageFinger.SetActive(FingerTarget.None != target);

        switch(target)
        {
            case FingerTarget.GoLeft:
                _fingerTarget = _btnGoLeft;
                break;
            case FingerTarget.Landing:
                _fingerTarget = _btnLanding;
                break;
            case FingerTarget.GoRight:
                _fingerTarget = _btnGoRight;
                break;
            case FingerTarget.VerticalVelocity:
                _fingerTarget = _slideVerticalVelocity;
                break;
            default:
                _fingerTarget = null;
                break;
        }

        UpdateFingerTarget();
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

    void UpdateVisibleControllerUI()
    {
        bool show = _visibleLayerControllerUI && _visibleLayerControllerUIByActorScript;
        _layerControllerUI.SetActive(show);
        _miniMap.SetActive(show);
        _hpBar.SetActive(show);
        //GetComponent<MissionObjectiveManager>().ShowMissionObjective(show);
    }

    public void SetVisibleControllerUI(bool show)
    {
        _visibleLayerControllerUI = show;
        UpdateVisibleControllerUI();
    }

    public void SetVisibleControllerUIByActorScript(bool show)
    {
        _visibleLayerControllerUIByActorScript = show;
        UpdateVisibleControllerUI();
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
        _visibleLayerControllerUI = GetVisibleControllerUI();
    }

    // Text Window
    public bool IsTextWindowActivated()
    {
        return _textWindow.GetComponent<TextManager>().IsActivated();
    }

    public TextManager GetTextWindow()
    {
        return _textWindow.GetComponent<TextManager>();
    }

    public void SetCharacterText(Characters character, string text, float timer)
    {
        _textWindow.GetComponent<TextManager>().SetCharacterText(character, text, timer);
    }
    
    public void ResetCharacterText()
    {
        _textWindow.GetComponent<TextManager>().ResetCharacterText();
    }

    public bool GetReadTextDone()
    {
        return _textWindow.GetComponent<TextManager>().GetReadTextDone();
    }

    // Fade Screen
    public bool CanFadeInOut()
    {
        return 0.0f == _fadeTime;
    }

    public void SetFadeInOutAndLevelChange(GameObject levelPrefab, float fadeInRatio = 1.0f)
    {
        if(CanFadeInOut() && levelPrefab != _nextLevelPrefab)
        {
            _fadeTime = FADE_TIME * fadeInRatio;
            _nextLevelPrefab = levelPrefab;
        }
    }

    public void SetFadeInOut(Callback callbackOnFadeOut = null)
    {
        _fadeTime = FADE_TIME;
        _callbackOnFadeOut = callbackOnFadeOut;
    }

    void UpdateFingerTarget()
    {
        if(null != _fingerTarget)
        {
            Vector3 position = _fingerTarget.transform.position;
            float scale = 1.0f;
            float opacity = 1.0f;

            if(_slideVerticalVelocity == _fingerTarget)
            {
                float slide = (Time.time * 1.0f) % 1.0f;
                position.y += Mathf.SmoothStep(0.0f, 100.0f, slide);
                opacity = Mathf.SmoothStep(0.0f, 1.0f, 1.0f - slide);
            }
            else
            {
                float t = Mathf.Sin(Time.time * 5.0f) * 0.5f + 0.5f;
                scale = Mathf.Lerp(1.0f, 1.5f, t);
                opacity = 1.0f - t;
            }

            _imageFinger.transform.position = position;
            _imageFinger.transform.localScale = new Vector3(scale, scale, scale);
            _imageFinger.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, opacity);
        }
    }

    void UpdateFadeInOut()
    {
        if(0.0f < _fadeTime)
        {
            float prevFadeTime = _fadeTime;
            _fadeTime = Mathf.Max(0.0f, _fadeTime - Time.deltaTime);
            float fade = 1.0f - Mathf.Abs((_fadeTime / FADE_TIME) * 2.0f - 1.0f);
            if(LevelManager.Instance.IsNullLevelPrefab())
            {
                fade = 1.0f;
            }
            _panelFadeInOut.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, fade);

            if(_panelFadeInOut.activeSelf && _fadeTime <= 0.0f)
            {
                _panelFadeInOut.SetActive(false);
            }
            else if(false == _panelFadeInOut.activeSelf && 0.0f < _fadeTime)
            {
                _panelFadeInOut.SetActive(true);
            }

            // fade out event
            if(HALF_FADE_TIME < prevFadeTime && _fadeTime <= HALF_FADE_TIME)
            {
                if(null != _callbackOnFadeOut)
                {
                    // fade in out callback event
                    _callbackOnFadeOut();
                    _callbackOnFadeOut = null;
                }
                else
                {
                    // fade in out and change level
                    LevelManager.Instance.SetCurrentLevelCallback(_nextLevelPrefab);
                    _nextLevelPrefab = null;
                }
            }
        }
    }

    // Subject Text
    public bool CheckSubjectTextDone()
    {
        return _subjectTime <= 0.0f;
    }

    public void SetSubjectText(string subject)
    {
        bool isEmpty = 0 == subject.Length;

        if(false == isEmpty)
        {
            SetVisibleControllerUI(false);
        }

        _textSubject.SetActive(!isEmpty);
        _textSubject.GetComponent<TextMeshProUGUI>().text = "- " + subject + " -";
        _subjectTime = isEmpty ? 0.0f : _totalSubjectTextTime;        
    }

    public void UpdateSubjectText()
    {
        if(0.0f < _subjectTime)
        {
            float opacity = (1.0f - Mathf.Min(1.0f, Mathf.Abs((_subjectTime / _totalSubjectTextTime) - 0.5f) * 2.0f)) * 2.0f;
            _textSubject.GetComponent<TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, opacity);
            _subjectTime -= Time.deltaTime;

            if(CheckSubjectTextDone())
            {
                SetVisibleControllerUI(true);
                _textSubject.SetActive(false);
            }
        }
    }

    // Indicator
    public IndicatorUI CreateIndicatorUI(Vector3 position, string name, Color color)
    {
        IndicatorUI indicator = Instantiate(_indicatorPrefab).GetComponent<IndicatorUI>();
        indicator.transform.parent = _canvasNoRayCast.transform;
        indicator.transform.position = Vector3.zero;
        indicator.transform.rotation = Quaternion.identity;
        indicator.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        indicator.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        indicator.GetComponent<RectTransform>().rotation = Quaternion.identity;
        indicator.SetIndicatorTargetPosition(position);
        indicator.SetIndicatorTargetName(name);
        indicator.SetIndicatorColor(color);
        return indicator;
    }

    public void DestroyIndicatorUI(ref IndicatorUI indicator)
    {
        if(null != indicator)
        {
            Destroy(indicator.gameObject);
            indicator = null;
        }
    }

    // Mission Region
    public void SetMissionRegionIndicator(Vector3 position)
    {
        _missionRegionIndicator.GetComponent<IndicatorUI>().SetIndicatorTargetPosition(position);
    }

    public void ShowMissionRegionWarning(bool show, bool isWarningRegionRight)
    {
        GetComponent<MissionRegionWarning>().ShowMissionRegionWarning(show, isWarningRegionRight);
    }

    // Mission Objective
    public bool IsMissionObjectiveTimeUp(string key)
    {
        return GetComponent<MissionObjectiveManager>().IsMissionObjectiveTimeUp(key);
    }

    public void ClearMissionObjectives()
    {
        GetComponent<MissionObjectiveManager>().ClearMissionObjectives();
    }

    public void RegistMissionObjective(string key, string missionText, float timer = 0.0f)
    {
        GetComponent<MissionObjectiveManager>().RegistMissionObjective(key, missionText, timer);
    }

    public void UnregistMissionObjective(string key)
    {
        GetComponent<MissionObjectiveManager>().UnregistMissionObjective(key);
    }

    public void SetMissionObjectiveState(string key, MissionObjectiveState state)
    {
        switch(state)
        {
            case MissionObjectiveState.Success:
                AudioManager.Instance._audioSuccess.Play();
                break;
            case MissionObjectiveState.Failed:
                AudioManager.Instance._audioWarnning.Play();
                break;
            default:
                break;
        }
        GetComponent<MissionObjectiveManager>().SetMissionObjectiveState(key, state);
    }

    // UIManager
    public Vector2 GetScreenSize()
    {
        return UIManager.Instance._canvasNoRayCast.GetComponent<RectTransform>().sizeDelta;
    }

    public Vector2 WorldToScreen(Vector3 targetPosition)
    {
        RectTransform CanvasRect = UIManager.Instance._canvasNoRayCast.GetComponent<RectTransform>();
        Vector2 ViewportPosition = MainCamera.Instance.GetComponent<Camera>().WorldToViewportPoint(targetPosition);
        ViewportPosition.x -= 0.5f;
        ViewportPosition.y -= 0.5f;        
        return ViewportPosition * CanvasRect.sizeDelta;
    }

    public void ResetUIManager()
    {
        _layeyExit.SetActive(false);
        SetFingerTarget(FingerTarget.None);
        SetInteractableButtonAll(true);
        ShowMissionCompleteOrFailed(false, false);
        SetSubjectText("");
    }

    public void OnStartLevel()
    {
        SetInteractableButtonAll(true);
        SetVisibleControllerUI(true);
        SetFingerTarget(FingerTarget.None);
        ShowMissionCompleteOrFailed(false, false);
        ShowMissionRegionWarning(false, false);
        ClearMissionObjectives();
        SetSubjectText("");
    }

    public void OnEndLevel(LevelEndTypes type)
    {
        ShowMissionCompleteOrFailed(LevelEndTypes.Silent != type, LevelEndTypes.MissionSucess == type);
        ShowMissionRegionWarning(false, false);
        SetVisibleControllerUI(false);
        SetInteractableButtonAll(false);
        SetFingerTarget(FingerTarget.None);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePopupExit();
        }

        UpdateFadeInOut();
        UpdateFingerTarget();
        UpdateSubjectText();

        _textScore.GetComponent<TextMeshProUGUI>().text = SaveData.Instance._playerData._score.ToString();

        // debug
        _textTime.GetComponent<TextMeshProUGUI>().text = "Mission Time: " + LevelManager.Instance.GetMissionTime().ToString();
        _debugTextLanguage.GetComponent<TextMeshProUGUI>().text = "Language: " + CultureInfo.CurrentCulture.Name;
        _debugTextVelocityX.GetComponent<TextMeshProUGUI>().text = string.Format("Horizontal Speed: {0:F1}", Player.Instance.GetAbsVelocityX());
        _debugTextVelocityY.GetComponent<TextMeshProUGUI>().text = string.Format("Vertical Speed: {0:F1}", Player.Instance.GetVelocityY());
        _debugTextAltitude.GetComponent<TextMeshProUGUI>().text = string.Format("Altitude: {0:F1}", Player.Instance.GetAltitude());

        // fps
        _frameCount += 1;
        _frameTime += Time.deltaTime;
        if(1.0f <= _frameTime)
        {
            float fps = (float)_frameCount / _frameTime;
            float time = _frameTime / (float)_frameCount * 1000.0f;
            _debugTextFps.GetComponent<TextMeshProUGUI>().text = string.Format("FPS: {0:F1}/{1:F1}ms", fps, time);
            _frameCount = 0;
            _frameTime = 0.0f;            
        }
    }
}

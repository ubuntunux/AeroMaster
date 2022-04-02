using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Characters
{
    None,
    Operator,
    Bright,
    Benedict,
    Mark,
    Count
};

public struct ActorScript
{
    public Characters _actor;
    public string _script;
};

public class ActorScriptManager : MonoBehaviour
{
    const char PAGE_SEPERATOR = '#';
    string _pageKey = "";
    float _readDoneTime = 0.0f;
    int _scriptIndex = 0;
    Dictionary<string, List<ActorScript>> _actorScriptsPages = new Dictionary<string, List<ActorScript>>();
    public delegate void Callback();
    private Callback _callbackOnPageReadDone = null;

    // Singleton instantiation
    private static ActorScriptManager _instance;    
    public static ActorScriptManager Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.FindObjectOfType<ActorScriptManager>();
            }
            return _instance;
        }
    }

    public void ClearActorScriptsPages()
    {
        _scriptIndex = 0;
        _pageKey = "";
        _callbackOnPageReadDone = null;
        _actorScriptsPages.Clear();

        TextManager textManager = UIManager.Instance.GetTextWindow();
        textManager.SetActive(false);
    }

    public void GenerateActorScriptsPages(string textScripts)
    {
        ClearActorScriptsPages();

        string[] pages = textScripts.Split(PAGE_SEPERATOR);
        for(int pageIndex = 1; pageIndex < pages.Length; pageIndex += 2)
        {
            string pageKey = pages[pageIndex].Trim();
            string page = pages[pageIndex + 1].Trim();
            if(0 < page.Length)
            {
                _actorScriptsPages.Add(pageKey, new List<ActorScript>());

                string[] scripts = page.Split('[');
                for(int scriptIndex = 0; scriptIndex < scripts.Length; ++scriptIndex)
                {
                    string script = scripts[scriptIndex].Trim();
                    if(0 < script.Length)
                    {
                        string[] text = script.Split(']');
                        ActorScript actorScript;
                        actorScript._actor = Characters.None;
                        actorScript._script = text[1].Trim();
                        foreach(Characters character in System.Enum.GetValues(typeof(Characters)))
                        {
                            if(character.ToString() == text[0])
                            {
                                actorScript._actor = character;
                                break;
                            }
                        }
                        _actorScriptsPages[pageKey].Add(actorScript);
                    }
                }
            }
        }
    }

    public bool CheckCurrentPageReadDone()
    {
        if(false == UIManager.Instance.IsTextWindowActivated())
        {
            if(false == _actorScriptsPages.ContainsKey(_pageKey) || _scriptIndex == _actorScriptsPages[_pageKey].Count)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckPageReadDone(string pageKey)
    {
        if(false == UIManager.Instance.IsTextWindowActivated())
        {
            if(pageKey == _pageKey)
            {
                return CheckCurrentPageReadDone();
            }
        }
        return false;
    }

    public bool SetPageAndCheckReadDone(string pageKey, Callback callbackOnPageReadDone = null, float readDoneTime = 0.0f)
    {
        SetPage(pageKey, callbackOnPageReadDone, readDoneTime);

        return CheckPageReadDone(pageKey);
    }

    public void SetPage(string pageKey, Callback callbackOnPageReadDone = null, float readDoneTime = 0.0f)
    {
        if(pageKey != _pageKey)
        {
            UIManager.Instance.SetVisibleControllerUIByActorScript(false);
            _callbackOnPageReadDone = callbackOnPageReadDone;
            _scriptIndex = 0;
            _pageKey = pageKey;
            _readDoneTime = readDoneTime;

            if(false == _actorScriptsPages.ContainsKey(pageKey))
            {
                SetPageReadDone();
            }            
        }
    }

    void SetPageReadDone()
    {
        UIManager.Instance.GetTextWindow().SetDone();
        UIManager.Instance.SetVisibleControllerUIByActorScript(true);
        if(null != _callbackOnPageReadDone)
        {
            _callbackOnPageReadDone();
            _callbackOnPageReadDone = null;
        }
    }

    public void Update()
    {
        if(GameManager.Instance.GetPaused())
        {
            return;
        }

        TextManager textManager = UIManager.Instance.GetTextWindow();
        if(textManager.GetReadTextAllDone())
        {
            if(_actorScriptsPages.ContainsKey(_pageKey))
            {
                if(_scriptIndex < _actorScriptsPages[_pageKey].Count)
                {
                    ActorScript actorScript = _actorScriptsPages[_pageKey][_scriptIndex];
                    UIManager.Instance.SetCharacterText(actorScript._actor, actorScript._script, _readDoneTime);
                    ++_scriptIndex;
                }
                else if(textManager.IsActivated())
                {
                    SetPageReadDone();
                }
            }
        }
    }
};
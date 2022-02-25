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

public class ActorScriptsPages
{
    int _pageIndex = 0;
    int _scriptIndex = 0;
    List<List<ActorScript>> _actorScriptsPages = new List<List<ActorScript>>();

    public void GenerateActorScriptsPages(string textScripts)
    {
        _pageIndex = 0;
        _scriptIndex = 0;
        _actorScriptsPages.Clear();

        int pageIndex = 0;
        string[] pages = textScripts.Split('#');
        foreach(string page in pages)
        {
            _actorScriptsPages.Add(new List<ActorScript>());
            string[] scripts = page.Split('[');
            for(int i = 0; i < scripts.Length; ++i)
            {
                string script = scripts[i].Trim();
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
                    _actorScriptsPages[pageIndex].Add(actorScript);
                }
            }
            ++pageIndex;
        }
    }

    public bool CheckAllPageReadDone()
    {
        return false == UIManager.Instance.IsTextWindowActivated() && _pageIndex == _actorScriptsPages.Count;
    }

    public bool CheckCurrentPageReadDone()
    {
        if(false == UIManager.Instance.IsTextWindowActivated())
        {
            if(_pageIndex == _actorScriptsPages.Count || _scriptIndex == _actorScriptsPages[_pageIndex].Count)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckCurrentScriptReadDoneAndUpdateScript()
    {
        bool isTextWindowActivated = UIManager.Instance.IsTextWindowActivated();
        if(isTextWindowActivated)
        {
            return false;
        }

        if(_pageIndex < _actorScriptsPages.Count)
        {
            if(_scriptIndex < _actorScriptsPages[_pageIndex].Count)
            {
                ActorScript actorScript = _actorScriptsPages[_pageIndex][_scriptIndex];
                UIManager.Instance.SetCharacterText(actorScript._actor, actorScript._script);
                ++_scriptIndex;
                return false;
            }
            else
            {
                _scriptIndex = 0;
                ++_pageIndex;

                if(_actorScriptsPages.Count == _pageIndex)
                {
                    UIManager.Instance.SetTextWindowDone();
                }
            }
        }
        return true;
    }
};


public class TextManager : MonoBehaviour
{
    public GameObject _imagePortrait;
    public GameObject _textName;
    public GameObject _textText;
    public Sprite[] _portraits;
    public AudioSource _sndPrompt;    

    // script
    List<string> _textList = new List<string>();
    float _textTime = 0.0f;
    int _pageIndex = 0;
    bool _readTextDone = true;

    // constants
    float _textSpeed = 50.0f;
    int _numTextCountPerLine = 25;
    int _numTextLineCountPerPage = 4;

    public bool IsActivated()
    {
        return gameObject.activeSelf;
    }

    public bool GetReadTextDone()
    {
        return _readTextDone;
    }

    public void SetCharacterText(Characters character, string text)
    {
        bool isEmpty = 0 == text.Length;// || Characters.None == character;

        gameObject.SetActive(!isEmpty);

        // set text
        _textName.GetComponent<TextMeshProUGUI>().text = character.ToString();
        _textText.GetComponent<TextMeshProUGUI>().text = "";

        // reset
        _readTextDone = isEmpty;
        _textTime = 0.0f;
        _pageIndex = 0;
        
        // rearrange text lines
        _textList.Clear();
        List<string> textList = new List<string>();
        string[] textArray = text.Split('\n');
        foreach(string currentText in textArray)
        {
            int lineCount = (currentText.Length + _numTextCountPerLine - 1) / _numTextCountPerLine;            
            for(int i = 0; i < lineCount; ++i)
            {
                int startIndex = i * _numTextCountPerLine;
                int availableTextCount = Mathf.Min(currentText.Length - startIndex, _numTextCountPerLine);
                textList.Add(currentText.Substring(startIndex, availableTextCount));

                if(_numTextLineCountPerPage == textList.Count)
                {
                    _textList.Add(string.Join("\n", textList));
                    textList.Clear();
                }
            }
        }

        // add rest texts
        if(0 < textList.Count)
        {
            _textList.Add(string.Join("\n", textList));
            textList.Clear();
        }

        // set portrait
        int index = (int)character;
        if(index < _portraits.Length)
        {
            _imagePortrait.GetComponent<Image>().sprite = _portraits[index];
        }

        // play prompt sound
        _sndPrompt.Play();

        if(false == isEmpty)
        {
            // hide controller
            UIManager.Instance.SetVisibleControllerUI(false);
        }
    }

    public void ResetCharacterText()
    {
        SetCharacterText(Characters.None, "");
    }

    public void SetDone()
    {
        // show controller
        UIManager.Instance.SetVisibleControllerUI(true);
    }

    public void OnClickNext()
    {
        if(_readTextDone)
        {
            _textTime = 0.0f;
            ++_pageIndex;

            if(_pageIndex < _textList.Count)
            {
                // continue
                _readTextDone = false;
                _sndPrompt.Play();
            }
            else            
            {
                _readTextDone = true;
                gameObject.SetActive(false);
            }
        }
        else
        {
            if(_pageIndex < _textList.Count)
            {
                _textTime = (float)_textList[_pageIndex].Length;
            }
        }
    }

    void Update()
    {
        if(GameManager.Instance.GetPaused())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            if(IsActivated())
            {
                OnClickNext();
            }
        }

        if(false == _readTextDone && _pageIndex < _textList.Count)
        {
            string text = _textList[_pageIndex];
            int numText = Mathf.Min(text.Length, (int)_textTime + 1);
            _textText.GetComponent<TextMeshProUGUI>().text = text.Substring(0, numText);
            if(text.Length == numText)
            {
                _readTextDone = true;
                _sndPrompt.Stop();
            }
            else
            {
                _textTime += Time.deltaTime * _textSpeed;
            }
        }
    }
}

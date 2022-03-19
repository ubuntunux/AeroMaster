using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    bool _readTextDone = false;
    bool _readTextAllDone = false;

    // constants
    float _textSpeed = 50.0f;
    int _numTextCountPerLine = 40;
    int _numTextLineCountPerPage = 4;

    public bool IsActivated()
    {
        return gameObject.activeSelf;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public bool GetReadTextDone()
    {
        return _readTextDone;
    }

    public bool GetReadTextAllDone()
    {
        return _readTextAllDone;
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
        _readTextAllDone = isEmpty;

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
    }

    public void ResetCharacterText()
    {
        SetCharacterText(Characters.None, "");
    }

    public void SetDone()
    {
        gameObject.SetActive(false);
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
                _readTextAllDone = true;
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

        if (Input.anyKeyDown)
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

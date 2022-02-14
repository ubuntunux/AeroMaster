using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Characters
{
    Operator,
    Bright,
    Benedict,
    Mark
};

public class TextManager : MonoBehaviour
{
    public GameObject _imagePortrait;
    public GameObject _textName;
    public GameObject _textText;

    public Sprite[] _portraits;

    List<string> _textList = new List<string>();
    float _textTime = 0.0f;
    int _pageIndex = 0;
    bool _paused = true;

    // constants
    float _textSpeed = 50.0f;
    int _numTextCountPerLine = 22;
    int _numTextLineCountPerPage = 4;

    public void SetCharacterText(Characters character, string text)
    {
        bool isEmpty = 0 == text.Length;

        gameObject.SetActive(!isEmpty);

        // set text
        _textName.GetComponent<TextMeshProUGUI>().text = character.ToString();
        _textText.GetComponent<TextMeshProUGUI>().text = "";

        // reset
        _paused = isEmpty;
        _textTime = 0.0f;
        _pageIndex = 0;
        
        // tuning text lines
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
    }

    public void OnClickReset()
    {
        SetCharacterText(Characters.Bright, "");
    }

    public void OnClickNext()
    {
        if(_paused)
        {
            _paused = false;
            _textTime = 0.0f;
            ++_pageIndex;

            // done
            if(_textList.Count <= _pageIndex)
            {
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

    void Start()
    {
    }

    void Update()
    {
        //Debug.Log("Update -  _paused: " + _paused.ToString() + ", _pageIndex: " + _pageIndex.ToString() + ", _textList.Count: " + _textList.Count.ToString());
        if(false == _paused && _pageIndex < _textList.Count)
        {
            string text = _textList[_pageIndex];
            int numText = Mathf.Min(text.Length, (int)_textTime + 1);
            _textText.GetComponent<TextMeshProUGUI>().text = text.Substring(0, numText);
            if(text.Length == numText)
            {
                _paused = true;
            }
            else
            {
                _textTime += Time.deltaTime * _textSpeed;
            }
        }
    }
}

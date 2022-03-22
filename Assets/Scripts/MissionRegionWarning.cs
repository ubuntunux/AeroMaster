using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionRegionWarning : MonoBehaviour
{
    public GameObject _missionRegionWarningText;
    public GameObject _missionRegionWarningImageLeft;
    public GameObject _missionRegionWarningImageRight;
    bool _isWarningRegionRight = false;

    // Start is called before the first frame update
    void Start()
    {
        ShowMissionRegionWarning(false, false);
    }

    public void ShowMissionRegionWarning(bool show, bool isWarningRegionRight)
    {
        _isWarningRegionRight = isWarningRegionRight;
        _missionRegionWarningText.SetActive(show);
        _missionRegionWarningImageLeft.SetActive(show && false == isWarningRegionRight);
        _missionRegionWarningImageRight.SetActive(show && isWarningRegionRight);
    }

    // Update is called once per frame
    void Update()
    {
        if(_missionRegionWarningText.activeSelf)
        {
            Color textColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            textColor.a = Mathf.Abs(Mathf.Sin(Time.time * Mathf.PI));            
            _missionRegionWarningText.GetComponent<TextMeshProUGUI>().color = textColor;

            if(_isWarningRegionRight)
            {
                _missionRegionWarningImageRight.GetComponent<RawImage>().color = textColor;
            }
            else
            {
                _missionRegionWarningImageLeft.GetComponent<RawImage>().color = textColor;
            }
        }
    }
}

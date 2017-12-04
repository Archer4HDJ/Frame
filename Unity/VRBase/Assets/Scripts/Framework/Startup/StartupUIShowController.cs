using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartupUIShowController : MonoBehaviour {

    public Slider slider;
    public Text Text_showContent;

    public void ShowProgress(int allNum,int currentNum)
    {
        float p = currentNum / allNum;
        slider.value = p;
    }

    public void ShowContent(string content)
    {
        Text_showContent.text = content;
    }
}

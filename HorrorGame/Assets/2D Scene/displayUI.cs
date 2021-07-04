using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class displayUI : MonoBehaviour
{
    public string myString;
    public TextMeshProUGUI hoverText;
    public float fadeTime;
    public bool displayInfo;

    void Start()
    {
        hoverText.color = Color.clear;
    }

    void Update()
    {
        FadeText();        
    }

    void OnMouseOver()
    {
        displayInfo = true;
    }

    void OnMouseExit()
    {
        displayInfo = false;
    }

    void FadeText()
    {
        if (displayInfo)
        {
            hoverText.text = myString;
            hoverText.color = Color.Lerp(hoverText.color, Color.white, fadeTime * Time.deltaTime);
        }
        else
        {
            hoverText.color = Color.Lerp(hoverText.color, Color.clear, fadeTime * Time.deltaTime);
        }
    }
}

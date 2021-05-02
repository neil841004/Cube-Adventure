using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLocalization : MonoBehaviour
{
    [Multiline(6)]
    public string enText;
    Text _text;

    void Awake()
    {
        _text = GetComponent<Text>();
    }

    void Start()
    {
        if (GameData.isEnglish) _text.text = enText;
    }
}

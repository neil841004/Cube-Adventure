using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownLocalization : MonoBehaviour
{
    public string[] enText;
    Dropdown _dropdown;


    // Start is called before the first frame update
    void Start()
    {
        _dropdown = this.GetComponent<Dropdown>();
        if (GameData.isEnglish)
        {
            for (int i = 0; i < enText.Length; i++)
            {
                _dropdown.options[i].text = enText[i];
            }
            transform.GetChild(0).GetComponent<Text>().text = enText[0];
        }
    }
}

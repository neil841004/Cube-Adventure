using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageLocalization : MonoBehaviour
{
    public Sprite en_Image;
    Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    void Start()
    {
        if (GameData.isEnglish) _image.sprite = en_Image;
    }
}

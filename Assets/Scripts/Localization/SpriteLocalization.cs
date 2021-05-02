using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteLocalization : MonoBehaviour
{
    public Sprite en_sprite;
    SpriteRenderer _sprite;


    void Start()
    {
        _sprite = this.GetComponent<SpriteRenderer>();
        if (GameData.isEnglish) _sprite.sprite = en_sprite;
    }
    
}

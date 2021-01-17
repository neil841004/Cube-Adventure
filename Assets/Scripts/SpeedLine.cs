using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SpeedLine : MonoBehaviour
{
    public PlayerMovement move;
    bool isFadeIn = false;
    Image _image;
    
    private void Start() {
        _image = this.GetComponent<Image>();    
    }

    void Update()
    {
        if (move.bodyDownCount >= 33 && move.bodyDown && !isFadeIn)
        {
            _image.DOFade(0.3f, 0.4f).SetEase(Ease.InCubic);
            isFadeIn = true;
        }
        if (!move.bodyDown)
        {
            _image.DOFade(0, 0.4f);
            isFadeIn = false;
        }
    }
}

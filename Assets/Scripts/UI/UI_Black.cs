using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UI_Black : MonoBehaviour
{
    Image _image;
    private void Start() {
        _image = this.GetComponent<Image>();    
    }

    public void FadeIn(){
        _image.DOFade(1, .3f).SetEase(Ease.OutSine);
    }
    public void FadeOut(){
        _image.DOFade(0, .3f).SetEase(Ease.OutSine);
    }
}

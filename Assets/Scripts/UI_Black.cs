using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UI_Black : MonoBehaviour
{
    public void FadeIn(){
        this.GetComponent<Image>().DOFade(1, .3f).SetEase(Ease.OutSine);
    }
    public void FadeOut(){
        this.GetComponent<Image>().DOFade(0, .3f).SetEase(Ease.OutSine);
    }
}

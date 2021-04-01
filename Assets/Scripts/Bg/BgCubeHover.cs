using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BgCubeHover : MonoBehaviour
{
    Tween hover;
    
    void Start()
    {
        hover = transform.DOMoveY(Random.Range(4,8),Random.Range(7,12)).SetEase(Ease.InOutQuad).SetRelative(true).SetLoops(-1,LoopType.Yoyo).SetDelay(Random.Range(0,2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

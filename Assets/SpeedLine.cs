using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SpeedLine : MonoBehaviour
{
    public PlayerMovement move;
    bool isFadeIn = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (move.bodyDownCount >= 33 && move.bodyDown && !isFadeIn)
        {
            this.GetComponent<Image>().DOFade(0.3f, 0.4f).SetEase(Ease.InCubic);
            isFadeIn = true;
        }
        if (!move.bodyDown)
        {
            this.GetComponent<Image>().DOFade(0, 0.4f);
            isFadeIn = false;
        }
    }
}

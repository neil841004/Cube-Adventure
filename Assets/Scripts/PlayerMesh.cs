using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMesh : MonoBehaviour
{
    public Color colorOringal, colorDash, color;
    public PlayerMovement move;
    float thick = 5.4f;
    float fadeTime = 1f;
    Tween colorTween;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (move.hasDashed)
        {
            colorTween = DOTween.To(() => color, x => color = x, colorOringal, fadeTime);
            colorTween = DOTween.To(() => thick, x => thick = x, 5.4f, fadeTime * 0.8f);
        }
        else if (!move.hasDashed)
        {
            colorTween = DOTween.To(() => color, x => color = x, colorDash, fadeTime);
            colorTween = DOTween.To(() => thick, x => thick = x, 11f, fadeTime);
        }
        this.GetComponent<MeshRenderer>().material.SetColor("_OutLineColor", color);
        this.GetComponent<MeshRenderer>().material.SetFloat("_EdgeThickness", thick);
    }
}

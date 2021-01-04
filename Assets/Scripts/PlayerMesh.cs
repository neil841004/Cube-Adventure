using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMesh : MonoBehaviour
{
    public Color colorOringal, colorDash, color;
    public PlayerMovement move;
    public GameObject GetCoinParticle;
    float thick = 5.4f;
    float fadeTime = 1f;
    Tween colorTween, scaleTween;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (move.hasDashed && !move.isWin)
        {
            colorTween = DOTween.To(() => color, x => color = x, colorOringal, fadeTime);
            colorTween = DOTween.To(() => thick, x => thick = x, 5.4f, fadeTime * 0.8f);
        }
        if (!move.hasDashed || move.isWin)
        {
            colorTween = DOTween.To(() => color, x => color = x, colorDash, fadeTime);
            colorTween = DOTween.To(() => thick, x => thick = x, 11f, fadeTime);
        }
        this.GetComponent<MeshRenderer>().material.SetColor("_OutLineColor", color);
        this.GetComponent<MeshRenderer>().material.SetFloat("_EdgeThickness", thick);
    }
    void SendAnim()
    {
        scaleTween.Kill();
        this.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        scaleTween = this.transform.DOScale(1f, 0.8f).SetEase(Ease.OutElastic);
    }

    void GetCoin()
    {
        scaleTween.Kill();
        this.transform.localScale = new Vector3(0.72f, 0.72f, 0.72f);
        scaleTween = this.transform.DOScale(1f, 0.8f).SetEase(Ease.OutElastic);
    }
}

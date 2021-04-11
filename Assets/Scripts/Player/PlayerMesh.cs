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
    public float coinPitch = 0;
    int getCoinTimeCount = 0;
    bool getCoin_3sec = false;
    Tween colorTween, scaleTween;
    Material _material;
    public Material _trail;

    private void Start()
    {
        _material = this.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if (move.hasDashed || move.isWin)
        {
            colorTween = DOTween.To(() => color, x => color = x, colorOringal, fadeTime);
            colorTween = DOTween.To(() => thick, x => thick = x, 5.4f, fadeTime * 0.8f);
        }
        if (!move.hasDashed)
        {
            colorTween = DOTween.To(() => color, x => color = x, colorDash, fadeTime);
            colorTween = DOTween.To(() => thick, x => thick = x, 11f, fadeTime);
        }
        _material.SetColor("_OutLineColor", color);
        _material.SetFloat("_EdgeThickness", thick);
        _trail.SetColor("_TintColor", color);
    }
    void SendAnim()
    {
        scaleTween.Kill();
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        scaleTween = transform.DOScale(1f, 0.8f).SetEase(Ease.OutElastic);
    }

    void GetCoin()
    {
        if (getCoin_3sec)
        {
            coinPitch += 0.04f;
        }
        scaleTween.Kill();
        transform.localScale = new Vector3(0.72f, 0.72f, 0.72f);
        scaleTween = transform.DOScale(1f, 0.8f).SetEase(Ease.OutElastic);
        StopCoroutine("GetCoinEnumerator");
        StartCoroutine("GetCoinEnumerator");
    }
    IEnumerator GetCoinEnumerator()
    {
        getCoin_3sec = true;
        yield return new WaitForSeconds(3);
        getCoin_3sec = false;
        coinPitch = 0;
    }
}

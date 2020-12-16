﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DisappearPF : MonoBehaviour
{
    float alpha = 1;
    Vector3 originPosition;

    // Start is called before the first frame update
    void Start()
    {
        originPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<MeshRenderer>().material.SetFloat("_Alpha", alpha);

    }

    void FadePF() { 
        
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            Sequence seq = DOTween.Sequence();
            seq.Append(this.transform.DOLocalMoveY(-4f, 1.2f).SetEase(Ease.InQuad));
            seq.Insert(0.85f,DOTween.To(() => alpha, x => alpha = x, 0, 0.3f));
            seq.InsertCallback(0.9f,CloseCollider);
        }
    }
    void CloseCollider() {
        this.GetComponent<BoxCollider>().enabled = false;
        StartCoroutine("RebootCollider");
    }

    IEnumerator RebootCollider() {
        yield return new WaitForSeconds(2.2f);
        this.transform.position = originPosition;
        DOTween.To(() => alpha, x => alpha = x, 1, 0.3f);
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<BoxCollider>().enabled = true;
    }

}

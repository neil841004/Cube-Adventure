using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DisappearPF : MonoBehaviour
{
    float alpha = 1;
    bool aniPlaying = false;
    Vector3 originPosition;
    Material _material;

    // Start is called before the first frame update
    void Start()
    {
        originPosition = this.transform.position;
        _material = this.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (aniPlaying) _material.SetFloat("_Alpha", alpha);
    }


    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        

        if (collision.gameObject.CompareTag("Player") && collision.contacts[0].normal.y != 1)
        {
            StopCoroutine("RebootCollider");
            aniPlaying = true;
            Sequence seq = DOTween.Sequence();
            seq.Append(this.transform.DOBlendableMoveBy(new Vector3(0, -4.5f, 0), 0.9f).SetEase(Ease.InCubic));
            seq.Insert(0.6f, DOTween.To(() => alpha, x => alpha = x, 0, 0.3f));
            seq.InsertCallback(0.75f, CloseCollider);
        }
    }
    void CloseCollider()
    {
        this.GetComponent<BoxCollider>().enabled = false;
        StartCoroutine("RebootCollider");
    }

    IEnumerator RebootCollider()
    {
        yield return new WaitForSeconds(2.2f);
        this.transform.position = originPosition;
        DOTween.To(() => alpha, x => alpha = x, 1, 0.3f);
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(0.25f);
        aniPlaying = false;
    }

}

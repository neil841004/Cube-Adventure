using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RhythmPF : MonoBehaviour
{
    public float delayStartTime = 0;
    public float standbyTime = 2;
    public GameObject mesh;
    Sequence seq;
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartTrap");
        gm = GameObject.FindWithTag("GM").GetComponent<GameManager>();
    }


    IEnumerator StartTrap() {
        yield return new WaitForSeconds(delayStartTime);
        seq = DOTween.Sequence();
        seq.Append(this.transform.DOLocalRotate(new Vector3(-180, 0, 0), 0.3f, RotateMode.LocalAxisAdd));
        seq.AppendInterval(standbyTime - 0.8f);
        seq.Append(mesh.transform.DOShakePosition(0.5f, new Vector3(0.08f, 0.15f, 0.1f), 16, fadeOut: false));
        seq.Append(this.transform.DOLocalRotate(new Vector3(-180 , 0, 0), 0.3f, RotateMode.LocalAxisAdd));
        seq.AppendInterval(standbyTime - 0.8f);
        seq.Append(mesh.transform.DOShakePosition(0.5f, new Vector3(0.08f, 0.15f, 0.1f), 16, fadeOut: false));
        seq.SetLoops(-1);
    }

    public void Update() {
        if(gm.reTrap) seq.Restart();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovePF_Z : MonoBehaviour
{
    public float delayStartTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartTrap");
    }

    IEnumerator StartTrap() {
        yield return new WaitForSeconds(delayStartTime);
        Sequence seq = DOTween.Sequence();
        seq.Append(this.transform.DOLocalMoveZ(2, 0.7f));
        seq.Insert(0.2f,this.transform.transform.DOLocalRotate(new Vector3(-180, 0, 0), 0.3f, RotateMode.LocalAxisAdd));
        seq.AppendInterval(1);
        seq.Append(this.transform.DOLocalMoveZ(-0.5f, 0.7f));
        seq.Insert(1.9f, this.transform.transform.DOLocalRotate(new Vector3(180 , 0, 0), 0.3f, RotateMode.LocalAxisAdd));
        seq.AppendInterval(1);
        seq.SetLoops(-1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RhythmPF : MonoBehaviour
{
    public float delayStartTime = 0;
    public float standbyTime = 2;
    public GameObject mesh;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartTrap");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartTrap() {
        yield return new WaitForSeconds(delayStartTime);
        Sequence seq = DOTween.Sequence();
        seq.Append(this.transform.DOLocalRotate(new Vector3(-180, 0, 0), 0.3f, RotateMode.LocalAxisAdd));
        seq.AppendInterval(standbyTime - 0.8f);
        seq.Append(mesh.transform.DOShakePosition(0.5f, new Vector3(0.08f, 0.15f, 0.1f), 16, fadeOut: false));
        seq.Append(this.transform.DOLocalRotate(new Vector3(-180 , 0, 0), 0.3f, RotateMode.LocalAxisAdd));
        seq.AppendInterval(standbyTime - 0.8f);
        seq.Append(mesh.transform.DOShakePosition(0.5f, new Vector3(0.08f, 0.15f, 0.1f), 16, fadeOut: false));
        seq.SetLoops(-1);
    }
}

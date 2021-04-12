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
    AudioSource _sound;
    public AudioClip[] _clip = new AudioClip[2];

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartTrap");
        gm = GameObject.FindWithTag("GM").GetComponent<GameManager>();
        _sound = GetComponent<AudioSource>();
    }


    IEnumerator StartTrap() {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return new WaitForSeconds(delayStartTime);
        seq = DOTween.Sequence();
        seq.Append(this.transform.DOLocalRotate(new Vector3(-180, 0, 0), 0.3f, RotateMode.LocalAxisAdd));
        seq.AppendInterval(standbyTime - 0.5f);
        seq.AppendCallback(ShakeSound);
        seq.Append(mesh.transform.DOShakePosition(0.5f, new Vector3(0.08f, 0.15f, 0.1f), 16, fadeOut: false).OnComplete(FlipSound));
        seq.Append(this.transform.DOLocalRotate(new Vector3(-180 , 0, 0), 0.3f, RotateMode.LocalAxisAdd));
        seq.AppendInterval(standbyTime - 0.5f).OnComplete(ShakeSound);
        seq.AppendCallback(ShakeSound);
        seq.Append(mesh.transform.DOShakePosition(0.5f, new Vector3(0.08f, 0.15f, 0.1f), 16, fadeOut: false).OnComplete(FlipSound));
        seq.SetLoops(-1);
    }

    public void Update() {
        if (gm.reTrap){
            StopCoroutine("DelayStartTrap");
            StartCoroutine("DelayStartTrap");
        }
    }
    IEnumerator DelayStartTrap() {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return new WaitForSeconds(delayStartTime);
        seq.Restart();
    }
    void ShakeSound(){
        _sound.PlayOneShot(_clip[0],0.7f);
    }
    void FlipSound(){
        _sound.PlayOneShot(_clip[1],0.7f);
    }


}

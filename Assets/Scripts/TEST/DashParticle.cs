using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DashParticle : MonoBehaviour
{
    GameObject player;
    bool _isAnimDash = false;
    bool canPlayParticle = true;
    bool isLocalRotate = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {
        _isAnimDash = player.GetComponent<PlayerMovement>().isAnimDash;
        if (_isAnimDash && canPlayParticle) {
            isLocalRotate = true;
            GetComponent<ParticleSystem>().Play();
            canPlayParticle = false;
            StartCoroutine("LocalRotate");
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            transform.DOLocalRotate(new Vector3(0, 0, -200), 0.28f, RotateMode.FastBeyond360).SetEase(Ease.OutSine);
        }
        if(isLocalRotate) this.transform.position = player.transform.position;
        if (!_isAnimDash) canPlayParticle = true;
    }

    IEnumerator LocalRotate() {
        yield return new WaitForSeconds(0.125f);
        isLocalRotate = false;
    }
}

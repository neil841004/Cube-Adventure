using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Portal : MonoBehaviour
{
    public GameObject destination;
    Tween portalScaleTween, portalScaleBackTween;
    bool isClose = false;
    BoxCollider _boxCollider, _otherBoxCollider;
    SoundEffectManager _sound;

    private void Start()
    {
        _boxCollider = this.GetComponent<BoxCollider>();
        _otherBoxCollider = destination.GetComponent<BoxCollider>();
        _sound = GameObject.Find("Level_SE_Manager").GetComponent<SoundEffectManager>();
    }

    public void PortalStart()
    {
        if (!isClose)
        {
            _boxCollider.enabled = false;
            _otherBoxCollider.enabled = false;
            portalScaleBackTween.Kill();
            StopCoroutine("PortalEenumerator");
            portalScaleTween = this.transform.GetChild(0).DOScale(0.4f, 0.2f);
            portalScaleTween = destination.transform.GetChild(0).DOScale(0.4f, 0.2f);
            StartCoroutine("PortalEenumerator");
            _sound.PlayOneSound(1,0.7f);
        }
        isClose = true;
    }
    IEnumerator PortalEenumerator()
    {
        yield return new WaitForSeconds(1f);

        portalScaleBackTween = this.transform.GetChild(0).DOScale(1f, 0.3f);
        portalScaleBackTween = destination.transform.GetChild(0).DOScale(1f, 0.3f);
        yield return new WaitForSeconds(0.1f);
        _boxCollider.enabled = true;
        _otherBoxCollider.enabled = true;
        isClose = false;
    }
}

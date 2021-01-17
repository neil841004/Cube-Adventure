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

    private void Start()
    {
        _boxCollider = this.GetComponent<BoxCollider>();
        _otherBoxCollider = destination.GetComponent<BoxCollider>();
    }

    public void PortalStart()
    {
        if (!isClose)
        {
            Debug.Log("A");

            _boxCollider.enabled = false;
            _otherBoxCollider.enabled = false;
            portalScaleBackTween.Kill();
            StopCoroutine("PortalEenumerator");
            portalScaleTween = this.transform.GetChild(0).DOScale(0.4f, 0.2f);
            portalScaleTween = destination.transform.GetChild(0).DOScale(0.4f, 0.2f);
            StartCoroutine("PortalEenumerator");
        }
        isClose = true;
    }
    IEnumerator PortalEenumerator()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("B");

        portalScaleBackTween = this.transform.GetChild(0).DOScale(1f, 0.3f);
        portalScaleBackTween = destination.transform.GetChild(0).DOScale(1f, 0.3f);
        yield return new WaitForSeconds(0.1f);
        _boxCollider.enabled = true;
        _otherBoxCollider.enabled = true;
        isClose = false;
    }
}

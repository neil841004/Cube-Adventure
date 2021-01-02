using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Portal : MonoBehaviour
{
    public GameObject destination;
    Tween portalScaleTween,portalScaleBackTween;
    bool isClose = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PortalStart()
    {
        if (!isClose)
        {
            Debug.Log("A");

            this.GetComponent<BoxCollider>().enabled = false;
            destination.GetComponent<BoxCollider>().enabled = false;
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
        this.GetComponent<BoxCollider>().enabled = true;
        destination.GetComponent<BoxCollider>().enabled = true;
        isClose = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraState : MonoBehaviour
{
    GameObject player;
    bool isblendY = false;
    bool isblendX = false;
    CinemachineVirtualCamera vCamera;
    float screenY, screenX;
    Tween tweenY, tweenX;
    public float blendTime = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        vCamera = GetComponent<CinemachineVirtualCamera>();
    }


    public void CameraStop()
    {
        vCamera.Follow = null;
    }
    public void CameraStart()
    {
        vCamera.Follow = player.transform;
    }

    public void ScreenYMove(float y)
    {
        tweenY.Kill();
        StopCoroutine("CloseBlendY");
        screenY = vCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY;
        isblendY = true;
        tweenY = DOTween.To(() => screenY, x => screenY = x, y, blendTime);
        StartCoroutine("CloseBlendY");
    }
    public void ScreenXMove(float x)
    {
        tweenX.Kill();
        StopCoroutine("CloseBlendX");
        screenX = vCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX;
        isblendX = true;
        tweenX = DOTween.To(() => screenX, m => screenX = m, x, blendTime);
        StartCoroutine("CloseBlendX");
    }

    private void Update()
    {
        if (isblendY)
        {
            vCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = screenY;
        }
        if (isblendX)
        {
            vCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = screenX;
        }
    }
    IEnumerator CloseBlendY()
    {
        yield return new WaitForSeconds(blendTime + 0.1f);
        isblendY = false;
    }

    IEnumerator CloseBlendX()
    {
        yield return new WaitForSeconds(blendTime + 0.1f);
        isblendX = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class SimpleCameraShakeInCinemachine : MonoBehaviour
{

    public float ShakeDuration = 0.3f;          // Time the Camera Shake effect will last
    public float ShakeAmplitude = 5f;         // Cinemachine Noise Profile Parameter
    public float ShakeFrequency = 0.08f;         // Cinemachine Noise Profile Parameter

    private float ShakeElapsedTime = 0f;
    private float originalLen;

    private int bodyDownCount = 0;

    // Cinemachine Shake
    public CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    Tweener shakeTimeS, shakeTimeL;

    VolumeProfile profile;
    ChromaticAberration myChromaticAberration;
    PlayerMovement move;

    // Use this for initialization
    void Start()
    {
        // Get Virtual Camera Noise Profile
        if (VirtualCamera != null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        profile = GameObject.Find("PostEffect").GetComponent<Volume>().profile;
        originalLen = VirtualCamera.m_Lens.FieldOfView;
        move = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        bodyDownCount = move.bodyDownCount;
        // If the Cinemachine componet is not set, avoid update
        if (VirtualCamera != null && virtualCameraNoise != null)
        {
            // If Camera Shake effect is still playing
            if (ShakeElapsedTime > 0)
            {
                // Set Cinemachine Camera Noise parameters
                virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
                virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

                // Update Shake Timer
                ShakeElapsedTime -= Time.deltaTime;
            }
            else
            {
                // If Camera Shake effect is over, reset variables
                virtualCameraNoise.m_AmplitudeGain = 0f;
                ShakeElapsedTime = 0f;
            }

            if (bodyDownCount > 0)
            {
                if (ShakeElapsedTime <= 0)
                {
                    if (bodyDownCount < 42)
                    {
                        virtualCameraNoise.m_AmplitudeGain = bodyDownCount * 0.05f;
                        virtualCameraNoise.m_FrequencyGain = bodyDownCount * 0.0025f;
                    }else if (bodyDownCount == 42)
                    {
                        virtualCameraNoise.m_AmplitudeGain = bodyDownCount * 0.05f;
                        virtualCameraNoise.m_FrequencyGain = bodyDownCount * 0.0025f;
                    }
                }
                profile.TryGet(out myChromaticAberration);
                myChromaticAberration.intensity.Override(bodyDownCount * 0.0238f);
                VirtualCamera.m_Lens.FieldOfView = originalLen +  (bodyDownCount * 0.1f);
            }
        }
    }

    public void ScreenShake_Dash()
    {

        ShakeAmplitude = 4f;
        ShakeFrequency = 0.2f;
        ShakeElapsedTime = 0.55f;
        shakeTimeL.Kill();
        shakeTimeS.PlayForward();
        shakeTimeS = DOTween.To(() => ShakeAmplitude, x => ShakeAmplitude = x, 0, 0.55f).SetEase(Ease.OutSine);
    }
    public void ScreenShake_Death()
    {

        ShakeAmplitude = 9f;
        ShakeFrequency = 0.06f;
        ShakeElapsedTime = 1.5f;
        shakeTimeS.Kill();
        shakeTimeL.PlayForward();
        shakeTimeL = DOTween.To(() => ShakeAmplitude, x => ShakeAmplitude = x, 0, 1.5f).SetEase(Ease.OutSine);
    }
    public void ScreenShake_DownJump()
    {

        ShakeAmplitude = 8f;
        ShakeFrequency = 0.12f;
        ShakeElapsedTime = 0.65f;
        shakeTimeS.Kill();
        shakeTimeL.PlayForward();
        shakeTimeL = DOTween.To(() => ShakeAmplitude, x => ShakeAmplitude = x, 0, 0.65f).SetEase(Ease.OutSine);
    }
}
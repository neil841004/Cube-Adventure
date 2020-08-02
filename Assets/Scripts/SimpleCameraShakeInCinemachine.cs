using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using DG.Tweening;

public class SimpleCameraShakeInCinemachine : MonoBehaviour
{

    public float ShakeDuration = 0.3f;          // Time the Camera Shake effect will last
    public float ShakeAmplitude = 5f;         // Cinemachine Noise Profile Parameter
    public float ShakeFrequency = 0.08f;         // Cinemachine Noise Profile Parameter

    private float ShakeElapsedTime = 0f;

    // Cinemachine Shake
    public CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    Tweener shakeTimeS,shakeTimeL;

    // Use this for initialization
    void Start()
    {
        // Get Virtual Camera Noise Profile
        if (VirtualCamera != null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Replace with your trigger
        if (Input.GetKey(KeyCode.S))
        {
            ScreenShake_L();
        }

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
        }
    }
    public void ScreenShake_S()
    {
        VirtualCamera.GetCinemachineComponent<Cinemachine.NoiseSettings>() = none;
        ShakeAmplitude = 4f;
        ShakeFrequency = 0.5f;
        ShakeElapsedTime = .6f;
        DOTween.To(() => ShakeAmplitude, x => ShakeAmplitude = x, 0, .6f).SetEase(Ease.OutSine);
    }
    public void ScreenShake_L()
    {
        ShakeAmplitude = 6f;
        ShakeFrequency = 0.09f;
        ShakeElapsedTime = 1f;
        shakeTimeL.PlayForward();
        shakeTimeL = DOTween.To(() => ShakeAmplitude, x => ShakeAmplitude = x, 0, 1f).SetEase(Ease.OutSine);
    }
}
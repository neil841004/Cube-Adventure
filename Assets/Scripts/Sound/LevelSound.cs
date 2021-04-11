using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public AudioClip[] audioClip;
    AudioSource singleAudio;

    private void Start()
    {
        singleAudio = this.GetComponent<AudioSource>();
    }

    public void PlayOneSound(int number, float volume)
    {
        singleAudio.pitch = 1;
        singleAudio.PlayOneShot(audioClip[number], volume);
    }

    public void PlayOneSound(int number, float volume, float pitchRandom)
    {
        singleAudio.pitch = Random.Range(1 + pitchRandom, 1 - pitchRandom);
        singleAudio.PlayOneShot(audioClip[number], volume);
    }

    public void PlayOneSoundFixedPitch(int number, float volume, float pitch)
    {
        singleAudio.pitch = 0.92f + pitch;
        singleAudio.PlayOneShot(audioClip[number], volume);
    }    

}

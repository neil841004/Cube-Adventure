using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] audioClip;
    AudioSource singleAudio;

    public void PlayOneSound(int number, float volume)
    {
        singleAudio.PlayOneShot(audioClip[number], volume);
    }

}

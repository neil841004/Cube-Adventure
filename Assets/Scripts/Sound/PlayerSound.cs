using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource singleAudio;
    public AudioSource loopAudio;
    public AudioSource canStopAudio;
    public AudioClip[] audioClip;
    public bool strongLanding = false;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PlayOneSound(int number)
    {
        singleAudio.pitch = 1;
        singleAudio.PlayOneShot(audioClip[number], 0.7f);
    }
    public void LandingSound()
    {
        singleAudio.pitch = Random.Range(1f, 0.68f);
        if (strongLanding)
        {
            singleAudio.PlayOneShot(audioClip[2], 0.5f);
            singleAudio.PlayOneShot(audioClip[3], 0.2f);
        }
        else
            singleAudio.PlayOneShot(audioClip[2], 0.32f);
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

    public void PlaySound(int number)
    {
        singleAudio.pitch = 1;
        canStopAudio.clip = audioClip[number];
        canStopAudio.Play();
    }

    public void PlaySound(int number, float volume)
    {
        singleAudio.pitch = 1;
        canStopAudio.clip = audioClip[number];
        canStopAudio.volume = volume;
        canStopAudio.Play();
    }

    public void StopSound(int number)
    {
        canStopAudio.clip = audioClip[number];
        canStopAudio.Stop();
    }

    public void PlayLoopSound(int number, float volume)
    {
        singleAudio.pitch = 1;
        loopAudio.clip = audioClip[number];
        loopAudio.volume = volume;
        loopAudio.Play();
    }
    public void StopLoopSound(int number)
    {
        loopAudio.clip = audioClip[number];
        loopAudio.Stop();
    }
}

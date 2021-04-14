using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSound : MonoBehaviour
{
    public AudioSource singleAudio;
    public AudioSource loopAudio;
    public AudioSource canStopAudio;
    public AudioClip[] audioClip;
    public bool strongLanding = false;
    PlayerMovement _player;
    public AudioMixer mixer;



    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_player.isDeath)
        {
            mixer.SetFloat("TrapVolume", -80);
        }
        else if (!_player.isDeath)
        {
            mixer.SetFloat("TrapVolume", 0);
        }
        if (_player.bodyDown)
        {
            mixer.SetFloat("TrapLowPass",700);
        }else if (!_player.bodyDown)
        {
            mixer.SetFloat("TrapLowPass",22000);
        }
    }
    public void PlayOneSound(int number)
    {
        singleAudio.pitch = 1;
        singleAudio.PlayOneShot(audioClip[number], 0.7f);
    }
    public void LandingSound()
    {
        if (!_player.isDeath)
        {
            singleAudio.pitch = Random.Range(1f, 0.68f);
            if (strongLanding)
            {
                singleAudio.PlayOneShot(audioClip[2], 0.16f);
                singleAudio.PlayOneShot(audioClip[3], 0.13f);
            }
            else
                singleAudio.PlayOneShot(audioClip[2], 0.11f);
        }
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

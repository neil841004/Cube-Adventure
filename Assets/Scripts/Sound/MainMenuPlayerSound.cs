using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPlayerSound : MonoBehaviour
{
    AudioSource _audio;
    public AudioClip dancingSound;
    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void PlayDancingSound(float volume)
    {
        _audio.PlayOneShot(dancingSound,volume);
    }
}

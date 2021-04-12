using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDelaySound : MonoBehaviour
{
    AudioSource _sound;
    float delayTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        _sound = GetComponent<AudioSource>();
    }

    public void PlayTrapSound(float i)
    {
        delayTime = i;
        StartCoroutine("TrapSoundIEnumerator");
    }
    IEnumerator TrapSoundIEnumerator()
    {
        yield return new WaitForSeconds(delayTime);
        _sound.Play();
    }
}

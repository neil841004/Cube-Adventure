using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTitleSound : MonoBehaviour
{
    SoundEffectManager _sound;
    // Start is called before the first frame update
    void Start()
    {
        _sound = GameObject.FindWithTag("GM").GetComponent<SoundEffectManager>();
    }

    // Update is called once per frame
    public void Start_1()
    {
        _sound.PlayOneSound(1, 0.7f);
    }
    public void Start_2()
    {
        _sound.PlayOneSound(2, 0.7f);
    }
}

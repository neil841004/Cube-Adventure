﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent death = new UnityEvent();
    public UnityEvent resetLevel = new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Death()
    {
        death.Invoke();
    }
    public void ResetLevel()
    {
        resetLevel.Invoke();
    }
}

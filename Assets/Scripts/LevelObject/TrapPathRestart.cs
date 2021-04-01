using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrapPathRestart : MonoBehaviour
{
    DOTweenPath ani;   
    GameManager gm; 
    void Start()
    {
        ani = GetComponent<DOTweenPath>();
        gm = GameObject.FindWithTag("GM").GetComponent<GameManager>();
    }

    public void Update() {
        if(gm.reTrap) ani.DORestart();
    }
}

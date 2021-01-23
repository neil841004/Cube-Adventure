using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrapRestart : MonoBehaviour
{
    DOTweenAnimation ani;   
    GameManager gm; 
    void Start()
    {
        ani = GetComponent<DOTweenAnimation>();
        gm = GameObject.FindWithTag("GM").GetComponent<GameManager>();
    }

    public void Update() {
        if(gm.reTrap) ani.DORestart();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapNumberReturn : MonoBehaviour
{
    public int trapNumber = 0;
    GameManager gm;

    private void Start() {
        gm = GameObject.FindWithTag("GM").GetComponent<GameManager>();
        if (trapNumber == 0) trapNumber = 5;
    }
    
    private void OnTriggerEnter(Collider co) {
        if(co.CompareTag("Player")) gm.SendMessage("TrapNumberRecord",trapNumber);
    }
}

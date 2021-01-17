﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheckPoint : MonoBehaviour
{
    private Animator anim;
    PlayerMovement move;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        move = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame


    IEnumerator RebirthAnimIenumerator()
    {
        yield return new WaitForSeconds(1.15f);

            Debug.Log("A");
            anim.Play("CheckPoint_Revival",0,0);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            move.checkPoint = this.gameObject;
            move.checkPointV3 = this.transform.position;
            anim.SetBool("enter", true);

            this.GetComponent<BoxCollider>().enabled = false;
        }
    }
}

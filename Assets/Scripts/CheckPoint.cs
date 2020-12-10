﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().checkPoint = this.transform.position;
            this.GetComponent<BoxCollider>().enabled = false;
        }
    }
}

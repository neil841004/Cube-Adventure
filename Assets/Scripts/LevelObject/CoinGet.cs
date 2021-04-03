using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGet : MonoBehaviour
{
    Coin parent;

    private void Start() {
        parent = this.transform.parent.GetComponent<Coin>();    
    }

    private void OnTriggerEnter(Collider co)
    {
        if (co.CompareTag("Player"))
        {
            parent.SendMessage("PickCoin");
        }

    }
}

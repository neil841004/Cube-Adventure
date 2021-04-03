using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinFail : MonoBehaviour
{
    Coin parent;
    private void Start()
    {
        parent = this.transform.parent.GetComponent<Coin>();
    }

    private void OnTriggerExit(Collider co)
    {
        if (co.CompareTag("Player"))
        {
            parent.SendMessage("NotPickCoin");
        }
    }
}

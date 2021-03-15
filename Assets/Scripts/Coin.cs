using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public ObjectPool pool;
    PlayerMesh playerMesh;
    

    void Start()
    {
        playerMesh = GameObject.FindWithTag("PlayerMesh").GetComponent<PlayerMesh>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerMesh.SendMessage("GetCoin");
            pool.ReUse(transform.position, Quaternion.Euler(-90, 0, 0));
            this.gameObject.SetActive(false);
        }

    }

}

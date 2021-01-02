using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public ObjectPool pool;
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
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject.FindWithTag("PlayerMesh").GetComponent<PlayerMesh>().SendMessage("GetCoin");
            pool.ReUse(transform.position,Quaternion.Euler(-90, 0, 0));
            this.gameObject.SetActive(false);
        }
        
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGParticle : MonoBehaviour
{
    public Transform player;
    ParticleSystem bg;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var bg = this.GetComponent<ParticleSystem>().shape;
        bg.position = new Vector3(player.position.x,player.position.y,0);
    }
}

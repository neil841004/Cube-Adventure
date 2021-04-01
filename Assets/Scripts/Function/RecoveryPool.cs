using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryPool : MonoBehaviour
{
    public float recoveryTime = 3.0f;
 
    private float _timer;
 
    private Transform _myTransform;
 
    void Awake()
    {
        _myTransform = transform;
    }
 
 
    void OnEnable()
    {
        _timer = Time.time;
    }
 
 
    void Update ()
    {
        if( !gameObject.activeInHierarchy )
            return;
 
        if( Time.time > _timer + recoveryTime )
        {
            GameObject.Find( "CoinParticlePool" ).GetComponent<ObjectPool>().Recovery( gameObject );
        }
 
    }
    
}

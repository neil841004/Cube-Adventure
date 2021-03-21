using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraState : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CameraStop(){
        GetComponent<CinemachineVirtualCamera>().Follow = null;
    }
    public void CameraStart(){
        GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent enterEvent = new UnityEvent();
    public UnityEvent exitEvent = new UnityEvent();
    
    private void OnTriggerEnter(Collider co) {
        if(co.CompareTag("Player")){
            enterEvent.Invoke();
        }
    }
    private void OnTriggerExit(Collider co) {
        if(co.CompareTag("Player")){
            exitEvent.Invoke();
        }
    }
}

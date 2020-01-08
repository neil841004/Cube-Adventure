using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("PrintText");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)){
            StopCoroutine("PrintText");
        }
    }
    IEnumerator PrintText(){
        Debug.Log("A");
        yield return new WaitForSeconds(5);
        Debug.Log("B");
    }
}

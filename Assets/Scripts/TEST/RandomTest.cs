using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTest : MonoBehaviour
{
    int i = 0;
    int i_0 = 0;
    int i_1 = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int j = 0; j < 5000; j++)
        {
            i = Random.Range(0,2);
            if(i == 0) i_0 ++;
            if(i == 1) i_1 ++;
        }
        Debug.Log(i_0 + " , " + i_1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

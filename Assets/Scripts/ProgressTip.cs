using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressTip : MonoBehaviour
{
    public GameObject checkPoints;
    public GameObject endTip;
    
    int cpCount; //CP總數
    int cpNumber = 0; //經過的編號
    // Start is called before the first frame update
    void Start()
    {
        if (checkPoints)
        {
            cpCount = checkPoints.transform.childCount;
            for (int i = 0; i < cpCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            endTip.SetActive(true);
            endTip.transform.position = transform.GetChild(cpCount).position;
        }
        
    }


    public void PassCP()
    {
        transform.GetChild(cpNumber).GetComponent<Animator>().SetBool("isPass", true);
        cpNumber++;
    }

    public void PassEnd()
    {
        endTip.GetComponent<Animator>().SetBool("isPass", true);
    }
}

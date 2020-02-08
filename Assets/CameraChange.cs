using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    public int CMNumber = 1;

    public void CMChange(int number)
    {
        for (int i = 0; i < CMNumber; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        transform.GetChild(number).gameObject.SetActive(true);
    }
}

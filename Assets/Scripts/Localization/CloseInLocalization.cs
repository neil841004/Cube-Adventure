using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseInLocalization : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameData.isEnglish) this.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeclectLocalization : MonoBehaviour
{
    public void CH(){
        GameData.isEnglish = false;
    }

    public void EN(){
        GameData.isEnglish = true;
    }
}

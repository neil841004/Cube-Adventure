using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LevelTitle : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendInterval(0.9f); 
        mySequence.Append(GetComponent<TMPro.TextMeshProUGUI>().DOFade(1, 0.4f));
        mySequence.AppendInterval(2.4f); 
        mySequence.Append(GetComponent<TMPro.TextMeshProUGUI>().DOFade(0, 0.7f));
        

    }

    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TitleLogo : MonoBehaviour
{
    public float delay = 1f;
    Image[] logo = new Image[5];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            logo[i] = transform.GetChild(i).GetComponent<Image>();
        }
    }

    // Update is called once per frame
    public void ShowLogo()
    {
        StartCoroutine("ShowLogoIEnumerator");
    }
    IEnumerator ShowLogoIEnumerator()
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < 5; i++)
        {
            logo[i].DOFade(1, 0.8f);
        }
    }
    public void CloseLogo()
    {
        for (int i = 0; i < 5; i++)
        {
            logo[i].DOFade(0, 0.6f);
        }
    }

}

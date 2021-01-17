using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GM_Tutorial : MonoBehaviour
{
    public GameObject[] text = new GameObject[5];
    public DOTweenAnimation[] t1_sawTween = new DOTweenAnimation[8];
    public DOTweenAnimation[] t3_sawTween = new DOTweenAnimation[17];
    public DOTweenAnimation[] t4_PF = new DOTweenAnimation[2];
    public Collider topColl;
    public int t = -1;
    public GameObject t2_platform;
    bool onPF = false;
    PlayerMovement move;
    Collision collision;


    // Start is called before the first frame update
    void Start()
    {
        if (t == -1) StartCoroutine("OpenUI", 0);
        move = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        collision = GameObject.FindWithTag("Player").GetComponent<Collision>();
    }

    // Update is called once per frame
    void Update()
    {
        if (text[0].activeSelf && t == -1 && move.xRaw != 0)
        {
            StartCoroutine("CloseUI", 0);
            t = 0;
        }
        if (text[1].activeSelf && t == 0)
        {
            t = 1;
            for (int i = 0; i < 8; i++)
            {
                t1_sawTween[i].DOPlay();
            }
        }
        if (t == 1 && move.isDeath == true)
        {
            for (int i = 0; i < 8; i++)
            {
                t1_sawTween[i].DORestart();
            }
        }
        if (text[2].activeSelf && t == 1)
        {
            t = 2;
            t2_platform.SetActive(true);
        }
        if (text[2].activeSelf && !onPF && collision.OnGround())
        {
            if (collision.onGroundDash[0].name == "t2_Platform")
            {
                if (!onPF)
                {
                    StartCoroutine("OnPF");
                }
                onPF = true;
            }
        }
        if (text[3].activeSelf && t == 2)
        {
            for (int i = 0; i < 17; i++)
            {
                t3_sawTween[i].DOPlay();
            }
            t = 3;
        }
        if (t == 3 && move.isDeath == true)
        {
            for (int i = 0; i < 17; i++)
            {
                t3_sawTween[i].DORestart();
            }
        }
        if (text[4].activeSelf && t == 3)
        {
            onPF = false;
            topColl.enabled = false;
            t4_PF[0].DOPlay();
            t4_PF[1].DOPlay();
            t = 4;
        }
        if (text[4].activeSelf && !onPF && collision.OnGround())
        {
            if (collision.onGroundDash[0].name == "TutorialEnd")
            {
                if (!onPF)
                {
                    text[4].SetActive(false);
                    GetComponent<GameManager>().SendMessage("NextLevel");
                }
                onPF = true;
            }
        }

    }

    IEnumerator OpenUI(int i)
    {
        yield return new WaitForSeconds(1f);
        text[i].SetActive(true);
        if (text[2].activeSelf)
        {
            yield return new WaitForSeconds(0.8f);
            t2_platform.SetActive(true);
        }
    }

    IEnumerator CloseUI(int i)
    {
        yield return new WaitForSeconds(2f);
        text[i].SetActive(false);
        if (i < 4) StartCoroutine("OpenUI", i + 1);
    }

    public void t1_Over()
    {
        if (move.isDeath == false)
        {
            StartCoroutine("CloseUI", 1);
        }
    }

    IEnumerator OnPF()
    {
        yield return new WaitForSeconds(2f);
        t2_platform.SetActive(false);
        StartCoroutine("CloseUI", 2);

    }

    public void t3_Over()
    {
        if (move.isDeath == false)
        {
            StartCoroutine("CloseUI", 3);
        }
    }


}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public UnityEvent start = new UnityEvent();
    public UnityEvent death = new UnityEvent();
    public UnityEvent resetLevel = new UnityEvent();
    public UnityEvent win = new UnityEvent();
    public UnityEvent nextLevel = new UnityEvent();

    public int nextLevelId = 0;
    public bool reTrap = false;

    [Header("Coin")]
    public int coinCountOrigin = 0; //coin總數
    public int coinCount = 0; //到終點結算coin數
    public int coinCheckCount = 0; //每到check point計算coin數
    public GameObject coinParent;
    public GameObject[] coinCheck;

    [Header("GameObject")]
    public Image passLevelTipWhite;

    [Header("Sattle")]
    public GameObject SattleIcon;
    public Text coinSattleText;
    public Text deathSattleText;
    public Text timeText;
    public Image continueTip;
    bool canContinue = false;

    int timeCount = 0;
    int timeCheck = 0;
    int deathCount = 0;
    int passlevelCount = 0;
    int deathCountByCP = 0;

    Sequence passSeq;
    // Start is called before the first frame update
    private void Awake()
    {
        start.Invoke();
    }
    private void Start()
    {
        coinCountOrigin = coinParent.transform.childCount;
        coinCheck = new GameObject[coinCountOrigin * 5];
        InvokeRepeating("TimeCount", 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            passlevelCount++;
            if (passlevelCount >= 40)
            {
                NextLevel();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Escape)) { passlevelCount = 0; }

        if (canContinue)
        {
            if (Input.GetButtonDown("Enter"))
            {
                StartCoroutine("NextLevelIEnumerator");
            }
        }
    }
    void TimeCount()
    {
        timeCount++;
    }
    public void Death()
    {
        death.Invoke();
    }
    public void ResetLevel()
    {
        deathCount++;
        deathCountByCP++;
        resetLevel.Invoke();
        if (deathCountByCP > 4)
        {
            passSeq.Kill();
            passSeq = DOTween.Sequence();
            passSeq.AppendInterval(0.4f);
            passSeq.Append(passLevelTipWhite.DOFade(1f, 0.7f));
            passSeq.AppendInterval(3.5f);
            passSeq.Append(passLevelTipWhite.DOFade(0f, 0.7f));
        }
        DeactivateChildren(coinParent, true);
        for (int i = 0; i < coinCheck.Length; i++)
        {
            if (coinCheck[i])
                coinCheck[i].SetActive(false);
        }
        reTrap = true;
        StartCoroutine("RestartTrap");
        timeCount = timeCheck;
    }
    public void CheckCoin()
    {

        deathCountByCP = 0;
        timeCheck = timeCount;
        foreach (Transform child in coinParent.transform)
        {
            if (child.gameObject.activeSelf == false)
            {
                coinCheck[coinCheckCount] = child.gameObject;
                coinCheckCount++;
                if (coinCheckCount >= coinCountOrigin * 5) coinCheckCount = 0;
            }
        }
    }
    IEnumerator RestartTrap()
    {
        yield return new WaitForSeconds(0.15f);
        reTrap = false;
    }



    void DeactivateChildren(GameObject g, bool state)
    {
        g.SetActive(state);

        foreach (Transform child in g.transform)
        {
            DeactivateChildren(child.gameObject, state);
        }
    }
    public void Win()
    {
        foreach (Transform child in coinParent.transform)
        {
            if (child.gameObject.activeSelf == false) coinCount++;
        }
        win.Invoke();
        CancelInvoke("TimeCount");
        StartCoroutine("SattleIEnumerator");
    }
    public void NextLevel()
    {
        StartCoroutine("NextLevelIEnumerator");
    }

    IEnumerator SattleIEnumerator()
    {
        yield return new WaitForSeconds(1f);
        SattleIcon.SetActive(true);
        coinSattleText.text = coinCount + " / " + coinCountOrigin;
        deathSattleText.text = "" + deathCount;
        timeText.text = Mathf.Floor(timeCheck / 60) + " : " + (timeCount - (Mathf.Floor(timeCheck / 60) * 60));


        yield return new WaitForSeconds(2f);
        continueTip.DOFade(1, 0.7f);
        canContinue = true;
    }

    IEnumerator NextLevelIEnumerator()
    {
        nextLevel.Invoke();
        if (SattleIcon) SattleIcon.SetActive(false);
        if (continueTip) continueTip.DOFade(0, 0.5f);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextLevelId);
    }

}

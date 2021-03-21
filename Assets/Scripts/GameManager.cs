using System.Collections;
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
    public bool reTrap = false;
    int coinCountOrigin = 0;
    int coinCount = 0;
    int coinCheckCount = 0;

    public GameObject coinParent;
    public GameObject[] coinCheck;
    int timeCount = 0;
    int timeCheck = 0;
    int deathCount = 0;
    int passlevelCount = 0;
    int deathCountByCP = 0;
    public Text passLevelTip;
    public Image passLevelTipWhite;
    Sequence passSeq;
    // Start is called before the first frame update
    private void Awake()
    {
        start.Invoke();
    }
    private void Start()
    {
        coinCountOrigin = coinParent.transform.childCount;
        coinCheck = new GameObject[coinCountOrigin * 3];
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
            passSeq.Append(passLevelTip.DOFade(0.85f, 0.8f));
            passSeq.Join(passLevelTipWhite.DOFade(0.2f, 0.8f));
            passSeq.AppendInterval(3f);
            passSeq.Append(passLevelTip.DOFade(0, 0.8f));
            passSeq.Join(passLevelTipWhite.DOFade(0f, 0.8f));
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
    }
    public void NextLevel()
    {
        nextLevel.Invoke();
    }

    public void LevelID(int levelNumber)
    {
        StartCoroutine("NextLevelIEnumerator", levelNumber);
    }

    IEnumerator NextLevelIEnumerator(int levelNumber)
    {
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(levelNumber);
    }

}

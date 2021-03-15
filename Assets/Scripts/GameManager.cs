using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            NextLevel();
        }
        // Debug.Log(timeCount);
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
        resetLevel.Invoke();
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
        timeCheck = timeCount;
        foreach (Transform child in coinParent.transform)
        {
            if (child.gameObject.activeSelf == false)
            {
                coinCheck[coinCheckCount] = child.gameObject;
                coinCheckCount++;
            }
            Debug.Log(coinCheckCount);
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

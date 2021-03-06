﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Networking;
using System.Text;
using System.IO;

public class GameManager : MonoBehaviour
{
    public UnityEvent start = new UnityEvent();
    public UnityEvent death = new UnityEvent();
    public UnityEvent resetLevel = new UnityEvent();
    public UnityEvent win = new UnityEvent();
    public UnityEvent nextLevel = new UnityEvent();

    public int nextLevelId = 0;
    public bool reTrap = false;
    bool startNextLevel = false;
    bool isWin = false;

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
    public bool recordInData = false;
    public bool uploadDate = false;
    public bool canSkipLevel = true;
    bool canContinue = false;
    int checkPointCount = 0;

    public int timeNotInDeathCount = 0;
    public int timeInDeathCount = 0;
    int timeCheck = 0;
    int deathCount = 0;
    int passlevelCount = 0;
    int deathCountByCP = 0;
    public int[] trapNumber = new int[] { 0, 0, 0, 0, 0 };
    SoundEffectManager _sound;

    Sequence passSeq;
    // Start is called before the first frame update
    private void Awake()
    {
        start.Invoke();
        _sound = GetComponent<SoundEffectManager>();
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
        if (Input.GetKey(KeyCode.P))
        {
            passlevelCount++;
            if (passlevelCount == 40 && !startNextLevel && canSkipLevel)
            {
                SkipLevel();
                _sound.PlayOneSound(0, 0.4f);
            }
        }
        else if (Input.GetKeyUp(KeyCode.P)) { passlevelCount = 0; }

        if (canContinue)
        {
            if (Input.GetButtonDown("Enter") && !startNextLevel)
            {
                StartCoroutine("NextLevelIEnumerator");
                _sound.PlayOneSound(0, 0.4f);
            }
        }
    }
    void TimeCount()
    {
        timeNotInDeathCount++;
        timeInDeathCount++;
    }
    public void Death()
    {
        death.Invoke();
        _sound.PlayOneSound(4, 0.7f);
    }
    public void ResetLevel()
    {
        deathCount++;
        deathCountByCP++;
        resetLevel.Invoke();
        // if (deathCountByCP > 4 && passLevelTipWhite)
        // {
        //     passSeq.Kill();
        //     passSeq = DOTween.Sequence();
        //     passSeq.AppendInterval(0.4f);
        //     passSeq.Append(passLevelTipWhite.DOFade(1f, 0.7f));
        //     passSeq.AppendInterval(3.5f);
        //     passSeq.Append(passLevelTipWhite.DOFade(0f, 0.7f));
        // }

        foreach (Transform child in coinParent.transform)
        {
            child.gameObject.SetActive(true);
            child.SendMessage("ResetCoinState");
        }
        for (int i = 0; i < coinCheck.Length; i++)
        {
            if (coinCheck[i])
                coinCheck[i].SetActive(false);
        }
        reTrap = true;
        StartCoroutine("RestartTrap");
        timeNotInDeathCount = timeCheck;
    }
    public void TrapNumberRecord(int number)
    {
        if (number == 5) number = 0;
        trapNumber[number]++;
    }
    public void CheckCoin()
    {
        checkPointCount++;
        deathCountByCP = 0;
        timeCheck = timeNotInDeathCount;
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

    public void Win()
    {
        isWin = true;
        checkPointCount++;
        foreach (Transform child in coinParent.transform)
        {
            if (child.gameObject.activeSelf == false) coinCount++;
        }
        win.Invoke();
        CancelInvoke("TimeCount");
        StartCoroutine("SattleIEnumerator");
        _sound.PlayOneSound(3, 0.56f);
        if (recordInData)
        {
            RecordInFile();
        }
    }
    public void SkipLevel()
    {
        StartCoroutine("NextLevelIEnumerator");
        if (recordInData)
        {
            RecordInFile();
        }
    }

    IEnumerator SattleIEnumerator()
    {
        yield return new WaitForSeconds(1f);
        SattleIcon.SetActive(true);
        coinSattleText.text = coinCount + " / " + coinCountOrigin;
        deathSattleText.text = "" + deathCount;

        int hour = timeInDeathCount / 60;
        int min = timeInDeathCount - ((timeInDeathCount / 60) * 60);
        timeText.text = hour + " : " + min.ToString("00");

        yield return new WaitForSeconds(2f);
        continueTip.DOFade(1, 0.7f);
        canContinue = true;
    }

    void RecordInFile()
    {
        FileStream fs = new FileStream(Application.dataPath + "/../PlayerTestData.txt", FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine(" ");
        sw.WriteLine(SceneManager.GetActiveScene().name);
        sw.WriteLine("Coin " + coinCount);
        sw.WriteLine("Death " + deathCount);
        sw.WriteLine("timeDeath " + timeInDeathCount);
        sw.WriteLine("timeNotDeath " + timeNotInDeathCount);
        // sw.WriteLine("CP " + checkPointCount);
        sw.WriteLine("Trap_0 " + trapNumber[0]);
        sw.WriteLine("Trap_1 " + trapNumber[1]);
        sw.WriteLine("Trap_2 " + trapNumber[2]);
        sw.WriteLine("Trap_3 " + trapNumber[3]);
        sw.WriteLine("Trap_4 " + trapNumber[4]);
        sw.Close();
    }

    // void RecordInData()
    // {
    //     GameData.levelName[GameData.levelCount] = SceneManager.GetActiveScene().name;
    //     GameData.coinCount[GameData.levelCount] = coinCount;
    //     GameData.deathCount[GameData.levelCount] = deathCount;
    //     GameData.timeInDeathCount[GameData.levelCount] = timeInDeathCount;
    //     GameData.timeNotInDeathCount[GameData.levelCount] = timeNotInDeathCount;
    //     GameData.checkPointCount[GameData.levelCount] = checkPointCount;

    //     // if (uploadDate)
    //     // {
    //     //     StartCoroutine("UploadIEnumerator");
    //     //     // GameData.levelCount = 0;
    //     // }
    //     GameData.levelCount++;
    // }

    IEnumerator NextLevelIEnumerator()
    {
        startNextLevel = true;
        nextLevel.Invoke();
        if (SattleIcon) SattleIcon.SetActive(false);
        if (continueTip) continueTip.DOFade(0, 0.5f);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextLevelId);
    }
    public void ChangeLevelID(int levelID)
    {
        if (levelID == 5)
        {
            if (GameData.levelOrder[0] == 6) nextLevelId = 7;
            if (GameData.levelOrder[0] == 5) nextLevelId = 6;
        }
        if (levelID == 6)
        {
            if (GameData.levelOrder[0] == 5) nextLevelId = 7;
            if (GameData.levelOrder[0] == 6) nextLevelId = 5;
        }
        if (levelID == 8)
        {
            if (GameData.levelOrder[2] == 9) nextLevelId = 10;
            if (GameData.levelOrder[2] == 8) nextLevelId = 9;
        }
        if (levelID == 9)
        {
            if (GameData.levelOrder[2] == 8) nextLevelId = 10;
            if (GameData.levelOrder[2] == 9) nextLevelId = 8;
        }
    }

    // IEnumerator UploadIEnumerator()
    // {
    //     // Create the form object.
    //     WWWForm form = new WWWForm();
    //     // Add the method data to the form object. (read or write data)
    //     // form.AddField("method", "write");

    //     form.AddField("levelName_0", GameData.levelName[0]);
    //     form.AddField("coinCount_0", GameData.coinCount[0]);
    //     form.AddField("deathCount_0", GameData.deathCount[0]);
    //     form.AddField("timeInDeathCount_0", GameData.timeInDeathCount[0]);
    //     form.AddField("timeNotInDeathCount_0", GameData.timeNotInDeathCount[0]);
    //     form.AddField("checkPointCount_0", GameData.checkPointCount[0]);


    //     form.AddField("levelName_1", GameData.levelName[1]);
    //     form.AddField("coinCount_1", GameData.coinCount[1]);
    //     form.AddField("deathCount_1", GameData.deathCount[1]);
    //     form.AddField("timeInDeathCount_1", GameData.timeInDeathCount[1]);
    //     form.AddField("timeNotInDeathCount_1", GameData.timeNotInDeathCount[1]);
    //     form.AddField("checkPointCount_1", GameData.checkPointCount[1]);


    //     Debug.Log(GameData.deathCount[1]);
    //     Debug.Log(GameData.levelName[0]);
    //     Debug.Log(GameData.levelName[1]);


    //     // Sending the request to API url with form object.
    //     using (UnityWebRequest www = UnityWebRequest.Post("https://script.google.com/macros/s/AKfycbzG2crWI37_SO3LQILD2y22tAKMRdtkouBzN4HytBpoPqLRTvv8lG1MTLJTmIcuy_Iohg/exec", form))
    //     {
    //         yield return www.SendWebRequest();

    //         if (www.isNetworkError || www.isHttpError)
    //         {
    //             Debug.Log(www.error);
    //         }
    //         else
    //         {
    //             // Done and get the response text.
    //             print(www.downloadHandler.text);
    //             Debug.Log("Form upload complete!");
    //         }
    //     }
    // }

}

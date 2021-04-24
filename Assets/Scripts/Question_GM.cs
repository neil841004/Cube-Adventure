using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.IO;

public class Question_GM : MonoBehaviour
{
    public UnityEvent AwakeEvent = new UnityEvent();
    public UnityEvent nextLevel = new UnityEvent();
    public GameObject ui;
    public Transform dropDown;
    public Transform InputField;
    public int nextLevelId = 0;
    public int QuestCount = 0;
    public int InputCount = 0;
    bool startNextLevel = false;

    private void Awake()
    {
        AwakeEvent.Invoke();
    }
    void Start()
    {
        StartCoroutine("OpenUI");
        if (dropDown) QuestCount = dropDown.childCount;
        if (InputField) InputCount = InputField.childCount;
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator OpenUI()
    {
        yield return new WaitForSeconds(0.5f);
        ui.SetActive(true);
    }

    public void NextLevel()
    {
        if (!startNextLevel)
        {
            StartCoroutine("NextLevelIEnumerator");
            RecordReply();
        }
    }

    public void RecordReply()
    {
        FileStream fs = new FileStream(Application.dataPath + "/PlayerTestData.txt", FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine(" ");
        for (int i = 0; i < QuestCount; i++)
        {
            sw.WriteLine(dropDown.GetChild(i).GetChild(0).GetComponent<Text>().text);
        }
        for (int i = 0; i < InputCount; i++)
        {
            sw.WriteLine(" ");
            sw.WriteLine("InputField" + i);
            sw.WriteLine(InputField.GetChild(i).GetComponent<InputField>().text);
        }

        sw.Close();
    }

    IEnumerator NextLevelIEnumerator()
    {
        startNextLevel = true;
        yield return new WaitForSeconds(0.1f);
        nextLevel.Invoke();
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene(nextLevelId);
    }

}

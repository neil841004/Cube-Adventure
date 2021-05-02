using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GM_Menu : MonoBehaviour
{
    public UnityEvent nextLevel = new UnityEvent();
    public UnityEvent startLevel = new UnityEvent();
    public GameObject tip;
    public int iLevel = 0;
    // Start is called before the first frame update
    void Awake()
    {
        startLevel.Invoke();
    }
    void Start() {
        QualitySettings.vSyncCount = 1;
        iLevel = Random.Range(0, 2);
        if (iLevel == 0)
        {
            GameData.levelOrder = new int[4] { 5, 6, 9, 8 };
        }
        else if (iLevel == 1) GameData.levelOrder = new int[4] { 6, 5, 8, 9 };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Enter")){
            NextLevel();
        }
    }

    public void NextLevel()
    {
        iLevel = Random.Range(0, 2);
        if (iLevel == 0)
        {
            GameData.levelOrder = new int[4] { 5, 6, 9, 8 };
        }
        else if (iLevel == 1) GameData.levelOrder = new int[4] { 6, 5, 8, 9 };
        nextLevel.Invoke();
        tip.SetActive(false);
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

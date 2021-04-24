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
    // Start is called before the first frame update
    void Awake()
    {
        startLevel.Invoke();
    }
    void Start() {
        QualitySettings.vSyncCount = 1;
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

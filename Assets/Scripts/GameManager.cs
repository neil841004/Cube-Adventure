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

    public GameObject coinParent;
    // Start is called before the first frame update
    private void Awake()
    {
        start.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            NextLevel();
        }
    }
    public void Death()
    {
        death.Invoke();
    }
    public void ResetLevel()
    {
        resetLevel.Invoke();
        DeactivateChildren(coinParent, true);
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
        win.Invoke();
    }
    public void NextLevel() {
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

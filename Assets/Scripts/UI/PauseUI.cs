using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    GameObject pauseUI;
    bool isPause = false;
    // Start is called before the first frame update
    void Start()
    {
        pauseUI = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause") && !isPause)
        {
            isPause = true;
            pauseUI.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (Input.GetButtonDown("Pause") && isPause)
        {
            isPause = false;
            pauseUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}

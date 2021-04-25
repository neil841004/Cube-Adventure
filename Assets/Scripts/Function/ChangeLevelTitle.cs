using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLevelTitle : MonoBehaviour
{
    public int levelID = 0;
    public Text text_1;
    public Text text_2;
    string newText;
    // Start is called before the first frame update
    void Start()
    {
        if (levelID == 5)
        {
            if (GameData.levelOrder[0] == 5) newText = "Level 1-A";
            if (GameData.levelOrder[0] == 6) newText = "Level 1-B";
        }
        if (levelID == 6)
        {
            if (GameData.levelOrder[0] == 5) newText = "Level 1-B";
            if (GameData.levelOrder[0] == 6) newText = "Level 1-A";
        }
        if (levelID == 8)
        {
            if (GameData.levelOrder[2] == 8) newText = "Level 2-A";
            if (GameData.levelOrder[2] == 9) newText = "Level 2-B";
        }
        if (levelID == 9)
        {
            if (GameData.levelOrder[2] == 8) newText = "Level 2-B";
            if (GameData.levelOrder[2] == 9) newText = "Level 2-A";
        }
        if (levelID > 4 && levelID < 10)
        {
            text_1.text = newText;
            text_2.text = newText;
        }
    }
}

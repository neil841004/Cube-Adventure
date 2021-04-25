using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class FileStreamStart : MonoBehaviour
{
    int i;
    // Start is called before the first frame update
    void Start()
    {
        //決定關卡順序
        i = Random.Range(0,2);
        if(i == 0){
            GameData.levelOrder = new int[4] {5,6,9,8};
        }else if(i == 1) GameData.levelOrder = new int[4] {6,5,8,9};
        for (int x = 0; x < 4; x++)
        {
            Debug.Log(GameData.levelOrder[x]);
        }

        //創建、刷新資料
        FileStream fs = new FileStream(Application.dataPath + "/PlayerTestData.txt", FileMode.Create);
        fs.Close();

        fs = new FileStream(Application.dataPath + "/PlayerTestData.txt", FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine(" ");
        sw.Close();
    }

}

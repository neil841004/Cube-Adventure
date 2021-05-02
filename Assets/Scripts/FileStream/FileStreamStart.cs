using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class FileStreamStart : MonoBehaviour
{
    void Start()
    {

        //創建、刷新資料
        FileStream fs = new FileStream(Application.dataPath + "/../PlayerTestData.txt", FileMode.Create);
        fs.Close();

        fs = new FileStream(Application.dataPath + "/../PlayerTestData.txt", FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine(" ");
        sw.Close();
    }

}

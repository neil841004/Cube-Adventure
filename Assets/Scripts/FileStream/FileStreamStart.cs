using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class FileStreamStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FileStream fs = new FileStream(Application.dataPath + "/PlayerTestData.txt", FileMode.Create);
        fs.Close();

        fs = new FileStream(Application.dataPath + "/PlayerTestData.txt", FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine(" ");
        sw.Close();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;  
using UnityEngine.UI;
 
public class ShowFPS : MonoBehaviour {  
 
    //public int fpsTarget;  //通常設30 ~ 60
    public float updateInterval = 0.5f;  //每幾秒算一次
    private float lastInterval;  
    private int frames = 0;  
    private float fps;  
    public Text FPS_text; //讓UITEXT放進來
 
 
    void Start()  
    {     
        //Application.targetFrameRate = fpsTarget;  //固定幀數
        lastInterval = Time.realtimeSinceStartup;  //自遊戲開始時間
        frames = 0;  //初始frames =0
    }  
 
    // 每一幀都會呼叫update()
    void Update()  
    {  
        frames++;   
        float timeNow = Time.realtimeSinceStartup;  
        if (timeNow >= lastInterval + updateInterval)  //每0.5秒更新一次
        {  
            fps = frames / (timeNow - lastInterval); //幀數= 每幀/每幀間隔毫秒 
            frames = 0;  
            lastInterval = timeNow;  
        }  
 
    }  
 
    void OnGUI()  
    {  
        FPS_text.text =  "FPS: " + fps.ToString() ; //在UI上顯示幀數
    }  
}  
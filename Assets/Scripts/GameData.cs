using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameData
{
    public static int levelCount = 0; //第幾關
    public static string[] levelName = new string[4]; //關卡名稱
    public static int[] coinCount = new int[4]; //蒐集品獲取數
    public static int[] deathCount = new int[4]; //死亡次數
    public static int[] timeNotInDeathCount = new int[4]; //不計失敗通關時間
    public static int[] timeInDeathCount = new int[4]; //總通關時間
    public static int[] checkPointCount = new int[4]; //關卡完成度
    public static int[] deathDesignNumber = new int[5]; // 死亡時紀錄當下陷阱對應設計模式
    // 0 = 無設計模式
    // 1 = 物件等距離安排
    // 2 = 物件規律性安排
    // 3 = 間歇式機關規律性運作
    // 4 = 挑戰組反覆
    public static string[,] question = new string[2,3]; //兩關, 三題


    // public static int[,] cpDeathCount = new int[4,10]; 

}

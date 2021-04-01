using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameData
{
    public static int levelCount = 0;
    public static string[] levelName = new string[4];
    public static int[] coinCount = new int[4];
    public static int[] deathCount = new int[4];
    public static int[] timeNotInDeathCount = new int[4];
    public static int[] timeInDeathCount = new int[4];
    public static int[] checkPointCount = new int[4];
    public static int[,] cpDeathCount = new int[4,10];
    public static string[,] question = new string[2,3];

}

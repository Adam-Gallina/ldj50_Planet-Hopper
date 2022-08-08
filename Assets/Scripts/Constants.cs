using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static int MainMenuScene = 0;
    public static int LevelScene = 1;

    public static string PlayerTag = "Player";
    public static string EnemyTag = "Enemy";

    public static int LaserLayer = 6;
    public static int EnemyLayer = 7;
    public static int MothershipLayer = 8;
    public static int ResourceLayer = 9;
    public static int ShieldLayer = 10;
    public static int PlayerLayer = 11;
    public static int EnvironmentLayer = 30;
    public static int GroundLayer = 31;

    public static Vector3 ToGround = new Vector3(0, -10, 0);
}

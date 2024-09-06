using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSettings : MonoBehaviour
{
    public static bool IsGamePaused => _isGamePaused;
    private static bool _isGamePaused;

    public static void SetPause(bool value,bool stopTime=false)
    {
        _isGamePaused = value;
        if (stopTime) Time.timeScale = 0;
        else Time.timeScale = 1;
    }
}

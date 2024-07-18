using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalSettings
{
    public static bool IsGamePaused=>_isGamePaused;
    private static bool _isGamePaused;

    public static void SetGamePause(bool isGamePaused)
    {
        _isGamePaused = isGamePaused;
        if(_isGamePaused ) Time.timeScale = 0.0f;
        else Time.timeScale = 1.0f;
    }
}

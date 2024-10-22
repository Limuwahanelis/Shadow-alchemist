using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object message, Object context)
    {
        Debug.Log(message);
    }

    public static void Warning(object message)
    {
        Debug.LogWarning(message);
    }

    public static void Error(object message) 
    {
        Debug.LogError(message);
    }

}

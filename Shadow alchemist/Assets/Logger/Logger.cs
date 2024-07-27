using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    public static void Log(object message)
    {
        Debug.Log(message);
    }

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

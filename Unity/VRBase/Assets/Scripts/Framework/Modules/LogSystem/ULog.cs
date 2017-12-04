using HDJ.Framework.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ULog  {

    public static List<LogData> logMessages = new List<LogData>();
    private const int cacheNumber = 60;

    private static readonly PoolClassManager<LogData> poolClassManager =new PoolClassManager<LogData>(15);

    public static LogData SetLogData(LogType logType, string logString, string stackTrace)
    {
        LogData logData = poolClassManager.New();
        logData.SetLogData(logType, logString, stackTrace, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        if (logMessages.Count >= cacheNumber)
        {
            poolClassManager.Recycle(logMessages[0]);
            logMessages.RemoveAt(0);
        }
        logMessages.Add(logData);
     
        return logData;
    }

    public static void Log(object message)
    {
        Debug.Log(message);
    }

    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }
    public static void LogError(object message)
    {
        Debug.LogError(message);
    }
    public static void LogAssertion(object message)
    {
        Debug.LogAssertion(message);
    }
    public static void LogException(Exception exception)
    {
        Debug.LogException(exception);
    }
}

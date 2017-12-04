using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using HDJ.Framework.Utils;
using HDJ.Framework.Tools;

namespace HDJ.Framework.Modules
{
    /// <summary>
    /// 管理日志文件输出
    /// </summary>
    public class LogRecordFileManager
    {
        private static StreamWriter infoStream;

        

        private static bool isOpenLog = true;
        private const string PlayerPrefsKey_OpenLog = "isOpenLog";
        private const string PlayerPrefsKey_WriteToFile = "isWriteLogToFile";

        private const string LogDirName = "/Logs/";

   
        private static bool isWriteLogToFile = false;
        public static bool IsOpenLog
        {
            get
            {
                return isOpenLog;
            }

            set
            {

                isOpenLog = value;
                PlayerPrefs.SetInt(PlayerPrefsKey_OpenLog, isOpenLog ? 1 : 0);
                Debug.unityLogger.logEnabled = isOpenLog;

               
            }
        }


        public static bool IsWriteLogToFile
        {
            get
            {
                return isWriteLogToFile;
            }

            set
            {
                isWriteLogToFile = value;
                PlayerPrefs.SetInt(PlayerPrefsKey_WriteToFile, isWriteLogToFile ? 1 : 0);
                if (isWriteLogToFile && infoStream == null)
                {
                    string[] paths = GetAllLogFilePaths();
                    if (paths.Length > 7)
                    {
                        FileUtils.DeleteFile(paths[0]);
                    }
                    string tempPath = dirPath + "Log-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
                    FileUtils.CreateTextFile(tempPath, "");
                    infoStream = new StreamWriter(tempPath, false, Encoding.Unicode, 1024);
                }
            }
        }

        private static string dirPath;
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {

            dirPath = Application.persistentDataPath + LogDirName;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            IsOpenLog = PlayerPrefs.GetInt(PlayerPrefsKey_OpenLog, 1) == 1 ? true : false;

            IsWriteLogToFile = PlayerPrefs.GetInt(PlayerPrefsKey_WriteToFile, 1) == 1 ? true : false;

            //在这里做一个Log的监听  
            //转载的原文中是用Application.RegisterLogCallback(HandleLog);但是这个方法在unity5.0版本已经废弃不用了  
            Application.logMessageReceived += HandleLog;

            InternalDataManager.PutData("LogData",ULog.logMessages);
        }

        static void HandleLog(string logString, string stackTrace, LogType logType)
        {
            if (!isOpenLog)
                return;

            if (logType == LogType.Warning)
                return;
            LogData logData= ULog.SetLogData(logType, logString, stackTrace);

            if (IsWriteLogToFile)
            {
                infoStream.WriteLine(logData.ToLogString());
                infoStream.Flush();
            }
        }

        public static string[] GetAllLogFilePaths()
        {
            string dirPath = Application.persistentDataPath + LogDirName;
            string[] paths = PathUtils.GetDirectoryFilePath(dirPath);

            return paths;
        }

        //public static LogData[] GetAllLogFromLogFile(string filePath)
        //{
        //   string[] content =  FileUtils.LoadTextFileLineByPath(filePath);
        //    List<LogData> list = new List<LogData>();

        //    for (int i = 0; i < content.Length; i++)
        //    {
        //        Debug.Log("Line :" + content[i]);
        //        LogData logData = LogData.LogStringToObject(content[i]);
        //        list.Add(logData);
        //    }

        //    return list.ToArray();
        //}
    }
} 
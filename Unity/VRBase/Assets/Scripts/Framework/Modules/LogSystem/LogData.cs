using System;
using UnityEngine;

namespace HDJ.Framework.Modules
{
    [System.Serializable]
    public class LogData
    {
        public LogType logType;
        public string logString;
        public string stackTrace;
        public string time;


        public LogData() { }
        public LogData(LogType logType,
         string log,
         string stackTrace,
         string time
         )
        {
            this.logType = logType;
            this.logString = log;
            this.stackTrace = stackTrace;
            this.time = time;
        }
        public void SetLogData(LogType logType, string log,string stackTrace, string time)
        {
            this.logType = logType;
            this.logString = log;
            this.stackTrace = stackTrace;
            this.time = time;
        }

        public string ToLogString()
        {
            return "[" + logType + "] [" + time + "] " + logString + "\r\n" + stackTrace + "\r\n";
        }

        //public static LogData LogStringToObject(string log)
        //{
        //    string[] temp0 = log.Split(']');
        //    string time = temp0[0].Replace("[", "");
        //    string[] temp1 = temp0[1].Split(':');
        //    LogType type = (LogType)Enum.Parse(typeof(LogType), temp1[0]);
        //    string[] temp2 = temp1[2].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        //    string logString = temp2[0];
        //    string stackTrace = temp2[1];
        //    return new LogData(type, logString, stackTrace, time);
        //}
    }
}


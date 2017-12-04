using HDJ.Framework.Modules;
using HDJ.Framework.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Modules
{
    /// <summary>
    /// 游戏运行时持久化存储
    /// </summary>
    public static class GameRuntimeStoreManager
    {

        private static Dictionary<string, object> storeDataDic = new Dictionary<string, object>();
        private const string savePathDir = "RuntimeData/GameRuntimeStoreData.txt";
        private static string filePath = "";
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            if (string.IsNullOrEmpty(filePath))
                filePath = PathUtils.GetSpecialPath(savePathDir, SpecialPathType.Persistent);
            string text = FileUtils.LoadTextFileByPath(filePath);
            if (string.IsNullOrEmpty(text))
                return;
            storeDataDic = InternalConfigManager.LoadData(text);

        }
        public static object GetValue(string key, object defultData)
        {
            if (storeDataDic.ContainsKey(key))
                return storeDataDic[key];
            else
                return defultData;
        }
        public static T GetValue<T>(string key, T defultData)
        {
            if (storeDataDic.ContainsKey(key))
            {
                object d = storeDataDic[key];
                if (d.GetType().FullName == typeof(T).FullName)
                    return (T)d;
                else
                    Debug.LogWarning("GameRuntimeStoreManager:获取值失败，已含有key:" + key + ",但值类型不同:" + d.GetType().FullName + "  : " + typeof(T).FullName);
            }
            return defultData;
        }

        public static bool SetValue(string key, object value)
        {
            if (storeDataDic.ContainsKey(key))
            {
                object d = storeDataDic[key];
                if (d.GetType().FullName == value.GetType().FullName)
                {
                    storeDataDic[key] = value;
                    return true;
                }
                else
                {
                    Debug.LogError("GameRuntimeStoreManager：存储失败，已含有相同key：" + key + " ,但值类型不同：" + d.GetType().FullName + "  : " + value.GetType().FullName);
                    return false;
                }
            }
            else
            {
                storeDataDic.Add(key, value);
                return true;
            }
        }
        public static void DeleteVlaue(string key)
        {
            if (storeDataDic.ContainsKey(key))
            {
                storeDataDic.Remove(key);
            }
        }
        public static void Clear()
        {
            storeDataDic.Clear();
        }
        public static void Save()
        {
            InternalConfigManager.SaveData(filePath, storeDataDic);
        }
    }
}

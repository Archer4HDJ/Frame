using HDJ.Framework.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Modules
{
    /// <summary>
    /// 游戏设置存储
    /// </summary>
    public static class GameSettingStoreManager
    {

        private static Dictionary<string, object> storeDataDic = new Dictionary<string, object>();
        private const string savePathDir = "Configs/InternalConfigs/GlobalGameSettingsIConfig.txt";
        private static string filePath = "";

        private static bool isInit = false;
        private static void Initialize()
        {
            if (isInit)
                return;
            isInit = true;

            if (string.IsNullOrEmpty(filePath))
                filePath = PathUtils.GetSpecialPath(savePathDir, SpecialPathType.Resources);
            AssetData[] res = ResourcesManager.LoadAssetsByName("GlobalGameSettingsIConfig");
            string text = "";
            if (res.Length > 0)
            {
                TextAsset tx = (TextAsset)res[0].asset;
                text = tx.text;
            }

            if (string.IsNullOrEmpty(text))
                return;
            storeDataDic = InternalConfigManager.LoadData(text);


        }
        public static object GetValue(string key, object defultData)
        {
            Initialize();
            if (storeDataDic.ContainsKey(key))
                return storeDataDic[key];
            else
                return defultData;
        }
        public static T GetValue<T>(string key, T defultData)
        {
            Initialize();
            if (storeDataDic.ContainsKey(key))
            {
                object d = storeDataDic[key];
                if (d.GetType().FullName == typeof(T).FullName)
                    return (T)d;
                else
                    Debug.LogWarning("GameSettingStoreManager:获取值失败，已含有key:" + key + ",但值类型不同:" + d.GetType().FullName + "  : " + typeof(T).FullName);
            }
            return defultData;
        }

        public static bool SetValue(string key, object value)
        {
            Initialize();
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
                    Debug.LogError("GameSettingStoreManager：存储失败，已含有相同key：" + key + " ,但值类型不同：" + d.GetType().FullName + "  : " + value.GetType().FullName);
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
            Initialize();
            if (storeDataDic.ContainsKey(key))
            {
                storeDataDic.Remove(key);
            }
        }
        public static void Clear()
        {
            storeDataDic.Clear();
            isInit = false;
        }
        public static void Save()
        {
            InternalConfigManager.SaveData(filePath, storeDataDic);
        }
    }
}

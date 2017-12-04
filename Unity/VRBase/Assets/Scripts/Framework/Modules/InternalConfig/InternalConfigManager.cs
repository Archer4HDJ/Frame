using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HDJ.Framework.Utils;
using  HDJ.Framework.Modules;

namespace HDJ.Framework.Modules
{

    public class InternalConfigManager
    {
        private static Dictionary<string, Dictionary<string, object>> confDatas = new Dictionary<string, Dictionary<string, object>>();
        public static Dictionary<string, object> GetConfig(string configName)
        {
            if (confDatas.ContainsKey(configName))
            {
                return confDatas[configName];
            }
            else
            {
                string text = ResourcesManager.LoadTextFileByName(configName);
                Dictionary<string, object> data = LoadData(text);
                if (data == null)
                    return null;
                confDatas.Add(configName, data);
                return data;
            }
        }
        public static Dictionary<string, object> LoadData(string textData)
        {
            List<BaseValue> data = JsonUtils.JsonToList<BaseValue>(textData);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            for (int i = 0; i < data.Count; i++)
            {
                object value = data[i].GetValue();
                //Debug.Log(value.GetType());
                dic.Add(data[i].name, value);
            }
            return dic;
        }

        public static void SaveData(string path, Dictionary<string, object> data)
        {
            List<BaseValue> temp = new List<BaseValue>();
            foreach (string obj in data.Keys)
            {
                BaseValue bv = new BaseValue(obj, data[obj]);
                temp.Add(bv);
            }
            string json = JsonUtils.ListToJson(temp);
            FileUtils.CreateTextFile(path, json);
        }

        public static void ReleaseAll()
        {
            List<string> names = new List<string>(confDatas.Keys);
            for (int i = 0; i < names.Count; i++)
            {
                ResourcesManager.ReleaseByName(names[i]);
            }
            confDatas.Clear();
        }

    }
}

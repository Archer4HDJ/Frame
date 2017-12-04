using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HDJ.Framework.Tools
{
    public static class InternalDataManager
    {
        private static Dictionary<string, object> dataDictionary = new Dictionary<string, object>();

        public static Dictionary<string, object> DataDictionary
        {
            get
            {
                return dataDictionary;
            }
        }

        public static void PutData(string name,object data)
        {
            if (!Application.isEditor)
                return;

            if (dataDictionary.ContainsKey(name))
            {
                dataDictionary[name] = data;
                return;
            }

            dataDictionary.Add(name, data);
        }

        public static bool RemoveData(string name)
        {
            if (dataDictionary.ContainsKey(name))
            {
                dataDictionary.Remove(name);
                return true;
            }

            return false;
        }

        public static void Clear()
        {
            dataDictionary.Clear();
        }
    }

}

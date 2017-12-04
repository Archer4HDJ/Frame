using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace HDJ.Framework.Modules
{
    public class ConfigTable<T> where T : TableConfigBase
    {
        public List<T> data = new List<T>();
        private List<object> keys = null;
        public List<object> Keys
        {
            get
            {
                if (keys == null)
                {
                    keys = new List<object>();
                    FieldInfo[] fields = typeof(T).GetFields();
                    if (fields.Length > 0)
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            keys.Add(fields[0].GetValue(data[i]));
                        }
                    }
                }
                return keys;
            }

        }

        /// <summary>
        /// 获取List数据中类变量相同时的一个数据
        /// </summary>
        /// <param name="fieldName">属性名字</param>
        /// <param name="fieldValue">属性值</param>
        /// <returns></returns>
        public T GetItem(object keyValue)
        {
            int temp = -1;
            for (int i = 0; i < Keys.Count; i++)
            {
                if (Keys[i].Equals(keyValue))
                    temp = i;
            }
            if (temp != -1)
            {
                return data[temp];
            }
            return default(T);
        }

    }
}
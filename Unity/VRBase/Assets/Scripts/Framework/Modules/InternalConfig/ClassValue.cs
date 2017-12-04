
using System.Collections.Generic;
using System;
using System.Reflection;
using HDJ.Framework.Utils;

namespace HDJ.Framework.Modules
{
    /// <summary>
    /// 脚本数据
    /// </summary>
    [System.Serializable]
    public class ClassValue
    {

        public string ScriptName = "";
        public List<BaseValue> fieldValues = new List<BaseValue>();
        public List<BaseValue> propertyValues = new List<BaseValue>();

        public ClassValue() { }
        public ClassValue(object value)
        {
            SetValue(value);
        }
        public void SetValue(object value)
        {
            if (value == null)
                return;
            fieldValues.Clear();

            Type type = value.GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo[] fields = type.GetFields(flags);
            ScriptName = type.FullName;
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo f = fields[i];
                object v = f.GetValue(value);
                if (v == null)
                    continue;
                BaseValue scriptValue = new BaseValue(f.Name, v);
                fieldValues.Add(scriptValue);
            }

            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo property = propertyInfos[i];
                if (property.CanRead && property.CanWrite)
                {
                    try
                    {
                        BaseValue scriptValue = new BaseValue(property.Name, property.GetValue(value, null));
                        propertyValues.Add(scriptValue);
                    }
                    catch 
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 获取储存的实例
        /// </summary>
        /// <param name="getInstanceCallBackR">自定义实例生成，用于GameObject组件赋值</param>
        /// <returns></returns>
        public object GetValue(CallBackR<object, Type> getInstanceCallBackR = null)
        {
            if (string.IsNullOrEmpty(ScriptName))
                return null;
            Type type = ReflectionUtils.GetTypeByTypeFullName(ScriptName);
            object classObj = null;
            if (getInstanceCallBackR != null)
            {
                classObj = getInstanceCallBackR(type);
            }
            else
                classObj = ReflectionUtils.CreateDefultInstance(type);

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            for (int i = 0; i < fieldValues.Count; i++)
            {
                BaseValue fInfo = fieldValues[i];
                FieldInfo f = type.GetField(fInfo.name, flags);
                if (f!=null && f.Name == fInfo.name)
                {
                    try
                    {
                        f.SetValue(classObj, fInfo.GetValue());
                    }
                    catch 
                    {
                        continue;
                    }
                }
            }



            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo property = propertyInfos[i];
                if (property.CanRead && property.CanWrite)
                {
                    try
                    {
                        for (int j = 0; j < propertyValues.Count; j++)
                        {
                            BaseValue pinfo = propertyValues[i];
                            if (property!=null&& property.Name == pinfo.name)
                            {
                                property.SetValue(classObj, pinfo.GetValue(), null);
                            }
                        }
                    }
                    catch 
                    {
                        continue;
                    }
                }
            }

            return classObj;
        }
    }
}

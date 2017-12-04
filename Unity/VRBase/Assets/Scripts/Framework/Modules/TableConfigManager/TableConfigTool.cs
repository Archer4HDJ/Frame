using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HDJ.Framework.Utils;
namespace HDJ.Framework.Modules
{
    public static class TableConfigTool
    {


        public static string ConfigInfo2TableText(TableConfigOtherInfo info, ConfigFileContents configData)
        {
            if (string.IsNullOrEmpty(info.configDescription))
                info.configDescription = "配置文件描述";
            string tableText = info.configDescription;
            tableText += "\r\n";
            List<TableConfigFieldInfo> list = new List<TableConfigFieldInfo>(info.fieldInfoDic.Values);
            for (int i = 0; i < list.Count; i++)
            {
                tableText += TypeChangeToKeyWord(list[i].defultValue.GetType());
                if (i == list.Count - 1)
                    tableText += "\r\n";
                else
                    tableText += "\t";

            }
            for (int i = 0; i < list.Count; i++)
            {
                tableText += list[i].description;
                if (i == list.Count - 1)
                    tableText += "\r\n";
                else
                    tableText += "\t";

            }
            for (int i = 0; i < list.Count; i++)
            {
                tableText += list[i].fieldName;
                if (i == list.Count - 1)
                    tableText += "\r\n";
                else
                    tableText += "\t";
            }
            for (int i = 0; i < list.Count; i++)
            {
                tableText += Value2String(list[i].defultValue);
                if (i == list.Count - 1)
                    tableText += "\r\n";
                else
                    tableText += "\t";
            }
            if (configData != null)
            {
                for (int j = 0; j < configData.configRowDataList.Count; j++)
                {
                    List<ConfigRowData> rd = configData.configRowDataList[j];
                    for (int i = 0; i < list.Count; i++)
                    {
                        object v = GetConfigRowData(rd, list[i].fieldName);
                        tableText += Value2String(v);
                        if (i == list.Count - 1)
                            tableText += "\r\n";
                        else
                            tableText += "\t";
                    }
                }
            }

            return tableText;

        }
        private static object GetConfigRowData(List<ConfigRowData> rd, string fieldName)
        {
            foreach (var item in rd)
            {
                if (item.fieldName == fieldName)
                    return item.value;
            }

            return "";
        }

        private static string TypeChangeToKeyWord(Type t)
        {
            string typeName = t.FullName;

            if (typeof(int).FullName == typeName)
                return "int";
            else if (typeof(float).FullName == typeName)
                return "float";
            else if (typeof(bool).FullName == typeName)
                return "bool";
            else if (typeof(string).FullName == typeName)
                return "string";
            else if (typeof(Vector3).FullName == typeName)
                return "Vector3";
            else if (typeof(Vector2).FullName == typeName)
                return "Vector2";
            else if (typeof(Vector3[]).FullName == typeName)
                return "Vector3[]";
            else if (typeof(Vector2[]).FullName == typeName)
                return "Vector2[]";
            else if (typeof(int[]).FullName == typeName)
                return "int[]";
            else if (typeof(float[]).FullName == typeName)
                return "float[]";
            else if (typeof(bool[]).FullName == typeName)
                return "bool[]";
            else if (typeof(string[]).FullName == typeName)
                return "string[]";

            return typeName;
        }

        public static ConfigFieldValueType Type2ConfigFieldValueType(Type t)
        {
            string typeName = t.FullName;

            if (typeof(int).FullName == typeName)
                return ConfigFieldValueType.Int;
            else if (typeof(float).FullName == typeName)
                return ConfigFieldValueType.Float;
            else if (typeof(bool).FullName == typeName)
                return ConfigFieldValueType.Bool;
            else if (typeof(string).FullName == typeName)
                return ConfigFieldValueType.String;
            else if (typeof(Vector3).FullName == typeName)
                return ConfigFieldValueType.Vector3;
            else if (typeof(Vector2).FullName == typeName)
                return ConfigFieldValueType.Vector2;
            else if (typeof(Vector3[]).FullName == typeName)
                return ConfigFieldValueType.Vector3_Array;
            else if (typeof(Vector2[]).FullName == typeName)
                return ConfigFieldValueType.Vector2_Array;
            else if (typeof(int[]).FullName == typeName)
                return ConfigFieldValueType.Int_Array;
            else if (typeof(float[]).FullName == typeName)
                return ConfigFieldValueType.Float_Array;
            else if (typeof(bool[]).FullName == typeName)
                return ConfigFieldValueType.Bool_Array;
            else if (typeof(string[]).FullName == typeName)
                return ConfigFieldValueType.String_Array;
            return ConfigFieldValueType.Int;
        }

        public static Type ConfigFieldValueType2Type(ConfigFieldValueType vType)
        {
            switch (vType)
            {
                case ConfigFieldValueType.Int:
                    return typeof(int);
                case ConfigFieldValueType.Float:
                    return typeof(float);
                case ConfigFieldValueType.Bool:
                    return typeof(bool);
                case ConfigFieldValueType.String:
                    return typeof(string);
                case ConfigFieldValueType.Vector2:
                    return typeof(Vector2);
                case ConfigFieldValueType.Vector3:
                    return typeof(Vector3);
                case ConfigFieldValueType.Int_Array:
                    return typeof(int[]);
                case ConfigFieldValueType.Float_Array:
                    return typeof(float[]);
                case ConfigFieldValueType.Bool_Array:
                    return typeof(bool[]);
                case ConfigFieldValueType.String_Array:
                    return typeof(string[]);
                case ConfigFieldValueType.Vector2_Array:
                    return typeof(Vector2[]);
                case ConfigFieldValueType.Vector3_Array:
                    return typeof(Vector3[]);
            }

            return null;
        }

        private static string Value2String(object value)
        {
            string result = "";
            Type t = value.GetType();
            string typeName = t.FullName;

            if (t.IsPrimitive || typeName == typeof(string).FullName)
            {
                result = value.ToString();
            }
            else if (typeof(Vector3).FullName == typeName)
            {
                Vector3 v3 = (Vector3)value;
                return v3.x + "," + v3.y + "," + v3.z;
            }

            else if (typeof(Vector2).FullName == typeName)
            {
                Vector3 v2 = (Vector3)value;
                return v2.x + "," + v2.y;
            }

            else if (t.IsArray)
            {
                Type itemType = t.GetElementType();
                PropertyInfo pro = t.GetProperty("Length");
                int count = (int)pro.GetValue(value, null);
                MethodInfo methodInfo = t.GetMethod("GetValue", new Type[] { typeof(int) });
                if (itemType.IsPrimitive)
                {
                    for (int i = 0; i < count; i++)
                    {
                        object da = methodInfo.Invoke(value, new object[] { i });
                        result += da.ToString();
                        if (i < count - 1)
                            result += ",";
                    }

                }
                else if (itemType.FullName == typeof(string).FullName)
                {
                    for (int i = 0; i < count; i++)
                    {
                        object da = methodInfo.Invoke(value, new object[] { i });
                        result += da.ToString();
                        if (i < count - 1)
                            result += "<Str:END>";
                    }
                }
                else if (typeof(Vector3).FullName == itemType.FullName)
                {
                    for (int i = 0; i < count; i++)
                    {
                        object da = methodInfo.Invoke(value, new object[] { i });
                        result += "[";
                        Vector3 v3 = (Vector3)da;
                        result += v3.x + "," + v3.y + "," + v3.z;
                        result += "]";
                    }
                }
                else if (typeof(Vector2).FullName == itemType.FullName)
                {
                    for (int i = 0; i < count; i++)
                    {
                        object da = methodInfo.Invoke(value, new object[] { i });
                        result += "[";
                        Vector2 v2 = (Vector2)da;
                        result += v2.x + "," + v2.y;
                        result += "]";
                    }
                }
            }
            return result;

        }

    }
}

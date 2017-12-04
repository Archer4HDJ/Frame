using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using HDJ.Framework.Modules;

namespace HDJ.Framework.Modules
{
    public class TableConfigManager
    {

        private static Dictionary<string, List<TableConfigBase>> allTableConfigDataDic = new Dictionary<string, List<TableConfigBase>>();
        private static Dictionary<string, List<List<BaseValue>>> allBaseValueConfigDataDic = new Dictionary<string, List<List<BaseValue>>>();
        public static ConfigTable<T> GetConfigTable<T>(string fileName = null) where T : TableConfigBase
        {
            ConfigTable<T> table = new ConfigTable<T>();
            table.data = GetConfig<T>(fileName);
            return table;
        }
        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">当fileName为空或null时，默认使用类名作为文件名加载</param>
        /// <returns></returns>
        public static List<T> GetConfig<T>(string fileName = null) where T : TableConfigBase
        {
            List<TableConfigBase> list = GetConfig(typeof(T), fileName);

            List<T> tList = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                tList.Add((T)list[i]);
            }
            return tList;
        }
        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">当fileName为空或null时，默认使用类名作为文件名加载</param>
        /// <returns></returns>
        public static List<TableConfigBase> GetConfig(Type type, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = type.Name;
            List<TableConfigBase> list = null;
            if (!allTableConfigDataDic.ContainsKey(fileName))
            {
                string[][] temp1 = LoadConfigData(fileName);
                //Debug.Log("Lenth" + temp1.Length);
                list = ChangeToClassData(type, temp1);

                if (allTableConfigDataDic.ContainsKey(fileName))
                {
                    allTableConfigDataDic[fileName] = list;
                }
                else
                    allTableConfigDataDic.Add(fileName, list);
            }
            else
                list = allTableConfigDataDic[fileName];
            return list;
        }
        public static void ReleaseAll()
        {
            List<string> keys = new List<string>(allTableConfigDataDic.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                Release(keys[i]);
            }
            allTableConfigDataDic.Clear();
            keys = new List<string>(allBaseValueConfigDataDic.Keys);
            for (int j = 0; j < keys.Count; j++)
            {
                Release(keys[j]);
            }
            allBaseValueConfigDataDic.Clear();
        }
        public static void Release(string fileName)
        {
            if (allTableConfigDataDic.ContainsKey(fileName))
            {
                ResourcesManager.ReleaseByName(fileName);
                allTableConfigDataDic.Remove(fileName);
            }
            if (allBaseValueConfigDataDic.ContainsKey(fileName))
                allBaseValueConfigDataDic.Remove(fileName);
        }




        private static string[][] LoadConfigData(string fileName)
        {
            AssetData[] dats = ResourcesManager.LoadAssetsByName(fileName);
            UnityEngine.Object temp = dats[0].asset;
            string[][] temp1 = null;
            if (temp != null)
            {
                string data = (temp as TextAsset).text;
                temp1 = ParseConfigData(data);

            }
            else
            {
                Debug.LogError("加载配置失败！ 文件路径：" + fileName);
            }
            return temp1;
        }

        private static string[][] ParseConfigData(string data)
        {
            //Debug.Log(data);
            //回车换行
            string[] temp0 = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            string[][] temp1 = new string[temp0.Length][];

            for (int i = 0; i < temp0.Length; i++)
            {
                //制表符
                temp1[i] = temp0[i].Split(new char[] { '\t' });
            }
            return temp1;
        }

        private static List<TableConfigBase> ChangeToClassData(Type type, string[][] configDatas)
        {
            TableConfigOtherInfo tableConfigOtherInfo = null;
            if (!TableConfigBase.tableConfigOtherInfo.ContainsKey(type.Name))
            {
                tableConfigOtherInfo = new TableConfigOtherInfo();
                TableConfigBase.tableConfigOtherInfo.Add(type.Name, tableConfigOtherInfo);
            }
            else
            {
                tableConfigOtherInfo = TableConfigBase.tableConfigOtherInfo[type.Name];
            }

            List<TableConfigBase> list = new List<TableConfigBase>();
            string configDescription = configDatas[0][0];
            tableConfigOtherInfo.configDescription = configDescription;
            for (int j = 0; j < configDatas[4].Length; j++)
            {
                string fieldName = configDatas[3][j];
                FieldInfo f = type.GetField(fieldName);
                if (f != null)
                {
                    string typeName = configDatas[1][j];
                    object defaultData = GetValue(typeName, configDatas[4][j]);
                    TableConfigFieldInfo info = new TableConfigFieldInfo();
                    info.fieldName = fieldName;
                    info.defultValue = defaultData;
                    info.fieldValueType = TableConfigTool.Type2ConfigFieldValueType(info.defultValue.GetType());
                    info.description = configDatas[2][j];
                    tableConfigOtherInfo.fieldInfoDic.Add(fieldName, info);
                }
            }
            for (int i = 5; i < configDatas.Length; i++)
            {
                TableConfigBase instance = Activator.CreateInstance(type) as TableConfigBase;

                if (instance == null)
                {
                    Debug.LogError("Config Change fail. Name : " + type);
                    break;
                }
                for (int j = 0; j < configDatas[i].Length; j++)
                {
                    string fieldName = configDatas[3][j];
                    FieldInfo f = type.GetField(fieldName);
                    if (f != null)
                    {
                        string valueStr = configDatas[i][j];
                        string typeName = configDatas[1][j];
                        object value = null;
                        object defaultData = tableConfigOtherInfo.fieldInfoDic[fieldName].defultValue;
                        if (string.IsNullOrEmpty(valueStr))
                        {
                            value = defaultData;
                        }
                        else
                        {
                            value = GetValue(typeName, valueStr);
                        }

                        f.SetValue(instance, value);
                    }
                }
                list.Add(instance);
            }
            return list;
        }


        private static object GetValue(string variableValueType, string data)
        {
            object obj = null;
            bool isDataEmpty = string.IsNullOrEmpty(data);
            //  Debug.Log("variableValueType : " + variableValueType + " data: " + data);
            try
            {
                if (variableValueType == "int")
                {
                    obj = (isDataEmpty) ? 0 : int.Parse(data);
                }
                else if (variableValueType == "float")
                {
                    obj = (isDataEmpty) ? 0f : float.Parse(data);
                }
                else if (variableValueType == "bool")
                {
                    obj = (isDataEmpty) ? false : bool.Parse(data);
                }
                else if (variableValueType == "string")
                {
                    obj = (isDataEmpty) ? "" : data;
                }
                else if (variableValueType == "Vector3")
                {
                    if (!isDataEmpty)
                    {
                        obj = ParseV2V3(data); ;
                    }
                    else
                    {
                        obj = Vector3.zero;
                    }
                }
                else if (variableValueType == "Vector2")
                {
                    if (!isDataEmpty)
                    {
                        obj = ParseV2V3(data); ;
                    }
                    else
                    {
                        obj = Vector2.zero;
                    }
                }
                else if (variableValueType == "Vector3[]")
                {

                    if (!isDataEmpty)
                    {
                        string[] temp0 = StringSplitInStartAndEndChar(data, '[', ']');
                        List<Vector3> v3List = new List<Vector3>();
                        for (int x = 0; x < temp0.Length; x++)
                        {
                            Vector3 v3 = (Vector3)ParseV2V3(temp0[x]);
                            v3List.Add(v3);
                        }
                        obj = v3List.ToArray();
                    }
                    else
                    {
                        obj = new Vector3[0];
                    }
                }
                else if (variableValueType == "Vector2[]")
                {
                    if (!isDataEmpty)
                    {
                        string[] temp0 = StringSplitInStartAndEndChar(data, '[', ']');
                        List<Vector2> v2List = new List<Vector2>();
                        for (int x = 0; x < temp0.Length; x++)
                        {
                            Vector2 v2 = (Vector2)ParseV2V3(temp0[x]);
                            v2List.Add(v2);
                        }
                        obj = v2List.ToArray();
                    }
                    else
                    {
                        obj = new Vector2[0];
                    }
                }
                else if (variableValueType == "int[]")
                {
                    if (!isDataEmpty)
                    {
                        string[] temp = data.Split(new char[] { ',' });
                        int[] temp1 = new int[temp.Length];
                        for (int i = 0; i < temp.Length; i++)
                        {
                            temp1[i] = int.Parse(temp[i]);
                        }
                        obj = temp1;
                    }
                    else
                    {
                        obj = new int[0];
                    }
                }
                else if (variableValueType == "float[]")
                {
                    if (!isDataEmpty)
                    {
                        string[] temp = data.Split(new char[] { ',' });
                        float[] temp1 = new float[temp.Length];
                        for (int i = 0; i < temp.Length; i++)
                        {
                            temp1[i] = float.Parse(temp[i]);
                        }
                        obj = temp1;
                    }
                    else
                    {
                        obj = new float[0];
                    }
                }
                else if (variableValueType == "bool[]")
                {
                    if (!isDataEmpty)
                    {
                        string[] temp = data.Split(new char[] { ',' });
                        bool[] temp1 = new bool[temp.Length];
                        for (int i = 0; i < temp.Length; i++)
                        {
                            temp1[i] = bool.Parse(temp[i]);
                        }
                        obj = temp1;
                    }
                    else
                    {
                        obj = new bool[0];
                    }
                }
                else if (variableValueType == "string[]")
                {
                    if (!isDataEmpty)
                    {
                        string[] temp = data.Split(new string[] { "<Str:END>" }, StringSplitOptions.None);// StringSplitInStartAndEndChar(data, '[', ']');
                        obj = temp;
                    }
                    else
                    {
                        obj = new string[0];
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Data :" + data + "\n" + e);
            }

            return obj;
        }
        private static string[] StringSplitInStartAndEndChar(string data, char startChar, char endChar)
        {
            char[] charArray;
            List<string> tList = new List<string>();
            string tempData = data;
            bool isEnd = false;

            while (!isEnd)
            {
                int startIndex = 0;
                int endIndex = 0;
                isEnd = true;
                bool startCheckEnd = false;
                charArray = tempData.ToCharArray();
                for (int i = 0; i < charArray.Length; i++)
                {
                    if (startCheckEnd && charArray[i] == endChar)
                    {
                        endIndex = i;
                        isEnd = false;
                        break;
                    }
                    if (!startCheckEnd && charArray[i] == startChar)
                    {
                        startIndex = i;
                        startCheckEnd = true;
                    }
                }
                if (!isEnd)
                {
                    string tempStr1 = tempData.Substring(startIndex, endIndex - startIndex + 1);
                    tempStr1 = tempStr1.Replace("[", "");
                    tempStr1 = tempStr1.Replace("]", "");
                    tList.Add(tempStr1);
                    tempData = tempData.Remove(startIndex, endIndex - startIndex + 1);
                    // Debug.Log("tempStr1 : " + tempStr1 + " __tempData : " + tempData);
                }

            }

            return tList.ToArray();

        }
        /// <summary>
        /// 解析vector2,3
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static object ParseV2V3(string data)
        {
            string[] temp = data.Split(new char[] { ',' });
            float[] temp1 = new float[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                temp1[i] = float.Parse(temp[i]);
            }
            if (temp.Length > 2)
            {
                Vector3 v3 = new Vector3(temp1[0], temp1[1], temp1[2]);
                return v3;
            }
            else
            {
                Vector2 v2 = new Vector2(temp1[0], temp1[1]);
                return v2;
            }
        }

    }
}

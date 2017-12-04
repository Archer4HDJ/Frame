using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  HDJ.Framework.Modules
{
    /// <summary>
    /// 资源路径管理器
    /// </summary>
    public class ResourcePathManager
    {
        private static Dictionary<string, string> pathDataDic = new Dictionary<string, string>();
        private static string fileName = "PathFile";
        private static bool isInit = false;

        public static void Init()
        {
            if (isInit) return;

            isInit = true;
            pathDataDic.Clear();
            if (ResourcesManager.assetsLoadType == AssetsLoadType.Resources)
            {
                AssetData[] res = ResourcesManager.LoadAssets(fileName);
                if (res.Length > 0 && res[0].asset)
                {
                    TextAsset tex = (TextAsset)res[0].asset;
                    string text = tex.text;
                    pathDataDic = LoadPathData(text);
                }
            }
            else
            {
                string filePathPer = Application.persistentDataPath + "/" + fileName + ".txt";
                string filePathStream = Application.streamingAssetsPath + "/" + fileName + ".assetbundle";

                string textDataPer = FileUtils.LoadTextFileByPath(filePathPer);
                Dictionary<string, string> pathDataPer = LoadPathData(textDataPer);

                AssetData[] resArr = AssetBundleLoadManager.LoadAssets(filePathStream, false);
                TextAsset tex = (TextAsset)resArr[0].asset;
                string text = tex.text;
                //Debug.Log(text);
                pathDataDic = LoadPathData(text);
                List<string> list = new List<string>(pathDataDic.Keys);
                for (int i = 0; i < list.Count; i++)
                {
                    pathDataDic[list[i]] = Application.streamingAssetsPath + "/" + pathDataDic[list[i]];
                }
                foreach (string key in pathDataPer.Keys)
                {
                    if (pathDataDic.ContainsKey(key))
                    {
                        pathDataDic[key] = Application.persistentDataPath + "/" + pathDataPer[key];
                    }
                }

            }
        }
        public static Dictionary<string, string> LoadPathData(string textData)
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(textData))
            {
                string[] dataArr = textData.Split('&');
                for (int i = 0; i < dataArr.Length; i++)
                {
                    if (string.IsNullOrEmpty(dataArr[i]))
                        continue;
                    string[] ss = dataArr[i].Split(',');
                    string name = ss[0].Trim();
                    string path = ss[1].Trim();
                    if (ss.Length > 0 && !string.IsNullOrEmpty(name))
                    {
                        if (temp.ContainsKey(name))
                            Debug.LogError("已含有：name：" + name);
                        else
                            temp.Add(name, path);
                    }
                }
            }
            return temp;
        }
        public static List<string> GetAllPathNames()
        {
            Init();
            return new List<string>(pathDataDic.Keys);
        }
        public static string GetPath(string resName)
        {
            Init();
            if (ResourcesManager.assetsLoadType == AssetsLoadType.AssetBundle)
                resName = resName.ToLower();

            if (!pathDataDic.ContainsKey(resName))
            {
                Debug.LogError("没有文件名：" + resName);
                return "";
            }
            return pathDataDic[resName];
        }
        public static void ReLoadData()
        {
            Clear();
            Init();
        }
        public static void Clear()
        {
            ResourcesManager.ReleaseAll();
            pathDataDic.Clear();
            isInit = false;
        }
        public static bool ContainsFileName(string name)
        {
            Init();
            name = name.ToLower();
            List<string> allName = new List<string>(pathDataDic.Keys);
            for (int a = 0; a < allName.Count; a++)
            {
                allName[a] = allName[a].ToLower();
            }
            return allName.Contains(name);
        }

#if UNITY_EDITOR
        public static void DeleteResouceFile(string name)
        {
            Init();
            if (ContainsFileName(name))
            {
                string path = PathUtils.GetSpecialPath(GetPath(name), SpecialPathType.Resources);
                Debug.Log(path);
                FileUtils.DeleteFile(path);               
                Type t = ReflectionUtils.GetTypeByTypeFullName("AssetBundleBuildUtils");
                object[] paras = new object[] { "Assets/Resources", null };
                ReflectionUtils.InvokMethod(t, null, "CreateAllResPathInfo", ref paras);
                //AssetBundleBuildUtils.CreateAllResPathInfo("Assets/Resources");
                ReLoadData();
            }
        }
#endif
    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using HDJ.Framework.Utils;
using HDJ.Framework.Modules;
using System;

/// <summary>
/// 把Resource下的资源打包成.assetbundle 到StreamingAssets目录下
/// </summary>
public class AssetBundleBuildUtils
{
    public static void BuildAssetBundle(string AssetBundlesOutputPath)
    {
        if (!Directory.Exists(AssetBundlesOutputPath))
        {
            Directory.CreateDirectory(AssetBundlesOutputPath);
        }

        //根据BuildSetting里面所激活的平台进行打包
        BuildPipeline.BuildAssetBundles(AssetBundlesOutputPath, 0, EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.Refresh();

        Debug.Log("打包完成");

    }

    /// <summary>
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包
    /// 之前说过，只要设置了AssetBundleName的，都会进行打包，不论在什么目录下
    /// </summary>
    public static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log(length);
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }

        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
        length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log(length);
    }

    public static void SetAllResourceBundleName(string path, string[] endsWith)
    {
        string[] pathArr = PathUtils.GetDirectoryFilePath(path, endsWith);

        for (int i = 0; i < pathArr.Length; i++)
        {
            string s =Path.GetFileNameWithoutExtension(pathArr[i]);
            if (s == "PathFile")
                continue;
            AssetImporter assetImporter = AssetImporter.GetAtPath(pathArr[i]);

            if (assetImporter)
            {
                string bundleName = PathUtils.CutPath(pathArr[i], "Resources");
                bundleName = bundleName.Replace(Path.GetExtension(bundleName), ".assetbundle");
                assetImporter.assetBundleName = bundleName;
            }
        }
    }

    public static void SetPakagOneBundleName(string path, string[] endsWith)
    {
        string[] pathArr = PathUtils.GetDirectoryFilePath(path, endsWith);
        string bundleName = PathUtils.CutPath(path, "Resources") + "/" + Path.GetFileName(path) + ".assetbundle";
        Debug.Log(bundleName);
        for (int i = 0; i < pathArr.Length; i++)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(pathArr[i]);
            if (assetImporter)
                assetImporter.assetBundleName = bundleName;
        }
    }
    /// <summary>
    /// 创建bundle信息
    /// </summary>
    /// <param name="settingData"></param>
    public static void CreateBundleVersionFile(string assetPath, AssetBundleInfo assetBundleInfo)
    {
        if (assetPath[assetPath.Length - 1] != '/')
            assetPath += "/";
        string data = JsonUtils.ClassOrStructToJson(assetBundleInfo);
        FileUtils.CreateTextFile(assetPath + UpdateAssetsConst.AssetBundleInfoFileName, data);
    }
    /// <summary>
    /// 创建资源路径文件
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="endsWith"></param>
    public static void CreateAllResPathInfo(string assetPath, string[] endsWith=null)
    {
        if (assetPath[assetPath.Length - 1] != '/')
            assetPath += "/";
        string[] pathArray = PathUtils.GetDirectoryFilePath(assetPath, endsWith);
        pathArray = PathUtils.RemovePathWithEnds(pathArray, new string[] { ".meta" });
        string data = "";
        Dictionary<string, string> dic = new Dictionary<string, string>();
        foreach (string s in pathArray)
        {
            string temp = s.Replace(assetPath, "");
            string name = Path.GetFileNameWithoutExtension(s);
           // name = name.ToLower();
            data += name + "," + temp + "&";

            if (dic.ContainsKey(name))
            {
                Debug.LogError("资源文件名重复! " + name + " [ " + temp + "]   [" + dic[name]+"]");
            }
            else
            {
                dic.Add(name, temp);
            }
        }
        FileUtils.CreateTextFile(assetPath + "PathFile.txt", data);
        AssetDatabase.Refresh();
        Debug.Log("创建资源路径文件");
    }

    public static Dictionary<string,string> GetPathFileData(string path)
    {
        string data = FileUtils.LoadTextFileByPath(path);
        string[] temp = data.Split('&');
        Dictionary<string, string> dir = new Dictionary<string, string>();
        foreach (var item in temp)
        {
            string[] str = item.Split(',');
            dir.Add(str[0], str[1]);
        }

        return dir;
    }

    /// <summary>
    /// 创建资源MD5和路径文件
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="endsWith"></param>
    public static void CreateAllResMD5AndPath(string assetPath, string[] endsWith=null)
    {
        if (assetPath[assetPath.Length - 1] != '/')
            assetPath += "/";
        string[] pathArray = PathUtils.GetDirectoryFilePath(assetPath, endsWith);
        pathArray = PathUtils.RemovePathWithEnds(pathArray, new string[] { ".meta" });
        string data = "";
        foreach (string s in pathArray)
        {
            //string temp = s.Replace(assetPath, "");
            string md5 = FileUtils.GetFileMD5(s);
            string name = Path.GetFileNameWithoutExtension(s);
            name = name.ToLower();
            data += name + "," + md5 + "&";
        }
        FileUtils.CreateTextFile(assetPath + UpdateAssetsConst. PathMD5FileName, data);
        AssetDatabase.Refresh();
    }
  

    public static void CreateOneFileBundle(string AssetBundlesOutputPath,string oldPath,string fileName,string assetBundleName)
    {
        string path1 = oldPath;
        string copyPath = "Assets/Temp/"+fileName;
        string s = Path.GetDirectoryName(copyPath);
        if (!Directory.Exists(s))
        {
            Directory.CreateDirectory(s);
        }
        File.Copy(path1, copyPath, true);
        AssetDatabase.Refresh();
        AssetImporter assetImporter = AssetImporter.GetAtPath(copyPath);

        if (assetImporter)
        {
            assetImporter.assetBundleName = assetBundleName;
        }
        AssetBundleBuildUtils.BuildAssetBundle(AssetBundlesOutputPath);
        if (Directory.Exists(s))
        {
            Directory.Delete(s, true);
        }
    }

}


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace  HDJ.Framework.Modules
{
    public class AssetBundleLoadManager
    {
        //AssetBundle解压缓存，key为路径
        private static Dictionary<string, AssetData[]> assetCacheDic = new Dictionary<string, AssetData[]>();
        //AssetBundle缓存
        private static Dictionary<string, AssetBundle> assetBundleCacheDic = new Dictionary<string, AssetBundle>();

        //总依赖文件
        private static AssetBundleManifest assetBundleManifest = null;
        private static Dictionary<string, string> assetBundleManifestNameDic = new Dictionary<string, string>();
        public static void LoadAssetsAsync(MonoBehaviour mono, string path, CallBack<AssetData[]> callBack = null)
        {
            mono.StartCoroutine(LoadAssetsIEnumerator(path, callBack));
        }

        public static IEnumerator LoadAssetsIEnumerator(string path, CallBack<AssetData[]> callBack)
        {
            if (assetBundleManifest == null)
                LoadAssetBundleManifest();
            AssetData[] rds = null;
            if (!assetCacheDic.ContainsKey(path))
            {
                string[] depArr = GetAllDependenciesName(Path.GetFileNameWithoutExtension(path));
                for (int i = 0; i < depArr.Length; i++)
                {
                    string p = ResourcePathManager.GetPath(depArr[i]);
                    if (!assetCacheDic.ContainsKey(p))
                        yield return LoadAssetsIEnumerator(p, null);
                }
                string temp = OtherUtils.GetWWWLoadPath(path);

                //加载bundle文件
                WWW www = new WWW(temp);
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    try
                    {
                        AssetBundle ab = www.assetBundle;
                        rds = DealWithAssetBundle(ab);
                        //Debug.Log("加载成功：" + path);
                        assetBundleCacheDic.Add(path, ab);
                        assetCacheDic.Add(path, rds);
                        www.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Path:" + path + "\r\n" + e);
                    }
                }
                else
                {
                    Debug.LogError("加载失败,Path:" + path + "  error:" + www.error);
                }


            }
            else
            {
                rds = assetCacheDic[path];
            }
            yield return new WaitForEndOfFrame();
            if (rds == null)
                rds = new AssetData[0];
            if (callBack != null)
                callBack(rds);


        }
        public static AssetData[] LoadAssets(string path)
        {
            return LoadAssets(path, true);
        }
        public static AssetData[] LoadAssets(string path, bool isLoadDependencieRes = true)
        {
            if (isLoadDependencieRes && assetBundleManifest == null)
                LoadAssetBundleManifest();

            if (assetCacheDic.ContainsKey(path))
                return assetCacheDic[path];

            if (Application.platform == RuntimePlatform.Android)
            {
                //Application.streamingAssetsPath ＝ jar:file:///data/app/com.xxx.xxx-1.apk!/assets

                //Application.dataPath+”!assets” ＝ /data/app/com.xxx.xxx-1.apk!assets
                path = path.Replace(@"jar:file://", "");
                path = path.Replace("apk!/assets", "apk!assets");
            }
            if (isLoadDependencieRes)
            {
                string[] depArr = GetAllDependenciesName(Path.GetFileNameWithoutExtension(path));
                for (int i = 0; i < depArr.Length; i++)
                {
                    string p = ResourcePathManager.GetPath(depArr[i]);
                    Debug.Log(p);
                    LoadAssets(p, isLoadDependencieRes);
                }
            }
            AssetBundle ab = AssetBundle.LoadFromFile(path);

            if (ab == null)
            {
                Debug.LogError("Load Sources failed! path: " + path);
                return null;
            }
            AssetData[] rds = DealWithAssetBundle(ab);
            assetBundleCacheDic.Add(path, ab);
            assetCacheDic.Add(path, rds);
            // ab.Unload(false);
            return rds;
        }
        public static void Release(string path)
        {
            if (assetCacheDic.ContainsKey(path))
            {
                AssetData[] resA = assetCacheDic[path];
                for (int i = 0; i < resA.Length; i++)
                {
                    if (resA[i].asset)
                        UnityEngine.Resources.UnloadAsset(resA[i].asset);
                }
                assetCacheDic.Remove(path);
                assetBundleCacheDic[path].Unload(false);
                assetBundleCacheDic.Remove(path);
            }
            else
            {
                Debug.LogError("没找到的资源：" + path);
            }
        }
        public static void ReleaseAll()
        {
            List<string> list = new List<string>(assetCacheDic.Keys);
            for (int i = 0; i < list.Count; i++)
            {
                Release(list[i]);
            }
            assetCacheDic.Clear();
            assetBundleCacheDic.Clear();
            assetBundleManifestNameDic.Clear();
            assetBundleManifest = null;
        }

        private static void LoadAssetBundleManifest()
        {
            string path = ResourcePathManager.GetPath("StreamingAssets");
            Debug.Log(path);
            AssetData[] resA = LoadAssets(path, false);
            assetBundleManifest = resA[0].asset as AssetBundleManifest;
            string[] sArr = assetBundleManifest.GetAllAssetBundles();
            for (int i = 0; i < sArr.Length; i++)
            {
                assetBundleManifestNameDic.Add(Path.GetFileNameWithoutExtension(sArr[i]), sArr[i]);
            }

        }
        private static string[] GetAllDependenciesName(string name)
        {
            try
            {
                string assetBundleName = assetBundleManifestNameDic[name];
                string[] ss = assetBundleManifest.GetAllDependencies(assetBundleName);
                List<string> list = new List<string>();
                for (int i = 0; i < ss.Length; i++)
                {
                    list.Add(Path.GetFileNameWithoutExtension(ss[i]));
                }
                return list.ToArray();
            }
            catch //(Exception e)
            {
                return new string[0];
            }


        }
        private static AssetData[] DealWithAssetBundle(AssetBundle ab)
        {

            List<AssetData> resList = new List<AssetData>();
            string[] names = ab.GetAllAssetNames();
            for (int i = 0; i < names.Length; i++)
            {
                AssetData res = new AssetData(names[i]);
                res.asset = ab.LoadAsset(names[i]);
                resList.Add(res);
            }
            return resList.ToArray();
        }



    }
}
/// <summary>
/// AssetBundle的依赖文件信息
/// </summary>
//public class AssetBundleManifestInfo
//{
//    private string[] assetsPathInAssetBundle;
//    private string[] dependenciesPath;

//    public AssetBundleManifestInfo(string text)
//    {
//      string[] tempStrArr =  text.Split(new string[] { "Assets:" },StringSplitOptions.RemoveEmptyEntries);
//        if (tempStrArr.Length > 1)
//        {
//            string[] strArr0 = tempStrArr[1].Trim().Split(new string[] { "Dependencies:" }, StringSplitOptions.RemoveEmptyEntries);

//            string[] strArr1 = strArr0[0].Trim().Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
//            for (int i = 0; i < strArr1.Length; i++)
//            {
//                strArr1[i] = strArr1[i].Replace("- ", "");
//            }
//            assetsPathInAssetBundle = strArr1;

//            if(strArr0[1].Trim() == "[]")
//            {
//                dependenciesPath = new string[0];
//            }
//            else
//            {
//                string[] strArr2 = strArr0[1].Trim().Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
//                for (int i = 0; i < strArr2.Length; i++)
//                {
//                    strArr2[i] = strArr2[i].Replace("- ", "");
//                }
//                dependenciesPath = strArr2;
//            }
          
//        }
//    }
//    /// <summary>
//    /// 获取AssetBundle所有资源路径，路径格式如："Assets/Resources/Materials/New Material1.mat"
//    /// </summary>
//    /// <returns></returns>
//    public string[] GetAllAssetsPathInAssetBundle()
//    {
//        return assetsPathInAssetBundle;
//    }
//    /// <summary>
//    /// 获取AssetBundle所有依赖AssetBundle路径,如:"F:/Project/MyProjects/VRBase/VRBase/Assets/StreamingAssets/materials/materials.assetbundle"
//    /// </summary>
//    /// <returns></returns>
//    public string[] GetAllDependenciesPath()
//    {
//        return dependenciesPath;
//    }
//}

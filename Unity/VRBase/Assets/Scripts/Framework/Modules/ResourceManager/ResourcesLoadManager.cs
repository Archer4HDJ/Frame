using HDJ.Framework.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace  HDJ.Framework.Modules
{
    public class ResourcesLoadManager
    {

        private static Dictionary<string, AssetData[]> assetCacheDic = new Dictionary<string, AssetData[]>();
        public static void LoadAssetsAsync(MonoBehaviour mono, string path, CallBack<AssetData[]> callBack = null)
        {
            mono.StartCoroutine(LoadAssetsIEnumerator(path, callBack));
        }
        public static IEnumerator LoadAssetsIEnumerator(string path, CallBack<AssetData[]> callBack)
        {
            AssetData[] rds = null;
            if (!assetCacheDic.ContainsKey(path))
            {
                string s = PathUtils.RemoveExtension(path);
                ResourceRequest ass = UnityEngine.Resources.LoadAsync(s);
                yield return ass;

                if (ass.asset != null)
                {
                    rds = new AssetData[1];
                    rds[0] = new AssetData(path);
                    rds[0].asset = ass.asset;
                    assetCacheDic.Add(path, rds);
                }
                else
                {
                    Debug.LogError("加载失败,Path:" + path);
                }
            }
            else
            {
                rds = assetCacheDic[path];
            }
            if (rds == null)
                rds = new AssetData[0];
            if (callBack != null)
                callBack(rds);
            yield return new WaitForEndOfFrame();

        }
        public static AssetData[] LoadAssets(string path)
        {
            if (assetCacheDic.ContainsKey(path))
                return assetCacheDic[path];

            string s = PathUtils.RemoveExtension(path);
            AssetData[] rds = null;
            Object ass = UnityEngine.Resources.Load(s);
            if (ass != null)
            {
                rds = new AssetData[1];
                rds[0] = new AssetData(path);
                rds[0].asset = ass;
                assetCacheDic.Add(path, rds);
            }
            else
            {
                Debug.LogError("加载失败,Path:" + path);
            }
            if (rds == null)
                rds = new AssetData[0];
            return rds;
        }
        public static void Release(string path)
        {
            if (assetCacheDic.ContainsKey(path))
            {
                if (Application.isPlaying)
                {
                    AssetData[] resA = assetCacheDic[path];
                    for (int i = 0; i < resA.Length; i++)
                    {
                        if (resA[i].asset)
                            UnityEngine.Resources.UnloadAsset(resA[i].asset);
                    }
                }
                assetCacheDic.Remove(path);
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

        }

    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

namespace  HDJ.Framework.Modules
{
    public enum AssetsLoadType
    {
        /// <summary>
        /// Resources文件夹下的资源
        /// </summary>
        Resources = 0,
        /// <summary>
        ///AssetBundle下的资源
        /// </summary>
        AssetBundle = 1,

    }

    public struct QueueData
    {
        public string key;
        public CallBack<AssetData[]> callBack;
        public QueueData(string key, CallBack<AssetData[]> callBack)
        {
            this.key = key;
            this.callBack = callBack;
        }
    }

    public delegate AssetData[] LoadAssetsFunctionDelegate(string path);
    public delegate IEnumerator LoadAssetsAsyncFunctionDelegate(string path, CallBack<AssetData[]> callBack);
    public class ResourcesManager
    {
        public static AssetsLoadType assetsLoadType = AssetsLoadType.Resources;

        private static Queue<QueueData> loadQueue = new Queue<QueueData>();
        private static bool isLoadAsync = false;
        /// <summary>
        /// 是否正在异步加载资源中
        /// </summary>
        public static bool IsLoadAsync
        {
            get { return isLoadAsync; }
        }
        /// <summary>
        /// 资源加载队列中剩余要加载的个数
        /// </summary>
        public static int ResidueLoadAsyncCount
        {
            get { return loadQueue.Count - 1; }
        }
        private static LoadAssetsAsyncFunctionDelegate LoadAssetsAsyncFunction;
        private static LoadAssetsFunctionDelegate LoadAssetsFunction;
        private static CallBack<string> ReleaseFunction;
        private static CallBack ReleaseAllFunction;

        private static bool isInit = false;
        private static void Initialize()
        {
            if (isInit)
                return;
            isInit = true;
            if (assetsLoadType == AssetsLoadType.Resources)
            {
                LoadAssetsAsyncFunction = ResourcesLoadManager.LoadAssetsIEnumerator;
                LoadAssetsFunction = ResourcesLoadManager.LoadAssets;
                ReleaseFunction = ResourcesLoadManager.Release;
                ReleaseAllFunction = ResourcesLoadManager.ReleaseAll;
            }
            else
            {
                LoadAssetsAsyncFunction = AssetBundleLoadManager.LoadAssetsIEnumerator;
                LoadAssetsFunction = AssetBundleLoadManager.LoadAssets;
                ReleaseFunction = AssetBundleLoadManager.Release;
                ReleaseAllFunction = AssetBundleLoadManager.ReleaseAll;
            }

        }

        /// <summary>
        /// 同步加载一个资源
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="callBack"></param>
        public static AssetData[] LoadAssetsByName(string resName)
        {
            string path0 = ResourcePathManager.GetPath(resName);
            return LoadAssets(path0);
        }
        /// <summary>
        /// 同步加载一个资源
        /// </summary>
        /// <param name="resData"></param>
        /// <param name="callBack"></param>
        public static AssetData[] LoadAssets(string path)
        {
            Initialize();
            return LoadAssetsFunction(path);
        }

        public static void LoadAssetsAsyncByName(string resName, CallBack<AssetData[]> callBack)
        {
            string path0 = ResourcePathManager.GetPath(resName);
            LoadAssetsAsync(path0, callBack);
        }
        /// <summary>
        /// 加载一个资源
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="name">具体资源名（用于AssetBundle加载资源）</param>
        /// <param name="callBack">加载资源完成后自动回调</param>
        public static void LoadAssetsAsync(string path, CallBack<AssetData[]> callBack)
        {
            Initialize();
            QueueData data = new QueueData(path, callBack);
            loadQueue.Enqueue(data);
            if (!IsLoadAsync)
            {
              
                MonoBehaviourRuntime.Instance.StartCoroutine(StartLoadQueue());
            }
        }

        /// <summary>
        /// 一次加载大量资源（用于关卡预加载）
        /// </summary>
        /// <param name="names">资源名字</param>
        /// <param name="callBackLoadEachOne">没加载一个回调一次，（参数是还剩余多少个要加载的）</param>
        /// <param name="completeCallback">加载完成回调</param>
        public static void LoadManyAssetAsyncByName(List<string> names, CallBack<int> callBackLoadEachOne = null, CallBack completeCallback = null)
        {
            Initialize();
            for (int i = 0; i < names.Count; i++)
            {
                string path0 = ResourcePathManager.GetPath(names[i]);
                QueueData data = new QueueData(path0, (res) =>
                {
                    if (callBackLoadEachOne != null)
                        callBackLoadEachOne(ResidueLoadAsyncCount);
                    if (ResidueLoadAsyncCount == 0 && completeCallback != null)
                        completeCallback();
                });
                loadQueue.Enqueue(data);
            }
            if (!IsLoadAsync)
            {
                MonoBehaviourRuntime.Instance.StartCoroutine(StartLoadQueue());
            }
        }

        private static IEnumerator StartLoadQueue()
        {
            isLoadAsync = true;
            while (loadQueue.Count > 0)
            {
                QueueData data = loadQueue.Dequeue();
                yield return LoadAssetsAsyncFunction(data.key, data.callBack);
                if (loadQueue.Count == 0)
                    isLoadAsync = false;
            }
        }

        public static string LoadTextFileByName(string name)
        {
            TextAsset tex = LoadUnityAssetByName<TextAsset>(name);
            if (tex == null)
                return null;
            return tex.text;
        }

        public static T LoadUnityAssetByName<T>(string name) where T:UnityEngine.Object
        {
            AssetData[] res = ResourcesManager.LoadAssetsByName(name);
            if (res.Length == 0 || res[0].asset == null)
                return null;
            return (T)res[0].asset;
        }
        public static void ReleaseAll()
        {
            Initialize();
            ReleaseAllFunction();
        }
        public static void ReleaseByName(string resName)
        {
            string path0 = ResourcePathManager.GetPath(resName);
            if (!string.IsNullOrEmpty(path0))
                Release(path0);
        }
        public static void Release(string path)
        {
            ReleaseFunction(path);
        }

    }
}

using HDJ.Framework.Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 资源文件变化事件
/// </summary>
public class PeojectAssetChangeEvent  {

    [InitializeOnLoadMethod]
    static void EventFileChange()
    {
        AssetsSortManagement.Init();
        PeojectAssetWillModificationEvent.OnCreateAssetCallBack += OnCreateAssetCallBack;
        PeojectAssetWillModificationEvent.OnDeleteAssetCallBack += OnDeleteAssetCallBack;
        PeojectAssetWillModificationEvent.OnMoveAssetCallBack += OnMoveAssetCallBack;
        PeojectAssetWillModificationEvent.OnSaveAssetsCallBack += OnSaveAssetsCallBack;
    }

    private static void OnSaveAssetsCallBack(string[] t)
    {
        List<string> paths = new List<string>();
        paths.AddRange(t);
        UpdateAsset(paths);

        GlobalEvent.DispatchEvent(EditorGlobalEventEnum.OnSaveAssets, t);
    }

    private static void OnMoveAssetCallBack(AssetMoveResult t, string t1, string t2)
    {
        List<string> paths = new List<string>();
        paths.Add(t1);
        paths.Add(t2);
        UpdateAsset(paths);
        GlobalEvent.DispatchEvent(EditorGlobalEventEnum.OnMoveAsset, t,t1,t2);
    }

    private static void OnDeleteAssetCallBack(AssetDeleteResult t, string t1, RemoveAssetOptions t2)
    {
        List<string> paths = new List<string>();
        paths.Add(t1);
        UpdateAsset(paths);
        GlobalEvent.DispatchEvent(EditorGlobalEventEnum.OnDeleteAsset, t, t1, t2);
    }

    private static void OnCreateAssetCallBack(string t)
    {
        List<string> paths = new List<string>();
        paths.Add(t);
        UpdateAsset(paths);
        GlobalEvent.DispatchEvent(EditorGlobalEventEnum.OnCreateAsset, t);
    }

    private static void UpdateAsset(List<string> paths)
    {
        bool isUpdate = false;
        foreach (var item in paths)
        {
            if (item.Contains("Assets/Resources"))
            {
                isUpdate = true;
                break;
            }
        }
        if (isUpdate)
        {
            AssetsSortEidtorWindow.RefreshResData();
            AssetBundleBuildUtils.CreateAllResPathInfo("Assets/Resources");
            ResourcePathManager.Clear();

            GlobalEvent.DispatchEvent(EditorGlobalEventEnum.OnResourcesAssetsChange);
        }
    }
}

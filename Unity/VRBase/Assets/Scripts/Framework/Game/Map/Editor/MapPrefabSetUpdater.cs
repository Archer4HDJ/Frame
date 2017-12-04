using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HDJ.Framework.Utils;
using System.IO;
using HDJ.Framework.Modules;

public class MapPrefabSetUpdater  {

    [InitializeOnLoadMethod]
    static void RunEditor()
    {
        GlobalEvent.AddEvent(EditorGlobalEventEnum.OnResourcesAssetsChange, OnResourcesChange);

        UnityLayerTagUtils.AddLayer(MapObjectLayer);

    }
    const string mapPrefabPathDir = "Assets/Resources/Prefabs/Maps";
    public const string MapObjectLayer = "Map";
    private static void OnResourcesChange(object[] args)
    {
        CheckMapPrefabForder();
        string[] paths =  PathUtils.GetDirectoryFilePath(mapPrefabPathDir, new string[] { ".prefab" });

        foreach (var item in paths)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(item);

            if (obj.layer == LayerMask.NameToLayer(MapObjectLayer))
                continue;
            obj.layer = LayerMask.NameToLayer(MapObjectLayer);
            EditorUtility.SetDirty(obj);
        }
    }
    public static void CheckMapPrefabForder()
    {
        if (!Directory.Exists(mapPrefabPathDir))
        {
            Directory.CreateDirectory(mapPrefabPathDir);
            AssetDatabase.Refresh();
            return;
        }
    }
}

using HDJ.Framework.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldObjectManager  {

    private static Dictionary<string, GameObject> itemDic = new Dictionary<string, GameObject>();

    public static GameObject CreateObject(string prefabName , string id )
    {

        GameObject obj = PoolObjectManager.GetObject(prefabName);

        if (!itemDic.ContainsKey(id))
            itemDic.Add(id, obj);
        return obj;
    }

  
    public static void DestroyObject(string id)
    {
        GameObject obj = null;
        if (itemDic.ContainsKey(id))
        {
            obj = itemDic[id];
            itemDic.Remove(id);
        }
        if (obj)
        {
            PoolObjectManager.DestroyObject(obj);
        }
    }

    public static GameObject GetItem(string id)
    {
        GameObject obj = null;
        if (itemDic.ContainsKey(id))
        {
            obj = itemDic[id];
        }

        return obj;
    }
}

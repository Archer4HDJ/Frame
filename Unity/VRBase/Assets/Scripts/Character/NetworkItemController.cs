using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkItemController  {

    private static Dictionary<string, GameObject> itemDic = new Dictionary<string, GameObject>();
    public static void AddItem(string id,GameObject obj)
    {
        if (!itemDic.ContainsKey(id))
            itemDic.Add(id, obj);

    }

	public static GameObject RemoveItem(string id)
    {
        GameObject obj=null;
        if (itemDic.ContainsKey(id))
        {
           obj = itemDic[id];
            itemDic.Remove(id);
        }

        return obj;
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

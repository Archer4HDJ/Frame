using UnityEngine;
using System.Collections;
using HDJ.Framework.Modules;

public class GetInstanceObject {

    /// <summary>
    /// 使用路径配置文件获取的地址实例化预制
    /// </summary>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    public static GameObject GetInstance(string prefabName)
    {
        return  GetInstance(null, prefabName);
    }
    /// <summary>
    /// 使用路径配置文件获取的地址实例化预制
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    public static GameObject GetInstance(GameObject parent, string prefabName)
    {
        GameObject prefab = GetPrefab(prefabName);
        if (prefab == null)
            return null;
        GameObject obj = GameObject.Instantiate(prefab) as GameObject;
        obj.name = prefabName;
        if (parent)
        {
            obj.transform.SetParent(parent.transform);
            obj.transform.localPosition = Vector3.zero;
        }
        return obj;

    }


    public static GameObject GetPrefab(string prefabName)
    {
        AssetData[] data = ResourcesManager.LoadAssetsByName(prefabName);

        if (data.Length>0 && data[0].asset != null)
        {
            GameObject obj = data[0].asset as GameObject;
            return obj;
        }

        return null;
    }

    public static void GetInstanceAsync(string prefabName, CallBack<GameObject> callBack)
    {
        ResourcesManager.LoadAssetsAsyncByName(prefabName, (res) =>
        {
            if (res.Length>0 && res[0].asset)
            {
                GameObject prefab = res[0].asset as GameObject;
                GameObject obj = GameObject.Instantiate(prefab) as GameObject;
                obj.name = prefabName;
                if (callBack != null)
                    callBack(obj);
            }
            else
            {
                if (callBack != null)
                    callBack(null);
            }

        });
    }

}

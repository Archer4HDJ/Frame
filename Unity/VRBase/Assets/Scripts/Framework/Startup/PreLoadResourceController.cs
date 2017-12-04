using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HDJ.Framework.Utils;
using HDJ.Framework.Modules;

public static class PreLoadResourceController  {

    public static CallBack<int, int> progressCallBack;
    public static CallBack<string> showContentCallBack;

    public static void StartLoadResource( CallBack completeCallBack)
    {
        AssetData[] ads = ResourcesManager.LoadAssetsByName("AssetBundleInfo");
        if (ads.Length > 0)
        {
            TextAsset tx = ads[0].asset as TextAsset;
            Dictionary<string, object> tempDic = InternalConfigManager.LoadData(tx.text);
            string s = tempDic["preLoadResPaths"].ToString();
            List<string> pathNames = tempDic["preLoadResPaths"] as List<string>;
            int count = pathNames.Count;
            ResourcesManager.LoadManyAssetAsyncByName(pathNames, (num) =>
            {
                int left = count - num + 1;
                //Debug.LogError("Left :" + num);
                if (progressCallBack != null)
                {
                    progressCallBack(count, count - num);
                }
                if (showContentCallBack != null)
                {
                    string ss ="Preload("+(count-num)+" / " + count+")";
                    showContentCallBack(ss);
                }
            }, completeCallBack);
        }
        else
        {
            if (completeCallBack != null)
                completeCallBack();
        }
    }

   
}

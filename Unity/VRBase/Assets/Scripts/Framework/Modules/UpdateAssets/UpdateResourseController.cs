using HDJ.Framework.Modules;
using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UpdateResourseController  {
    public static CallBack<int, int> progressCallBack;
    public static CallBack<string> showContentCallBack;

    static private string platform;
    static string PathDir;
    static string serverPath = @"file:///C:/UnityUpdateTest";

    // Use this for initialization
   static public void StartUpdating(MonoBehaviour mono,string serverPath, CallBack completCalback) {
         platform = OtherUtils. GetPlatformFolder(Application.platform);
        PathDir = Application.persistentDataPath + "/" + platform;
        UpdateResourseController. serverPath = serverPath;
        Debug.Log("PathDir: " + PathDir);
        showContentCallBack("Check for updates");
        mono.StartCoroutine(Updating(completCalback));
	}

   static  IEnumerator Updating(CallBack completCalback)
    {
        //加载沙河路径下的AssetBundleInfo
        string assetBundleInfoPer = FileUtils.LoadTextFileByPath(PathDir + "/AssetBundleInfo.txt");
        string assetBundleInfoStream = "";
        string tempAbInfoPath = OtherUtils.GetWWWLoadPath(Application.streamingAssetsPath + "/AssetBundleInfo.txt");
        //加载StreamAssets下的AssetBundleInfo
        yield return FileUtils.LoadTxtFileIEnumerator(tempAbInfoPath , (data) => { assetBundleInfoStream = data; });
        string bundleVersion = "";
        int updatePackVersion = 0;
        Debug.Log("加载StreamAssets下的AssetBundleInfo ：" + assetBundleInfoStream);
        Dictionary<string, object> tempDic = InternalConfigManager.LoadData(assetBundleInfoStream);
        bundleVersion = tempDic["bundleVersion"].ToString();

        if (!string.IsNullOrEmpty(assetBundleInfoPer))
        {
            Dictionary<string, object> dic = InternalConfigManager.LoadData(assetBundleInfoPer);
            string streamver1 = dic["updatePackVersion"].ToString();
            string bunStreamVer1 = dic["bundleVersion"].ToString();

            if (bundleVersion == bunStreamVer1)
            {
                updatePackVersion = int.Parse(streamver1.Split('-')[2]);
            }

        }
        Debug.Log("updatePackVersion: " + updatePackVersion);

        string updatePackData = "";
        showContentCallBack("Download UpdatePackData");
        //服务器下载updatePackData
        yield return FileUtils.LoadTxtFileIEnumerator(serverPath + "/" + platform + "/updatePackData.txt", (data) => { updatePackData = data; });
        List<string> upVerList = new List<string>();
        Debug.Log("服务器下载updatePackData :" + updatePackData);
        if (!string.IsNullOrEmpty(updatePackData))
        {
            Dictionary<string, object> updatePackDataDic = InternalConfigManager.LoadData(updatePackData);
            if (updatePackDataDic.ContainsKey(bundleVersion))
            {
                List<string> upVerListTemp = (List<string>)updatePackDataDic[bundleVersion];

                for (int i = 0; i < upVerListTemp.Count; i++)
                {
                    int temp = int.Parse(upVerListTemp[i].Split('-')[2]);
                    if (temp > updatePackVersion)
                    {
                        Debug.Log("updatePackVersion : " + updatePackVersion + "  upVerListTemp[i]:" + upVerListTemp[i]);
                        upVerList.Add(upVerListTemp[i]);
                    }
                }
            }
        }

        for (int i = 0; i < upVerList.Count; i++)
        {
            int count = upVerList.Count;
            string serverFilePath = serverPath + "/" + platform + "/" +bundleVersion+"/" +upVerList[i] +"/"+upVerList[i] + ".zip";
            string fileTempSavePath = PathDir + "/" + upVerList[i] + ".zip";
            showContentCallBack("Download Update Pack："+ upVerList[i]);
            progressCallBack(count, i + 1);
            Debug.Log("更新包：" + serverFilePath);
            yield return DownloadUpdateZipFile(serverFilePath, fileTempSavePath);
        }
        yield return new WaitForEndOfFrame();
        Debug.Log("更新完成");
        if (completCalback != null)
            completCalback();
    }

    static IEnumerator DownloadUpdateZipFile(string serverFilePath,string fileTempSavePath)
    {
        WWW www = new WWW(serverFilePath);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            string tempDir = Path.GetDirectoryName(fileTempSavePath);
            try
            {
                
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);

                FileStream stream = new FileStream(fileTempSavePath, FileMode.OpenOrCreate);
                byte[] dataByte = www.bytes;
                stream.Write(dataByte, 0, dataByte.Length);
                stream.Close();
            }catch(Exception e)
            {
                Debug.LogError("文件写入失败："+e);
            }
            ZipUtils.UnZip(fileTempSavePath, tempDir);
            FileUtils.DeleteFile(fileTempSavePath);

            //string zipFileName = Path.GetFileNameWithoutExtension(fileTempSavePath);

           string tempPathData = FileUtils.LoadTextFileByPath(tempDir + "/UpdatePathFile.txt");
            Dictionary<string, string> pathDaraDic = ResourcePathManager.LoadPathData(tempPathData);

            string tempPathData1 = FileUtils.LoadTextFileByPath(tempDir + "/PathFile.txt");
            Dictionary<string, string> pathDaraDic1 = ResourcePathManager.LoadPathData(tempPathData);

            foreach(string key in pathDaraDic.Keys)
            {
                if (pathDaraDic1.ContainsKey(key))
                    pathDaraDic1[key] = pathDaraDic[key];
                else
                    pathDaraDic1.Add(key, pathDaraDic[key]);

                string oldPath = tempDir +"/" + Path.GetFileName(pathDaraDic[key]);
                string newPath = tempDir + "/" + pathDaraDic[key];
                FileUtils.MoveFile(oldPath, newPath);

            }
            string PathFileData = "";
            foreach (string key in pathDaraDic1.Keys)
            {
                PathFileData += key + "," + pathDaraDic1[key] + "&";
            }
            FileUtils.CreateTextFile(tempDir + "/PathFile.txt", PathFileData);
            FileUtils.DeleteFile(tempDir + "/UpdatePathFile.txt");
        }
        else
        {
            Debug.LogError("DownloadUpdateZipFile error: " + www.error);
        }
        yield return new WaitForEndOfFrame();
    }

  
}

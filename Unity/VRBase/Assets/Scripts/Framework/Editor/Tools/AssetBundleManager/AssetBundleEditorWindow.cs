using HDJ.Framework.Modules;
using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class AssetBundleEditorWindow : EditorWindow {

    [MenuItem("Tool/AssetBundle资源打包(1100)",priority =1100)]
    static void OpenWindow()
    {
        GetWindow<AssetBundleEditorWindow>().Init();
    }
    string assetPath = "Assets/Resources/";
    const string AssetBundlesOutputPath = "Assets/StreamingAssets";
    string settingDataPath = "GameSetingConfigs/AssetBundleSettingData.asset";
    private Color oldGUIColor;
    private void Init()
    {
        oldGUIColor = GUI.color;
        if (!Directory.Exists(assetPath))
        {
            Directory.CreateDirectory(assetPath);
        }

        settingData = ScriptableObjectUtils.LoadCreateScriptableObject<AssetBundleSettingData>(assetPath+settingDataPath);
        settingData.Init();
   
        UpdateFileExtension();
        UpdatePackageController.Init();
        gameVersionInfo = UpdatePackageController.GetGameVersionInfo(PlayerSettings.productName);
        if (gameVersionInfo == null)
        {
            gameVersionInfo = new GameVersionInfo();
            gameVersionInfo.forceUpdateVersion = PlayerSettings.bundleVersion;
            gameVersionInfo.latestVersion = PlayerSettings.bundleVersion;
        }

        versionControlInfo = UpdatePackageController.GetVersionControlInfo(PlayerSettings.productName, PlayerSettings.bundleVersion);
        if (versionControlInfo == null)
            versionControlInfo = new VersionControlInfo();
        int num = GetMaxNumber(versionControlInfo.assetBundleVersions);
        if (num == -1)
        {
            assetBundleInfo = new AssetBundleInfo();
            assetBundleInfo.gameName = PlayerSettings.productName;
            assetBundleInfo.gameVersion = PlayerSettings.bundleVersion;
            assetBundleInfo.assetBundleVersion = 0;

        }
        else
        {
            assetBundleInfo = UpdatePackageController.GetAssetBundleInfo(PlayerSettings.productName, PlayerSettings.bundleVersion, num);
            assetBundleInfo.assetBundleVersion = assetBundleInfo.assetBundleVersion+1;
        }

    }
    //void OnEnable()
    //{
    //    Init();
    //}
    private int buildVersion;
    private GameVersionInfo gameVersionInfo;
    private VersionControlInfo versionControlInfo;
    private AssetBundleInfo assetBundleInfo;

    AssetBundleSettingData settingData;

    private int toolbarOption = 0;
    private string[] toolbarTexts = { "AssetBundle", "相关设置", "更新包" };

    private int toolbarOptionSec = 0;
    private string[] toolbarTextsSec = { "打包目录", "打包资源后缀名", "预加载文件" };

    void OnGUI()
    {

        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts,GUILayout.Width(Screen.width));
        switch (toolbarOption)
        {
            case 0:
                AssetBundleMainGUI();
                break;
            case 1:
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                toolbarOptionSec = GUILayout.Toolbar(toolbarOptionSec, toolbarTextsSec, GUILayout.Width(Screen.width-40));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                switch (toolbarOptionSec)
                {
                    case 0:
                        GUILayout.Space(10);
                        DrawFileDirectoryGUI();
                        break;
                    case 1:
                        GUILayout.Space(10);
                        AddPackageFileExtensionGUI();
                       
                        GUILayout.FlexibleSpace();
                        break;
                    case 2:
                        GUILayout.Space(10);
                        SetPreLoadFileGUI();
                        break;

                }
                break;
            case 2:
                BackUpAndUpdateGUI();
                break;

        }

    }
    void OnDestroy()
    {
        fileData = null;
    }

    #region 预加载文件选择
    TreeModelController<FileData> preLoadFileData;
    private Vector2 pos1 = Vector2.zero;
    private Vector2 pos4 = Vector2.zero;
    private string[] tempAllFilePath;
    void SetPreLoadFileGUI()
    {
        
        GUILayout.Space(7);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Refresh", GUILayout.Width(60)))
        {
            preLoadFileData = null;
            return;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        if (preLoadFileData == null)
        {
            string[] paths = PathUtils.GetDirectoryFilePath(AssetBundlesOutputPath);
            preLoadFileData = EditorDrawFileDirectory.GetFileDirectoryInfo(paths, true);
            //paths = PathUtils.RemovePathWithEnds(paths, new string[] { ".meta" });
            UpdatePreLoadFileData(preLoadFileData);
            tempAllFilePath = PathUtils.GetDirectoryFilePath(AssetBundlesOutputPath);
        }
        GUILayout.Label("选择预加载文件：");
        pos1 = GUILayout.BeginScrollView(pos1, "box");
        EditorDrawFileDirectory.DrawFileDirectory(preLoadFileData, ShowFileDirectoryType.ShowAllFile,new string[] { ".assetbundle" }, null, true, PreLoadChooseCallBack);
      
        GUILayout.EndScrollView();

        GUILayout.Space(8);
        GUILayout.Label("选中的文件路径：");
        pos4 = GUILayout.BeginScrollView(pos4, "box");
        foreach (string p in settingData.preLoadResPaths)
        {
            GUILayout.BeginHorizontal("box");
            if (!OtherUtils.ArrayContains(tempAllFilePath, p))
                GUI.color = Color.red;
            GUILayout.Label("预加载路径:" + p);
            if (GUI.color == Color.red)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("文件不存在");
            }
            GUI.color = oldGUIColor;
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }
        GUILayout.EndScrollView();
        GUILayout.Space(10);
        if (GUILayout.Button("移除不存在预加载文件路径"))
        {
            List<string> tList = new List<string>();
            foreach (string p in settingData.preLoadResPaths)
            {
                if (!OtherUtils.ArrayContains(tempAllFilePath, p))
                {
                    tList.Add(p);
                }
            }

            foreach(string p in tList)
            {
                settingData.preLoadResPaths.Remove(p);
            }
        }
    }
    void UpdatePreLoadFileData(TreeModelController<FileData> data)
    {
        data.ListForeachNode((n) =>
        {
            if (settingData.preLoadResPaths.Contains(n.relativeRootPath))
                n.isChoose = true;
            return true;
        });      
    }
    void PreLoadChooseCallBack(FileData data)
    {
        if (data.isDirectory)
        {
            data.isChoose = false;
            return;
        }
        if (data.isChoose)
        {
            settingData.preLoadResPaths.Add(data.relativeRootPath);
        }
        else
        {
            settingData.preLoadResPaths.Remove(data.relativeRootPath);
        }
    }
    #endregion

    
    void AssetBundleMainGUI()
    {
        GUILayout.Space(7);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Reset", GUILayout.Width(60)))
        {
            settingData.Reset();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginVertical("box");

        GUILayout.Label("Game Name :" + PlayerSettings.productName);
        GUILayout.Label ("Game Version :" + PlayerSettings.bundleVersion);
        buildVersion = (int)EditorDrawGUIUtil.DrawBaseValue("AssetBundle Version", buildVersion);


        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("移除StreamingAssets文件夹"))
        {
            if (Directory.Exists(AssetBundlesOutputPath))
            {
                Directory.Delete(AssetBundlesOutputPath, true);
                AssetDatabase.Refresh();
            }
        }
        //GUILayout.Space(7);
        //if (GUILayout.Button("创建Resources下资源路径文件"))
        //{
        //    AssetBundleBuildUtils.CreateAllResPathInfo("Assets/Resources");
        //}
        GUILayout.Space(7);
        if (GUILayout.Button("生成依赖包"))
        {
            AssetBundleBuildUtils.ClearAssetBundlesName();
            AssetBundleBuildUtils.SetAllResourceBundleName(assetPath, settingData.packageFileExtension.ToArray());
            SetPakagOneBundleName();
            AssetBundleBuildUtils.BuildAssetBundle(AssetBundlesOutputPath);

            assetBundleInfo.packageOnePaths = settingData.packageOnePaths;
            assetBundleInfo.preLoadResPaths = settingData.preLoadResPaths;

            AssetBundleBuildUtils.CreateBundleVersionFile(AssetBundlesOutputPath, assetBundleInfo);

            AssetBundleBuildUtils.CreateOneFileBundle(AssetBundlesOutputPath, "Assets/StreamingAssets/AssetBundleInfo.txt", "AssetBundleInfo.txt", "AssetBundleInfo.assetbundle");
            AssetBundleBuildUtils.CreateAllResPathInfo(AssetBundlesOutputPath, new string[] { ".assetbundle" ,""});
            AssetBundleBuildUtils.CreateAllResMD5AndPath(AssetBundlesOutputPath, new string[] { ".assetbundle", ".manifest" });
            
            AssetBundleBuildUtils.CreateOneFileBundle(AssetBundlesOutputPath, "Assets/StreamingAssets/PathFile.txt", "PathFile.txt", "PathFile.assetbundle");


            AssetDatabase.Refresh();
        }
        GUILayout.Space(7);
        //if (GUILayout.Button("备份StreamingAssets文件夹"))
        //{
        //    string resPath = AssetBundleBackUpControl.GetBackUpDataPath(settingData.bundleVersion, settingData.buildVersion);
        //    if (AssetBundleBackUpControl.IsCintainsBackUpdata(settingData.bundleVersion, settingData.buildVersion))
        //    {
        //       if( EditorUtility.DisplayDialog("警告", "该版本已存在，是否覆盖？", "是", "取消"))
        //        {
        //            if (Directory.Exists(resPath))
        //                Directory.Delete(resPath, true);
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }
        //    AssetBundleBackUpControl.AddBackUpData(settingData.bundleVersion, settingData.buildVersion);
        //    AssetBundleBackUpControl.SaveData();
           
        //    FileUtils.CopyDirectory(AssetBundlesOutputPath, resPath);
        //    toolbarOption = 2;
        //}
        //GUILayout.Space(7);
    }

    void SetPakagOneBundleName()
    {
        foreach (string path in settingData.packageOnePaths)
        {
            AssetBundleBuildUtils.SetPakagOneBundleName(path, settingData.packageFileExtension.ToArray());
        }
    }


    #region  文件夹目录选择打成一包逻辑
    private Vector2 pos0 = Vector2.zero;
    TreeModelController<FileData> fileData;
    void DrawFileDirectoryGUI()
    {
        if (fileData == null)
        {
            string[] paths = PathUtils.GetDirectoryFilePath(assetPath);
            fileData = EditorDrawFileDirectory.GetFileDirectoryInfo(paths);
            UpdateChooseFileData(fileData);
        }
        GUILayout.Label("选择打成一个包的目录：");
        pos0 = GUILayout.BeginScrollView(pos0, "box");
        EditorDrawFileDirectory.DrawFileDirectory(fileData, ShowFileDirectoryType.OnlyDirectory, null, null, true, PakagOneChooseCallBack);
        GUILayout.EndScrollView();
    }

    void PakagOneChooseCallBack(FileData data)
    {
        if (data.relativeRootPath == fileData.RootNode.relativeRootPath)
        {
            data.isChoose = false;
            return;
        }
        //  Debug.Log("ChooseCallBack : " + data.assetPath);
        UpdatesettingDataPath(data);
        UpdataChooseParent(data);
        if (data.isChoose)
        {
            UpdataChooseChildState(data);
        }
    }
    void UpdatesettingDataPath(FileData data)
    {
        if (!data.isChoose)
        {
            if (settingData.packageOnePaths.Contains(data.relativeRootPath))
                settingData.packageOnePaths.Remove(data.relativeRootPath);
        }
        else
        {
            settingData.packageOnePaths.Add(data.relativeRootPath);
        }
    }
    void UpdateChooseFileData(TreeModelController<FileData> data)
    {
        data.ListForeachNode((n) =>
        {
            if (settingData.packageOnePaths.Contains(n.relativeRootPath))
                n.isChoose = true;
            return true;
        });
    }

    void UpdataChooseParent(FileData data)
    {     
        FileData f = data;
        while (f.relativeRootPath != fileData.RootNode.relativeRootPath)
        {
            f = fileData.GetParentNode(f);
            if(f.isChoose)
            {
                data.isChoose = false;
                break;
            }
        }
    }
    void UpdataChooseChildState(FileData data)
    {

        fileData.SearchChilds(data, (d) =>
         {
             d.isChoose = false;
             UpdatesettingDataPath(d);
             UpdataChooseChildState(d);
             return true;
         });
      
    }
    #endregion


    #region 编辑资源后缀名
    List<string> allFileExtension = new List<string>();
    List<string> canAddFileExtension = new List<string>();
    void UpdateFileExtension()
    {
        string[] allFileName = PathUtils.GetDirectoryFileNames(assetPath, null, true);
        allFileExtension.Clear();
        canAddFileExtension.Clear();
        foreach (string s in allFileName)
        {
            string ex = Path.GetExtension(s);
            if (!allFileExtension.Contains(ex) && ex != ".meta")
                allFileExtension.Add(ex);
        }
        if (settingData.packageFileExtension.Count == 0)
        {
            foreach (string s in allFileExtension)
            {
                settingData.packageFileExtension.Add(s);
            }
        }

        foreach (string s in allFileExtension)
        {
            if (!settingData.packageFileExtension.Contains(s))
                canAddFileExtension.Add(s);
        }
    }
    string selectEnd = "";
    bool tempForder = false;
    //  Vector2 pos1 = Vector2.zero;
    void AddPackageFileExtensionGUI()
    {
        GUILayout.Label("需要打包资源的后缀名：");
        if (canAddFileExtension.Count > 0)
        {
            GUILayout.BeginHorizontal("box");
            selectEnd = EditorDrawGUIUtil.DrawPopup("添加", selectEnd, canAddFileExtension);
            if (GUILayout.Button("Add", GUILayout.Width(80)))
            {
                settingData.packageFileExtension.Add(selectEnd);
                UpdateFileExtension();
                return;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(5);
        tempForder = EditorGUILayout.Foldout(tempForder, "资源后缀");
        if (tempForder)
        {
            foreach (string s in settingData.packageFileExtension)
            {
                if (GUILayout.Button(s, GUILayout.Width(80)))
                {
                    settingData.packageFileExtension.Remove(s);
                    UpdateFileExtension();
                    return;
                }
            }
        }

    }
    #endregion

    #region 备份，跟新包
    string selecttemp = "";
    bool tempForder1 = true;
    Vector2 pos2 = Vector2.zero;
    Vector2 pos3 = Vector2.zero;

    void BackUpAndUpdateGUI()
    {
        GUILayout.Space(7);
        GUILayout.Box("Game Version : " + PlayerSettings.bundleVersion);

        gameVersionInfo.latestVersion = EditorDrawGUIUtil.DrawBaseValue("最新版本：", gameVersionInfo.latestVersion).ToString();
        gameVersionInfo.forceUpdateVersion = EditorDrawGUIUtil.DrawBaseValue("最小强制更新包版本：", gameVersionInfo.forceUpdateVersion).ToString();


        versionControlInfo.assetBundleVersions.Sort();

        GUILayout.Box("Assset Bundle Update Package :");
        foreach (var item in versionControlInfo.assetBundleVersions)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(item.ToString());
            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {

                if (EditorUtility.DisplayDialog("警告", "是否删除该更新版本？", "是", "取消"))
                {
                    versionControlInfo.assetBundleVersions.Remove(item);

                    return;
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }
        int num = GetMaxNumber(versionControlInfo.assetBundleVersions) + 1;
        GUILayout.Label("当前生成更新包版本 ：" + num);
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create Package"))
        {
            if (num == 0)
                versionControlInfo.assetBundleVersions.Add(num);


            int res = 0;
            if (num != 0)
            {
                string md5Data = UpdatePackageController.GetPathMD5File(PlayerSettings.productName, PlayerSettings.bundleVersion, num - 1);
                string md5DataNew = FileUtils.LoadTextFileByPath(Application.streamingAssetsPath + "/" + UpdateAssetsConst.PathMD5FileName);
                res = CreateUpdatePack(md5Data, md5DataNew);
                Debug.Log("更新包包含更新文件个数 ：" + res);
                
            }
            if (num == 0 || res > 0)
            {
                //GameVersionInfo
                FileUtils.CreateTextFile(UpdatePackageController.localDirPath + "/" + UpdateAssetsConst.GameVersionInfoFileName, JsonUtils.ClassOrStructToJson(gameVersionInfo));
                UpdatePackageController.UpLoadFile(UpdatePackageController.localDirPath +"/" + UpdateAssetsConst.GameVersionInfoFileName, assetBundleInfo.gameName);
                //VersionControlInfo
                FileUtils.CreateTextFile(UpdatePackageController.localDirPath + "/" + UpdateAssetsConst.VersionControlInfoFileName, JsonUtils.ClassOrStructToJson(versionControlInfo));
                string tempPath = assetBundleInfo.gameName + "/" + assetBundleInfo.gameVersion;
                UpdatePackageController.UpLoadFile(UpdatePackageController.localDirPath + "/" + UpdateAssetsConst.VersionControlInfoFileName, tempPath);

                //PathMD5File
                tempPath = assetBundleInfo.gameName + "/" + assetBundleInfo.gameVersion + "/" + assetBundleInfo.assetBundleVersion;
                UpdatePackageController.UpLoadFile(Application.streamingAssetsPath + "/" + UpdateAssetsConst.PathMD5FileName, tempPath);

                if (res > 0)
                {
                    //update.zip
                    UpdatePackageController.UpLoadFile(UpdatePackageController.localDirPath + "/" + UpdateAssetsConst.UpdatePackageFileName, tempPath);

                }
            }
        }

        GUILayout.Space(10);
    }

    private int GetMaxNumber(List<int> list)
    {
        int num = -1;
        foreach (var item in list)
        {
            if (item > num)
                num = item;
        }

        return num;
    }
    #endregion

    int CreateUpdatePack(string oldData, string newData)
    {
      string updateZipDir=  UpdatePackageController.localDirPath;
        string zipFileName = UpdateAssetsConst.UpdatePackageFileName;

        Dictionary<string, string> olderDic = GetMD5FileData(oldData);
        Dictionary<string, string> newDic = GetMD5FileData(newData);

        Dictionary<string, string> resDic = new Dictionary<string, string>();
        Dictionary<string, string> paths = AssetBundleBuildUtils.GetPathFileData(Application.streamingAssetsPath + "/PathFile.txt");

        foreach (string key in newDic.Keys)
        {
            if (!olderDic.ContainsKey(key))
            {
                resDic.Add(key, paths[key]);
            }
        }
        int num = resDic.Count;
        if (num <= 0)
        {
            return num;
        }
        string fileData = "";
        foreach (string s in resDic.Keys)
        {
            fileData += s + "," + resDic[s] + "&";
        }
        FileUtils.CreateTextFile(updateZipDir + "/UpdatePathFile.txt", fileData);


        List<string> zipPaths = new List<string>();
        foreach (string s in resDic.Values)
        {
            zipPaths.Add(Application.streamingAssetsPath  + "/" + s);
        }
        zipPaths.Add(updateZipDir + "/UpdatePathFile.txt");
        zipPaths.Add(updateZipDir + "/AssetBundleInfo.txt");

        string zipPath = updateZipDir + "/" + zipFileName+".zip";
        ZipUtils.ZipManyFilesOrDictorys(zipPaths, zipPath, null);
        FileUtils.DeleteFile(updateZipDir + "/UpdatePathFile.txt");


        return num;
    }

    Dictionary<string,string> GetMD5FileData(string data)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();

        Debug.Log("data: " + data);
        string[] sss = data.Split( '&');
        foreach(string ss in sss)
        {
            if (string.IsNullOrEmpty(ss))
                continue;
            string[] s = ss.Split(',');

            dic.Add(s[0], s[1]);
        }

        return dic;
    }


}




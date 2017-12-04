using UnityEngine;
using System.Collections;
using HDJ.Framework.Tools;
using HDJ.Framework.Utils;
using System.Collections.Generic;
using System.IO;

namespace HDJ.Framework.Modules
{
    public static class UpdatePackageController 
    {
        static FTPTool ftp;
     public   static string localDirPath = "";
        public static void Init()
        {
            MessageStringData data = Resources.Load<MessageStringData>("GameRunSettingData");
            string ip = data.GetValue("ftpServerIP");
            string userName = data.GetValue("ftpUserID");
            string pw = data.GetValue("ftpPassword");
           ftp = new FTPTool(ip, userName, pw);

            localDirPath = Application.dataPath.Replace("Assets", "TempUpdateAB");

            if (Directory.Exists(localDirPath))
                Directory.Delete(localDirPath, true);
        }
       
        public static GameVersionInfo GetGameVersionInfo(string gameName)
        {
           bool res=  ftp.DownLoadFile(localDirPath, gameName+"/"+UpdateAssetsConst.GameVersionInfoFileName);
            if (!res)
                return null;
            string data = FileUtils.LoadTextFileByPath(localDirPath + "/" + UpdateAssetsConst.GameVersionInfoFileName);
            return JsonUtils.JsonToClassOrStruct<GameVersionInfo>(data);
        }

        public static VersionControlInfo GetVersionControlInfo(string gameName, string gameVer)
        {
            bool res = ftp.DownLoadFile(localDirPath, gameName+"/"+gameVer+"/"+ UpdateAssetsConst.VersionControlInfoFileName);
            if (!res)
                return null;
            string data = FileUtils.LoadTextFileByPath(localDirPath + "/" + UpdateAssetsConst.VersionControlInfoFileName);
            return JsonUtils.JsonToClassOrStruct<VersionControlInfo>(data);
        }

        public static string GetPathMD5File(string gameName, string gameVer,int assetBundleVersion)
        {
            bool res = ftp.DownLoadFile(localDirPath, gameName + "/" + gameVer + "/" + assetBundleVersion +"/"+ UpdateAssetsConst.PathMD5FileName);
            if (!res)
                return null;
            string data = FileUtils.LoadTextFileByPath(localDirPath + "/" + UpdateAssetsConst.PathMD5FileName);
            return data;
        }

        public static AssetBundleInfo GetAssetBundleInfo(string gameName, string gameVer, int assetBundleVersion)
        {
            bool res = ftp.DownLoadFile(localDirPath, gameName + "/" + gameVer + "/" + assetBundleVersion + "/" + UpdateAssetsConst.UpdatePackageFileName);
            if (!res)
                return null;
            ZipUtils.UnZip(localDirPath + "/" + UpdateAssetsConst.UpdatePackageFileName, localDirPath);
            string data = FileUtils.LoadTextFileByPath(localDirPath + "/" + UpdateAssetsConst.AssetBundleInfoFileName);
            return JsonUtils.JsonToClassOrStruct<AssetBundleInfo>(data);
        }

        public static void UpLoadFile(string loacalPath,string remotePath)
        {
            ftp.FtpUploadFile(loacalPath, remotePath);
        }
    }
}

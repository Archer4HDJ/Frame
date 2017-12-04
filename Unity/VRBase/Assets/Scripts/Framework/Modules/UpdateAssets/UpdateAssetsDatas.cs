using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Modules
{
    public class GameVersionInfo
    {
        /// <summary>
        /// 最新版本
        /// </summary>
        public string latestVersion;
        /// <summary>
        /// 需要签字更新的最小版本（小于这个版本的需要强制更新包）
        /// </summary>
        public string forceUpdateVersion;
    }
    public class VersionControlInfo 
    {
        /// <summary>
        /// AssetBundle更新包版本
        /// </summary>
        public List<int> assetBundleVersions = new List<int>();
    }

    public class AssetBundleInfo
    {
        public string gameName;
        public string gameVersion;
        public int assetBundleVersion;

        //打成一个bundle包的目录
        public List<string> packageOnePaths = new List<string>();

        //需要预加载的目录
        public List<string> preLoadResPaths = new List<string>();
    }
}

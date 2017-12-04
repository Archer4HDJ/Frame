using UnityEngine;
using System.Collections;
using System.IO;

namespace  HDJ.Framework.Modules
{
    /// <summary>
    /// 本地资源信息储存类
    /// </summary>
    [System.Serializable]
    public class AssetData
    {
        public AssetData(string path)
        {
            assetPath = path;
        }

        private string assetName = "";
        /// <summary>
        /// 资源名字
        /// </summary>
        public string AssetName
        {
            get
            {
                assetName = Path.GetFileNameWithoutExtension(assetPath);
                return assetName;
            }
        }
        public string assetPath;//资源路径

        public Object asset;

    }
}

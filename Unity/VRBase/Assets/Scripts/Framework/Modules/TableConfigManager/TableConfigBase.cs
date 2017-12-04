using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HDJ.Framework.Modules
{
    [System.Serializable]
    public class TableConfigBase
    {
        public static Dictionary<string, TableConfigOtherInfo> tableConfigOtherInfo = new Dictionary<string, TableConfigOtherInfo>();

    }

    public class TableConfigOtherInfo
    {
        /// <summary>
        /// 配置描述
        /// </summary>
        public string configDescription = "";
        /// <summary>
        /// 字段描述信息
        /// </summary>
        public Dictionary<string, TableConfigFieldInfo> fieldInfoDic = new Dictionary<string, TableConfigFieldInfo>();
    }

    public class TableConfigFieldInfo
    {
        public string fieldName = "";
        public string description = "";
        public ConfigFieldValueType fieldValueType;
        public object defultValue;

    }

    public enum ConfigFieldValueType
    {
        Int,
        Float,
        Bool,
        String,
        Vector2,
        Vector3,
        Int_Array,
        Float_Array,
        Bool_Array,
        String_Array,
        Vector2_Array,
        Vector3_Array,
    }
}

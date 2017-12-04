using  HDJ.Framework.Modules;
using HDJ.Framework.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Modules
{
    public static class LocalizedLanguageManager
    {
        //本地化语言含有那几种文件名
        public const string LanguagesTypeFileName = "LanguageTypeNames";
        //是否最优先使用系统语言
        public const string GameSettingKeyName_IsUseSystemLanguage = "IsUseSystemLanguage";
        //当不使用系统语言或无法使用时， 优先使用的语言
        public const string GameSettingKeyName_PriorityLanguage = "PriorityLanguage";
        //当返回语言为Chinese时，优先使用简体还是繁体
        public const string GameSettingKeyName_ChineseLanguagePriorityLanguage = "ChineseLanguagePriorityLanguage";

        public const string GameRuntimeStore_UserSelectLanguage = "UserSelectLanguage";

        public static CallBack<string> SwitchLanguageCallback;
        public static string currentLanguage = "";


        private static List<string> LanguageTypeNames;
        private static Dictionary<string, string> languageDataDic = new Dictionary<string, string>();

        private static string[] chineseLangusge = { "ChineseSimplified", "ChineseTraditional" };
        private static bool isInit = false;
        // Use this for initialization
        private static void Initialize()
        {
            if (isInit)
                return;
            isInit = true;

            AssetData[] res = ResourcesManager.LoadAssetsByName(LanguagesTypeFileName);
            string text = "";
            if (res.Length > 0)
            {
                TextAsset tx = (TextAsset)res[0].asset;
                text = tx.text;
            }
            LanguageTypeNames = JsonUtils.JsonToList<string>(text);

            string userSelect = GameRuntimeStoreManager.GetValue(GameRuntimeStore_UserSelectLanguage, "");
            if (!string.IsNullOrEmpty(userSelect))
            {
                LoadLanguageData(userSelect);
                return;
            }

            bool IsUseSystemLanguage = GameSettingStoreManager.GetValue(GameSettingKeyName_IsUseSystemLanguage, true);
            string PriorityLanguage = GameSettingStoreManager.GetValue(GameSettingKeyName_PriorityLanguage, "");
            string ChineseLanguagePriorityLanguage = GameSettingStoreManager.GetValue(GameSettingKeyName_ChineseLanguagePriorityLanguage, "");
            if (IsUseSystemLanguage)
            {
                string sysLan = Application.systemLanguage.ToString();
                if (LanguageTypeNames.Contains(sysLan))
                {
                    LoadLanguageData(sysLan);
                    return;
                }
                else
                {
                    if (sysLan == SystemLanguage.Chinese.ToString())
                    {
                        if (LanguageTypeNames.Contains(ChineseLanguagePriorityLanguage))
                            LoadLanguageData(ChineseLanguagePriorityLanguage);
                        else if (LanguageTypeNames.Contains(chineseLangusge[0]))
                            LoadLanguageData(chineseLangusge[0]);
                        else if (LanguageTypeNames.Contains(chineseLangusge[1]))
                            LoadLanguageData(chineseLangusge[1]);
                    }
                }
            }

            LoadLanguageData(PriorityLanguage);
        }

        private static void LoadLanguageData(string languageName)
        {
            currentLanguage = languageName;

            AssetData[] res = ResourcesManager.LoadAssetsByName(LanguagesTypeFileName);
            string text = "";
            if (res.Length > 0)
            {
                TextAsset tx = (TextAsset)res[0].asset;
                text = tx.text;
            }
            languageDataDic = JsonUtils.JsonToDictionary<string, string>(text);
        }

        public static void Clear()
        {
            languageDataDic.Clear();
            isInit = false;
        }
        public static void SwichLanguage(string languageName)
        {
            Initialize();
            LoadLanguageData(languageName);
            if (SwitchLanguageCallback != null)
                SwitchLanguageCallback(languageName);
        }
        public static string[] GetSupportLanguageName()
        {
            Initialize();
            return LanguageTypeNames.ToArray();
        }
        public static string GetText(string key, params object[] paras)
        {
            Initialize();

            string value = "";
            if (languageDataDic.ContainsKey(key))
                value = languageDataDic[key];
            if (value == "")
            {
                Debug.LogError("Key no find! key:" + key);

                return "Key no find! key:" + key;
            }

            if (paras == null || paras.Length == 0)
                return value;
            else
            {
                string text = value;
                for (int i = 0; i < paras.Length; i++)
                {
                    string temp = "{" + i + "}";
                    if (text.Contains(temp))
                        text = text.Replace(temp, paras[i].ToString());
                }
                return text;
            }
        }
    }
}

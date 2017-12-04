using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HDJ.Framework.Utils;
using HDJ.Framework.Modules;

public class LocalizedLanguageEditorWindow : EditorWindow {

    private const string SaveFilePathDir = "Assets/Resources/Configs/LocalizedLanguageConfig";

    [MenuItem("Tool/多语言管理器",priority = 1103)]
	private static void OpenWindow()
    {
        GetWindow<LocalizedLanguageEditorWindow>();
    }

    void OnEnable()
    {
        string ks =  FileUtils.LoadTextFileByPath(SaveFilePathDir + "/LanguageKeys.txt");
        keysList = JsonUtils.JsonToList<string>(ks);

        ks = JsonUtils.ListToJson(new List<string>(langDataDic.Keys));
       ks =  FileUtils.LoadTextFileByPath(SaveFilePathDir + "/"+ LocalizedLanguageManager .LanguagesTypeFileName+ ".txt");
      List<string> LanguageTypeNames = JsonUtils.JsonToList<string>(ks);

        foreach (var item in LanguageTypeNames)
        {
           ks= FileUtils.LoadTextFileByPath(SaveFilePathDir + "/Languages/" + item + ".txt");
            Dictionary<string, string> dic= JsonUtils.JsonToDictionary<string,string>(ks);
            //Debug.Log(dic.Count);
            //foreach (var i in dic)
            //{
            //    Debug.Log(i.Key + "==>" + i.Value);
            //}
            langDataDic.Add(item, dic);
        }

        CheckKeys();
    }
    private List<string> keysList = new List<string>();
    private Dictionary<string, Dictionary<string, string>> langDataDic = new Dictionary<string, Dictionary<string, string>>();

    private void AddKey(string key)
    {
        //Debug.Log("Add Key :" + key);
        if (!keysList.Contains(key))
            keysList.Add(key);
        foreach (var item in langDataDic.Values)
        {
            if (!item.ContainsKey(key))
                item.Add(key, "");
        }

    }
    private void DeleteKey(string key)
    {
        if (keysList.Contains(key))
        {
            keysList.Remove(key);
        }
        foreach (var item in langDataDic.Values)
        {
            if (item.ContainsKey(key))
                item.Remove(key);
        }


    }
    private void CheckKeys()
    {
        foreach (var item in keysList)
        {
            foreach (var langName in langDataDic.Keys)
            {
                if (!langDataDic[langName].ContainsKey(item))
                    langDataDic[langName].Add(item, "");
            }
        }
        List<string> ss = new List<string>(langDataDic.Keys);

        foreach (var langName in langDataDic.Keys)
        {
            foreach (var item in ss)
            {
                if (!langDataDic[langName].ContainsKey(item))
                    langDataDic[langName].Add(item, LocalizedLanguageTool.LanguageNameToLocalizeName(item));
                else if (string.IsNullOrEmpty(langDataDic[langName][item]))
                    langDataDic[langName][item] = LocalizedLanguageTool.LanguageNameToLocalizeName(item);
                if (!keysList.Contains(item))
                    keysList.Add(item);
            }
        }
    }

    private void AddNewLanguage(string langName)
    {
        Dictionary<string, string> content = new Dictionary<string, string>();
        foreach (var item in keysList)
        {
            content.Add(item, "");
        }
        langDataDic.Add(langName, content);
    }
    private void DeleteLanguage(string langName)
    {
        if (langDataDic.ContainsKey(langName))
        {
            langDataDic.Remove(langName);
        }
        FileUtils.DeleteFile(SaveFilePathDir + "/Languages/" + langName + ".txt");
        SaveFile();
    }

    private int toolbarOption = 0;
    private string[] toolbarTexts = { "语言key编辑", "语言内容编辑", "设置" };
   
    void OnGUI()
    {
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));
        switch (toolbarOption)
        {
            case 0:
                LanguageKeyEditGUI();
                break;
            case 1:
                LanguageContentGUI();
                break;
            case 2:
                LanguageSettingEditGUI();
                break;
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("保存"))
        {
            SaveFile();
            GameSettingStoreManager.Save();
            AssetDatabase.Refresh();
        }
       
    }

    string[] chineseLangusge = { "ChineseSimplified", "ChineseTraditional" };
    private void LanguageSettingEditGUI()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Space(3);
        object value = EditorDrawGUIUtil.DrawBaseValue("是否使用系统语言", GameSettingStoreManager.GetValue(LocalizedLanguageManager.GameSettingKeyName_IsUseSystemLanguage, true));
        GameSettingStoreManager.SetValue(LocalizedLanguageManager.GameSettingKeyName_IsUseSystemLanguage, value);
        GUILayout.Space(3);
        List<string> tempss = new List<string>(langDataDic.Keys);
        if (tempss.Count > 0)
        {
            value = EditorDrawGUIUtil.DrawPopup("优先使用的语言：", GameSettingStoreManager.GetValue(LocalizedLanguageManager.GameSettingKeyName_PriorityLanguage, tempss[0]), tempss);
            GameSettingStoreManager.SetValue(LocalizedLanguageManager.GameSettingKeyName_PriorityLanguage, value);
        }
        GUILayout.Space(3);
        GUILayout.Label("当返回语言为Chinese时，优先使用简体还是繁体：");
        List<string> ss = new List<string>(chineseLangusge);
        value = EditorDrawGUIUtil.DrawPopup("中文语言：", GameSettingStoreManager.GetValue(LocalizedLanguageManager.GameSettingKeyName_ChineseLanguagePriorityLanguage, ss[0]), ss);
        GameSettingStoreManager.SetValue(LocalizedLanguageManager.GameSettingKeyName_ChineseLanguagePriorityLanguage, value);
        GUILayout.Space(3);
        GUILayout.EndVertical();
    }

    private int toolbarOption1 = 0;
    private string[] toolbarTexts1= {  "语言内容编辑", "新增语言" };
    private void LanguageContentGUI()
    {
        toolbarOption1 = GUILayout.Toolbar(toolbarOption1, toolbarTexts1, GUILayout.Width(Screen.width-20));
        switch (toolbarOption1)
        {
            case 0:
                LanguageContentEditGUI();
                break;
            case 1:
                AddLanguageContentEditGUI();
                break;

        }
    }
    void SaveFile()
    {
        string ks = JsonUtils.ListToJson(keysList);
        FileUtils.CreateTextFile(SaveFilePathDir + "/LanguageKeys.txt", ks);
        ks = JsonUtils.ListToJson(new List<string>(langDataDic.Keys));
        FileUtils.CreateTextFile(SaveFilePathDir + "/LanguageTypeNames.txt", ks);
        foreach (var item in langDataDic.Keys)
        {
            ks = JsonUtils.DictionaryToJson(langDataDic[item]);
            FileUtils.CreateTextFile(SaveFilePathDir + "/Languages/" + item + ".txt", ks);
        }
        AssetDatabase.Refresh();
    }

    SystemLanguage addNewLanguage;
    string showHelpStr;
    private void AddLanguageContentEditGUI()
    {
        GUILayout.BeginHorizontal();
        addNewLanguage = (SystemLanguage)EditorDrawGUIUtil.DrawBaseValue("添加新语言", addNewLanguage);
        bool canAdd = false;
        if (addNewLanguage == SystemLanguage.Chinese)
            showHelpStr = "不支持添加Chinese, 请添加ChineseSimplified 或 ChineseTraditional";
        else if (addNewLanguage == SystemLanguage.Unknown)
            showHelpStr = "不支持添加 Unknown";
        else if (langDataDic.ContainsKey(addNewLanguage.ToString()))
        {
            showHelpStr = "已添加当前语言";
        }
        else
        {
            showHelpStr = "";
            canAdd = true;
        }
        if (canAdd && GUILayout.Button("添加"))
        {
            AddNewLanguage(addNewLanguage.ToString());
            CheckKeys();
            toolbarOption1 = 0;
           // FileUtils.CreateTextFile(SaveFilePathDir + "/" + addNewLanguage + ".text", "");
        }
        GUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(showHelpStr))
            EditorGUILayout.HelpBox(showHelpStr, MessageType.Error);
    }

    private Vector2 pos = Vector2.zero;
    private void LanguageKeyEditGUI()
    {
      pos =  GUILayout.BeginScrollView(pos);
        keysList = EditorDrawGUIUtil.DrawList("语言key", keysList,()=>
        {
            string ss = "key" + keysList.Count;
            while (keysList.Contains(ss))
            {
                ss += "1";
            }
            return ss;
        },
        (isAdd, item) =>
         {
             if(isAdd)
             AddKey(item.ToString());
             else
                 DeleteKey(item.ToString());
         }) as List<string>;

        GUILayout.EndScrollView();
    }

    private string selectSystemLanguage;
    private void LanguageContentEditGUI()
    {
        List<string> tempss = new List<string>(langDataDic.Keys);
        GUILayout.BeginHorizontal();
        selectSystemLanguage = EditorDrawGUIUtil.DrawPopup("语言：", selectSystemLanguage, tempss);
        if (GUILayout.Button("删除"))
        {
            if (EditorUtility.DisplayDialog("警告", "是否删除语言文件"+selectSystemLanguage, "是", "取消"))
            {
                DeleteLanguage(selectSystemLanguage);
                return;
            }
        }
        GUILayout.EndHorizontal();
        if (string.IsNullOrEmpty(selectSystemLanguage))
            return;
        Dictionary<string, string> content = langDataDic[selectSystemLanguage];
        List<string> ks = new List<string>(content.Keys);
        pos = GUILayout.BeginScrollView(pos);
        foreach (var item in ks)
        {
            //Debug.Log(item + "==" + content[item]);
            content[item] = EditorDrawGUIUtil.DrawBaseValue(item, content[item]).ToString();
        }
        GUILayout.EndScrollView();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HDJ.Framework.Core.ECS;
using HDJ.Framework.Modules;
using HDJ.Framework.Utils;
using System;

public class ECSSettingEidtorWindow : EditorWindow {

    [MenuItem("Tool/ECS 世界设置")]
	static void OpenWindow()
    {
        GetWindow<ECSSettingEidtorWindow>();
    }

    private const string PathDic = "Assets/Resources/Configs/FunctionSettingConfig/";
    private const string SettingFileName = "WorldSettingConfig";

    private const string RealWorldName = "RealWorld";
    private WorldManagerSettingData worldSettingData;
    private List<string> systemNames = new List<string>();
    private void OnEnable()
    {
        string data = ResourcesManager.LoadTextFileByName(SettingFileName);

        if (!string.IsNullOrEmpty(data))
        {
             worldSettingData = JsonUtils.JsonToClassOrStruct<WorldManagerSettingData>(data);

        }
        if (worldSettingData == null)
            worldSettingData = new WorldManagerSettingData();

        systemNames.Clear();
        Type[] types = ReflectionUtils.GetChildTypes(typeof(ISystem));
        foreach (var item in types)
        {
            systemNames.Add(item.FullName);
        }

        AddNewWorldSettingData(RealWorldName);
       

    }
    private int toolbarOption = 0;
    private string[] toolbarTexts = { "World Setting", "World System Setting" };
    private Vector2 pos;
    private void OnGUI()
    {
        if (worldSettingData == null)
            return;
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));
        switch (toolbarOption)
        {
            case 0:
                WorldSettingGUI();
                break;
            case 1:
                WorldSystemSettingGUI();
                break;
        }
       
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("保存"))
        {
            string path = PathDic + SettingFileName + ".txt";
            string data = JsonUtils.ClassOrStructToJson(worldSettingData);

            FileUtils.CreateTextFile(path, data);
            AssetDatabase.Refresh();
        }
    }

    private void WorldSettingGUI()
    {
        GUILayout.Label("配置：");
        GUILayout.Space(3);
        worldSettingData.fixedUpdateDeltaTime = (int)EditorDrawGUIUtil.DrawBaseValue("固定更新时间：", worldSettingData.fixedUpdateDeltaTime);

        List<string> allWorldNames = worldSettingData.GetAllWorldNames();
        worldSettingData.defaultFirstRunWorldName = EditorDrawGUIUtil.DrawPopup("启动默认运行的世界：", worldSettingData.defaultFirstRunWorldName, allWorldNames);

        pos = GUILayout.BeginScrollView(pos, "box");
        GUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal("box");
        GUILayout.Box("All World Data");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", GUILayout.Width(50)))
        {
            string tempName = "New World";
            while (!AddNewWorldSettingData(tempName))
            {
                tempName += "_0";
            }
        }
        GUILayout.EndHorizontal();

        foreach (var item in worldSettingData.allWorldSettingData)
        {
            GUILayout.BeginVertical("box");
            DrawWorldSettingGUI(item);
            GUILayout.Space(4);

            if (GUILayout.Button("Delete"))
            {
                worldSettingData.allWorldSettingData.Remove(item);
                return;
            }
            GUILayout.Space(4);
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();


        GUILayout.EndScrollView();
    }

    private string selectWorldName;
    private void WorldSystemSettingGUI()
    {
        GUILayout.Space(5);

        List<string> allWorldNames = worldSettingData.GetAllWorldNames();
        selectWorldName = EditorDrawGUIUtil.DrawPopup("Select World ：", selectWorldName, allWorldNames);

        WorldSettingData settingData = worldSettingData.GetWorldSettingData(selectWorldName);

        List<SystemsSettingData> ssd = new List<SystemsSettingData>(settingData.allSystemSettingDatas.Values);
        GUILayout.Space(5);
        pos = GUILayout.BeginScrollView(pos, "box");
        foreach (var item in ssd)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("System Name ：" + item.systemName);
            item.delayExecute = (int)EditorDrawGUIUtil.DrawBaseValue("每次Update间隔时间(毫秒)：", item.delayExecute);
            GUILayout.Space(8);
            GUILayout.EndVertical();
        }

        GUILayout.EndScrollView();
    }

    private bool AddNewWorldSettingData(string name)
    {
        if (worldSettingData.GetWorldSettingData(name) == null)
        {
            WorldSettingData w = new WorldSettingData();
            w.worldName = name;
            w.useSystemList.AddRange(systemNames);
            worldSettingData.allWorldSettingData.Add(w);

            foreach (var item in systemNames)
            {
                SystemsSettingData settingData = new SystemsSettingData();
                settingData.systemName = item;
                settingData.delayExecute = 0;
                w.allSystemSettingDatas.Add(item, settingData);
            }
            return true;
        }

        return false;
    }

    private Dictionary<int, bool> foldStateDic = new Dictionary<int, bool>();
    private void DrawWorldSettingGUI(WorldSettingData worldSetting)
    {
        int hash =worldSetting.GetHashCode();
        bool isFold = true;
        if (foldStateDic.ContainsKey(hash))
        {
            isFold = foldStateDic[hash];
        }
        else
        {
            foldStateDic.Add(hash, isFold);
        }
        isFold = EditorGUILayout.Foldout(isFold, "World Setting");
        if (isFold)
        {
           
            if(worldSetting.worldName == RealWorldName)
            {
                GUILayout.Label("World Name: " + worldSetting.worldName);
            }
            else
            {
                string temp = EditorDrawGUIUtil.DrawBaseValue("World Name: ", worldSetting.worldName).ToString();
                while (worldSettingData.IsHaveRepeatName(worldSetting))
                {
                    temp += "_0";
                }
                worldSetting.worldName = temp;
            }
            GUILayout.BeginVertical("box");
            GUILayout.Box("Use System:");
            int LineNumber = 3;
            for (int i = 0; i < systemNames.Count; )
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                for (int j = 0; j < LineNumber; j++)
                {
                    
                    string sName = systemNames[i];
                    if (worldSetting.useSystemList.Contains(sName))
                        GUI.color = Color.green;
                    if (GUILayout.Button(sName, GUILayout.MaxWidth(Screen.width/LineNumber)))
                    {
                        if (worldSetting.useSystemList.Contains(sName))
                        {
                            worldSetting.useSystemList.Remove(sName);
                        }
                        else
                        {
                            worldSetting.useSystemList.Add(sName);
                        }
                    }
                    GUI.color = Color.white;
                    i++;
                    if (i >= systemNames.Count)
                        break;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();

        }

         foldStateDic[hash] = isFold;

    }
}

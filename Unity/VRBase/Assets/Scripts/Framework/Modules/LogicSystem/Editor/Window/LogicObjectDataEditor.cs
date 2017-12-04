﻿using HDJ.Framework.Modules;
using HDJ.Framework.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    public class LogicObjectDataEditor : EditorWindow
{
    [MenuItem("Tool/逻辑文件管理",priority = 1201)]
	public static void OpenWindow()
    {
      LogicObjectDataEditor win = EditorWindow.GetWindow<LogicObjectDataEditor>();
      win.Init();
    }
    Dictionary<string, List<string>> dataNameDic = new Dictionary<string, List<string>>();
    MessageStringData ms;
    void Init()
    {
        dataNameDic.Clear();

         ms = LogicSystemEditorTools.GetMessageStringData();
        ms.hideFlags = HideFlags.NotEditable;
        for (int i = 0; i < ms.mesList.Count; i++)
        {
            MessageString m = ms.mesList[i];
            string pathDic = PathUtils.GetSpecialPath(m.value, SpecialPathType.Resources);
            if (!Directory.Exists(pathDic))
            {
                Directory.CreateDirectory(pathDic);
                AssetDatabase.Refresh();
            }
            string[] temp = PathUtils.GetDirectoryFileNames(pathDic, new string[] { ".txt" });
            Debug.Log("Name:" + m.name);
            dataNameDic.Add(m.name, new List<string>(temp));
        }
    }

    private void OnEnable()
    {
        Init();
    }

    public static int toolbarOption = 0;
    static public string[] toolbarTexts = { "所有Logic文件", "新建Logic Type" };
    private Color oldColor;
    void OnGUI()
    {
        if (ms == null) return;
        oldColor = GUI.color;

        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts);
        switch (toolbarOption)
        {
            case 0:
                DrawAllFileGUI();
                break;
            case 1:
                AddNewTypeGUI();

                break;

        }

    }
    private string addName = "";
    private string addPath = "";
    private const string DicName = "LogicObjects";
    private void AddNewTypeGUI()
    {
        GUILayout.BeginHorizontal("box");
        addName = EditorDrawGUIUtil.DrawBaseValue("添加类型名:", addName).ToString();
        addPath = DicName + "/" + addName + "/";

        GUILayout.EndHorizontal();
        if (!string.IsNullOrEmpty(addName))
        {
            GUILayout.Label("路径: Assets/Resources/" + addPath);
        }
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("添加", GUILayout.Width(60)))
        {
            if (dataNameDic.ContainsKey(addName) || string.IsNullOrEmpty(addName))
            {
                EditorUtility.DisplayDialog("错误", "类型名字不能为空或重复", "OK");
                return;
            }
            ms.SetValue(addName, addPath);
            EditorUtility.SetDirty(ms);
            Init();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("设置所有类型", GUILayout.Width(110)))
        {
            Selection.activeObject = ms;
        }
    }

    private string selectName = "";

    private bool isNewOne = false;
    private string newName = "";

    private Vector2 scrollViewPos = Vector2.zero;
    private string editFileName = "";
        void DrawAllFileGUI()
        {
            List<string> nameList = new List<string>(dataNameDic.Keys);
            if (nameList.Count == 0) return;

            GUILayout.BeginHorizontal();
            selectName = EditorDrawGUIUtil.DrawPopup("类型:", selectName, nameList);
            //添加新类型
            if (!isNewOne && GUILayout.Button("+", GUILayout.Width(30)))
            {
                isNewOne = !isNewOne;
            }
            if (isNewOne)
            {
                newName = EditorDrawGUIUtil.DrawBaseValue("新建名字:", newName).ToString();
                if (GUILayout.Button("确定", GUILayout.Width(60)))
                {
                    if (string.IsNullOrEmpty(newName) || ResourcePathManager.ContainsFileName(newName) || dataNameDic[selectName].Contains(newName))
                    {
                        EditorUtility.DisplayDialog("错误", "名字不能重复或为空", "OK");
                        return;
                    }
                    dataNameDic[selectName].Add(newName);
                    isNewOne = false;
                }
                if (GUILayout.Button("取消", GUILayout.Width(60)))
                {
                    isNewOne = false;
                }
            }
            //删除该类型
            if (!isNewOne && GUILayout.Button("-", GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("警告", "是否删除类型：" + selectName, "OK", "Cancel"))
                {
                    ms.RemoveItem(selectName);
                    Init();
                    return;
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            scrollViewPos = GUILayout.BeginScrollView(scrollViewPos, true, true);

            foreach (string name in dataNameDic[selectName])
            {
                if (name == editFileName)
                {
                    GUI.color = Color.red;
                }
                GUILayout.BeginVertical("box");
                GUILayout.BeginHorizontal();
                GUILayout.Label("文件名：" + name);
                if (GUILayout.Button("编辑", GUILayout.Width(60)))
                {
                    LogicSystemEditorWindow.ShowWindow(name, selectName);
                    editFileName = name;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(9);
                if (GUILayout.Button("移除", GUILayout.Width(70)))
                {
                    if (EditorUtility.DisplayDialog("警告", "是否确定删除文件" + name, "OK", "Cancel"))
                    {
                        Debug.Log(name);
                        ResourcePathManager.DeleteResouceFile(name);
                        dataNameDic[selectName].Remove(name);
                    }

                    return;
                }
                GUILayout.EndVertical();

                GUI.color = oldColor;
            }

            GUILayout.EndScrollView();
        }
    }
}

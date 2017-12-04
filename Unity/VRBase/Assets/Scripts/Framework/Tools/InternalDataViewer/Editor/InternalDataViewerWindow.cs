using HDJ.Framework.Modules;
using HDJ.Framework.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 内部数据查看器
/// </summary>
public class InternalDataViewerWindow : EditorWindow {

    [MenuItem("Tool/内部数据查看器", priority = 100)]
    private static void OpenWindow()
    {
        GetWindow<InternalDataViewerWindow>();
    }



    string selectItem = "";
    private Vector2 pos0;

    private void OnGUI()
    {


        List<string> names = new List<string>(InternalDataManager.DataDictionary.Keys);
        selectItem = EditorDrawGUIUtil.DrawPopup("内部数据：", selectItem, names);

        if (string.IsNullOrEmpty(selectItem)) return;

        object obj = InternalDataManager.DataDictionary[selectItem];
        pos0 = GUILayout.BeginScrollView(pos0);

        obj = EditorDrawGUIUtil.DrawBaseValue(obj.GetHashCode().ToString(), obj,  EditorUIOpenState.OpenFirstFold);

        GUILayout.EndScrollView();
        if(GUILayout.Button("Clear"))
        {
            InternalDataManager.Clear();
        }

        if (GUILayout.Button("Log"))
        {
            ULog.Log("Test log");
        }
    }
}

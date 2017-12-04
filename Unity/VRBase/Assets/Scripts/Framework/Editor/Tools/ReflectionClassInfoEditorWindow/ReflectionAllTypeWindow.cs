using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ReflectionAllTypeWindow : EditorWindow
{

    [MenuItem("Tool/工具/反射查看所有Type")]
    static void OpenWindow()
    {
        GetWindow<ReflectionAllTypeWindow>().Init();
    }

   
    Assembly[] assemblys;
    private void Init()
    {
       
         assemblys= AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblys.Length; i++)
        {
            Assembly ass = assemblys[i];
            string[] sArr = ass.FullName.Split(',');
            if (sArr.Length > 0)
            {
                allAssemblyName.Add(sArr[0]);
            }
            else
                allAssemblyName.Add(ass.FullName);
        }
    }

    void CreateFullTypeNameToTree(Assembly ass)
    {
        Dictionary<string, Dictionary<string, object>> alldata = new Dictionary<string, Dictionary<string, object>>();
        Type[] types = ass.GetTypes();
        for (int j = 0; j < types.Length; j++)
        {
            Dictionary<string, object> tempData = new Dictionary<string, object>();
            string typeName = types[j].FullName;
            tempData.Add("AssemblyFullName", ass.FullName);
            tempData.Add("TypeFullName", typeName);
            tempData.Add("Type", types[j]);
            string path = typeName.Replace(".", "/");
            alldata.Add(path, tempData);
        }
        string[] paths = new List<string>(alldata.Keys).ToArray();
        control = EditorDrawFileDirectory.GetFileDirectoryInfo(paths, false, "cs Script Icon", (node) =>
        {
            if (alldata.ContainsKey(node.relativeRootPath))
            {
                node.otherData = alldata[node.relativeRootPath];
            }
            else
            {
                node.isDirectory = true;
            }

            if (!node.otherData.ContainsKey("Type") || node.isDirectory)
            {
                node.content = EditorDrawFileDirectory.GetPathGUIContent(node.relativeRootPath, "Folder Icon");
            }
        });
    }
    private Vector2 pos = Vector2.zero;


    private string selectAss = "";
    List<string> allAssemblyName = new List<string>();
    void OnGUI()
    {
        GUILayout.Space(6);
        selectAss = EditorDrawGUIUtil.DrawPopup("程序集：", selectAss, allAssemblyName, PopupSelectChange);

        pos = GUILayout.BeginScrollView(pos, "box");

        if (control == null)
            return;
        EditorDrawFileDirectory.DrawFileDirectory(control, ShowFileDirectoryType.ShowAllFile, null, SelectCallBack);

        GUILayout.EndScrollView();

        if(CanShowDetill && GUILayout.Button("查看详情"))
        {
            ReflectionClassInfoEditorWindow.AddType((Type)selectItem.otherData["Type"]);
            FocusWindowIfItsOpen<ReflectionClassInfoEditorWindow>();
        }

    }
    private void PopupSelectChange(string selec)
    {
        int index = allAssemblyName.IndexOf(selec);
        CreateFullTypeNameToTree(assemblys[index]);
    }
    private bool CanShowDetill = false;
    private FileData selectItem = null;
    private void SelectCallBack(FileData t)
    {
        if (!t.isDirectory&& t.otherData.ContainsKey("Type"))
        {
            CanShowDetill = true;
            selectItem = t;
        }
        else
        {
            t.isSelected = !t.isSelected;
            CanShowDetill = false;
        }
       
       
    }

    TreeModelController<FileData> control;
}

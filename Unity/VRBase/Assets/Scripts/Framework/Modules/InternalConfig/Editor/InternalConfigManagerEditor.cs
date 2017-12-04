using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using HDJ.Framework.Utils;
using HDJ.Framework.Modules;

public class InternalConfigManagerEditor : EditorWindow
{
    public static InternalConfigManagerEditor instance = null;

    [MenuItem("Tool/内部配置文件",priority = 1100)]
    public static void OpenWindow()
    {
      instance = EditorWindow.GetWindow<InternalConfigManagerEditor>();
      instance.Show();
      instance.Initialize();
    }

    string pathDic ="";
    List<string> fileNames = new List<string>();
    private void Initialize()
    {
        fileNames.Clear();
        pathDic = Application.dataPath + "/Resources/Configs/InternalConfigs";
        string[] temp = PathUtils.GetDirectoryFileNames(pathDic, new string[] { ".txt" });
        
        fileNames.AddRange(temp);
        CreateValueTypeName();
    }
    void OnEnable()
    {
        Initialize();
    }
    private int toolbarOption = 0;
    private string[] toolbarTexts = { "新建配置文件", "添加内容"};
    string newFileName="";
    private void OnGUI()
    {
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts);
        switch (toolbarOption)
        {
            case 0:
                GUILayout.BeginHorizontal("box");
                newFileName = EditorDrawGUIUtil.DrawBaseValue("新建配置文件", newFileName).ToString();
                if (GUILayout.Button("确定", GUILayout.Width(50)))
                {
                    if (ResourcePathManager.ContainsFileName(newFileName))
                    {
                        EditorUtility.DisplayDialog("错误", "文件名与其他文件重复", "OK");
                        return;
                    }
                    InternalConfigManager.SaveData(pathDic + "/" + newFileName + ".txt", new Dictionary<string, object>());
                    AssetDatabase.Refresh();
                    Initialize();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                break;
            case 1:
                AddNewValueGUI();
                break;

        }
        
        ChooseFile();

        if (GUILayout.Button("保存"))
        {
            InternalConfigManager.SaveData(pathDic +"/"+ chooseFileName + ".txt", fileContent);
            AssetDatabase.Refresh(); 
        }

    }
    private string chooseFileName = "";
    private string lastChooseFileName = "";
    private Dictionary<string, object> fileContent = new Dictionary<string, object>();
    private Vector2 scrollViewPos = new Vector2(0, 0);
    void ChooseFile()
    {
        GUILayout.BeginHorizontal();
        lastChooseFileName = chooseFileName;
        chooseFileName = EditorDrawGUIUtil.DrawPopup("选择文件", chooseFileName, fileNames);
        if (GUILayout.Button("删除", GUILayout.Width(50)))
        {
            if (EditorUtility.DisplayDialog("警告", "是否删除文件[" + chooseFileName + "]", "确定", "取消"))
            {
                fileNames.Remove(chooseFileName);
                FileUtils.DeleteFile(pathDic + "/" + chooseFileName + ".txt");
                AssetDatabase.Refresh();
                return;
            }
        }
        if (lastChooseFileName != chooseFileName)
        {
            if (string.IsNullOrEmpty(chooseFileName))
                return;
            fileContent = InternalConfigManager.GetConfig(chooseFileName);

        }
        GUILayout.EndHorizontal();

        GUILayout.Space(8);

        List<string> keys = new List<string>(fileContent.Keys);
       scrollViewPos= GUILayout.BeginScrollView(scrollViewPos, "box");
        foreach (string key in keys)
        {
            GUILayout.BeginHorizontal("box");
            fileContent[key] = EditorDrawGUIUtil.DrawBaseValue(key, fileContent[key]);
            if (GUILayout.Button("-", GUILayout.Width(50)))
            {
                fileContent.Remove(key);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(8);
        }
        GUILayout.EndScrollView();
    }

    private List<string> valueTypeList = new List<string>();
    private string valueName = "";
    private string enumTypeName = "";
    private string valueTypeChoose = "";
    void AddNewValueGUI()
    {
        GUILayout.Label("添加内容：");
        GUILayout.BeginHorizontal("box");
        GUILayout.BeginVertical();
        valueTypeChoose = EditorDrawGUIUtil.DrawPopup("添加", valueTypeChoose, valueTypeList);
        valueName = EditorDrawGUIUtil.DrawBaseValue("名字", valueName).ToString();
        if (valueTypeChoose == "Enum")
        {
            enumTypeName = EditorDrawGUIUtil.DrawBaseValue("枚举类型", enumTypeName).ToString();
        }
        GUILayout.EndVertical();
        if (GUILayout.Button("确定", GUILayout.Width(50)))
        {
            if (string.IsNullOrEmpty(valueName) || fileContent.ContainsKey(valueName))
            {
                EditorUtility.DisplayDialog("错误", "名字不能为空或重复！", "ok");
                return;
            }
            AddValue();
        }
       
        GUILayout.EndHorizontal();
    }

    void CreateValueTypeName()
    {
        valueTypeList.Clear();
        valueTypeList.Add(typeof(string).FullName);
        valueTypeList.Add(typeof(int).FullName);
        valueTypeList.Add(typeof(float).FullName);
        valueTypeList.Add(typeof(long).FullName);
        valueTypeList.Add(typeof(double).FullName);
        valueTypeList.Add(typeof(short).FullName);
        valueTypeList.Add(typeof(bool).FullName);
        valueTypeList.Add(typeof(Vector2).FullName);
        valueTypeList.Add(typeof(Vector3).FullName);
        valueTypeList.Add("Enum");
        valueTypeList.Add("List<string>");
        valueTypeList.Add("List<int>");
        valueTypeList.Add("List<float>");
        valueTypeList.Add("List<long>");
        valueTypeList.Add("List<double>");
        valueTypeList.Add("List<short>");
    }

    void AddValue()
    {
        object obj = null;
        try
        {
            if (valueTypeChoose == "Enum")
            {              
                Type type = ReflectionUtils.GetTypeByTypeFullName(enumTypeName.Trim());
                string temp = Enum.GetName(type, 0);
                obj = Enum.Parse(type, temp);
            }
            else if (valueTypeChoose == "List<string>")
            {
                obj = new List<string>();
            }
            else if (valueTypeChoose == "List<int>")
            {
                obj = new List<int>();
            }
            else if (valueTypeChoose == "List<float>")
            {
                obj = new List<float>();
            }
            else if (valueTypeChoose == "List<long>")
            {
                obj = new List<long>();
            }
            else if (valueTypeChoose == "List<double>")
            {
                obj = new List<double>();
            }
            else if (valueTypeChoose == "List<short>")
            {
                obj = new List<short>();
            }
            else if (valueTypeChoose == typeof(Vector2).FullName)
            {
                obj = new Vector2();
            }
            else if (valueTypeChoose == typeof(Vector3).FullName)
            {
                obj = new Vector3();
            }
            else if (valueTypeChoose == "List<short>")
            {
                obj = new List<short>();
            }
            else if (valueTypeChoose == typeof(string).FullName)
            {
                obj = "";
            }
            else
            {
                obj = Activator.CreateInstance(Type.GetType(valueTypeChoose));
            }

            fileContent.Add(valueName, obj);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("错误", e.ToString(), "ok");
        }
        
    }
}

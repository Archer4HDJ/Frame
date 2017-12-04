using HDJ.Framework.Modules;
using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

public class ConfigDataEditorWindow : EditorWindow
{
    public const string configLayerName = "TableConfig";

    [InitializeOnLoadMethod]
    static void AddLayerToAssetsSort()
    {
        if (AssetsSortManagement.AddLayer(configLayerName))
            AssetsSortManagement.Save();
    }

    [MenuItem("Tool/Table配置文件/编辑配置文件", priority =1101)]
    static void OpenWindow()
    {
        ConfigDataEditorWindow ce = EditorWindow.GetWindow<ConfigDataEditorWindow>();
        ce.Init();
    }

    List<string> fileNames = new List<string>();
    void Init()
    {
        fileNames.Clear();
        string[] temp = AssetsSortManagement.GetNamesByLayer(configLayerName);
        fileNames.AddRange(temp);
    }
    void OnEnable()
    {
        Init();
    }

    private int toolbarOption0 = 0;
    private string[] toolbarTexts0 = { "数据表", "新建配置文件" };
    private int toolbarOption = 0;
    private string[] toolbarTexts = { "修改数据", "修改字段" };
    void OnGUI()
    {
        toolbarOption0 = GUILayout.Toolbar(toolbarOption0, toolbarTexts0, GUILayout.Width(Screen.width));
        switch (toolbarOption0)
        {
            case 0:
                GUILayout.Space(8);
                ChooseFile();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width*3/4f));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                
                switch (toolbarOption)
                {
                    case 0:
                        DrawTableDataGUI();

                        break;
                    case 1:
                        DrawFields();
                        break;
                }
              
                GUILayout.FlexibleSpace();
                GUILayout.Space(5);
                if (GUILayout.Button("保存配置文件"))
                {
                    CheckEditConfigData();
                    TableConfigOtherInfo infos = TableConfigBase.tableConfigOtherInfo[chooseFileName];
                    string content = TableConfigTool.ConfigInfo2TableText(infos, currentEditConfigData);
                    string path = CreateConfigClassEditor.SaveConfigFilePath + "/" + chooseFileName + ".txt";
                    FileUtils.CreateTextFile(path, content);
                    CreateConfigClassEditor.CreateConfigClassFile(content, chooseFileName);
                    AssetDatabase.Refresh();
                }
                break;
            case 1:
                GUILayout.Space(5);
                NewTableConfig();
                break;

        }
      
    }

    private string chooseFileName = "";

    private Vector2 scrollViewPos = new Vector2(0, 0);
    void ChooseFile()
    {
        GUILayout.BeginHorizontal();
        chooseFileName = EditorDrawGUIUtil.DrawPopup("选择文件", chooseFileName, fileNames, LoadData);
        if (GUILayout.Button("删除", GUILayout.Width(50)))
        {
            if (EditorUtility.DisplayDialog("警告", "是否删除文件[" + chooseFileName + "]", "确定", "取消"))
            {
                fileNames.Remove(chooseFileName);
                FileUtils.DeleteFile(CreateConfigClassEditor.SaveConfigFilePath + "/" + chooseFileName + ".txt");
                string csPath = Application.dataPath + "/Scripts/ConfigClass/" + chooseFileName + ".cs";
                FileUtils.DeleteFile(csPath);
                AssetDatabase.Refresh();
                return;
            }
        }
      
        GUILayout.EndHorizontal();
    }
    private ConfigFileContents currentEditConfigData;
    TableConfigOtherInfo tableConfigOtherInfo;
    private void LoadData(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;
        Type t =ReflectionUtils.GetTypeByTypeFullName (fileName);
        List<TableConfigBase> listdata = TableConfigManager.GetConfig(t);
        currentEditConfigData = new ConfigFileContents();
        foreach (var item in listdata)
        {
          Type type =  item.GetType();
            FieldInfo[] fInfos= type.GetFields();
            List<ConfigRowData> hang = new List<ConfigRowData>();
            foreach (var f in fInfos)
            {
              object value =  f.GetValue(item);
                ConfigRowData co = new ConfigRowData();
                co.fieldName = f.Name;
                co.value = value;
                hang.Add(co);
               
            }
            currentEditConfigData.configRowDataList.Add(hang);
        }
        TableConfigOtherInfo info= TableConfigBase.tableConfigOtherInfo[fileName];
        tableConfigOtherInfo = new TableConfigOtherInfo();
        tableConfigOtherInfo.configDescription = info.configDescription;
        tableConfigOtherInfo.fieldInfoDic = new Dictionary<string, TableConfigFieldInfo>();
        foreach (var item in info.fieldInfoDic)
        {
            TableConfigFieldInfo f = new TableConfigFieldInfo();
            f.fieldName = item.Value.fieldName;
            f.description = item.Value.description;
            f.defultValue = item.Value.defultValue;
            f.fieldValueType = item.Value.fieldValueType;
            tableConfigOtherInfo.fieldInfoDic.Add(item.Key, f);
        }
    }
    private float ItemWith = 80;
    private float ItemHeight = 20;
    private int index = 0;
    private int temp;
    private Vector2 pos0 = Vector2.zero;
    void DrawTableDataGUI()
    {
        if (currentEditConfigData == null )
            return;
        TableConfigOtherInfo infos = TableConfigBase.tableConfigOtherInfo[chooseFileName];

        GUILayout.Space(5);
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", GUILayout.Width(50)))
        {
            currentEditConfigData.AddLine(infos);
            index = currentEditConfigData.configRowDataList.Count - 1;
        }
        if (GUILayout.Button("-", GUILayout.Width(50)))
        {
            if(EditorUtility.DisplayDialog("警告", "要删除当前页数据？", "OK", "Cancel"))
            {
                currentEditConfigData.configRowDataList.RemoveAt(index);
                return;
            }
           
        }
        GUILayout.EndHorizontal();
        if (currentEditConfigData.configRowDataList.Count == 0)
            return;

        GUILayout.Space(5);
      //  Debug.Log("Index: " + index + "  count : " + currentEditConfigData.configRowDataList.Count);
        List<ConfigRowData> datas = currentEditConfigData.configRowDataList[index];

        //确保字段定义和数据一致
        CheckEditConfigData();


        pos0 = GUILayout.BeginScrollView(pos0, "CurveEditorBackground");
        for (int i = 0; i < datas.Count; i++)
        {
            GUILayout.BeginVertical("U2D.createRect");
            ConfigRowData d = datas[i];
            EditorGUILayout.HelpBox("字段描述：" + infos.fieldInfoDic[d.fieldName].description, MessageType.Info,true);
            d.value = EditorDrawGUIUtil.DrawBaseValue(d.fieldName, d.value);
            GUILayout.EndVertical();
            GUILayout.Space(8);
            
        }
        GUILayout.EndScrollView();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal("box");
        int count = currentEditConfigData.configRowDataList.Count;
        if (GUILayout.Button("上一页", GUILayout.Width(ItemWith), GUILayout.Height(ItemHeight)))
        {
            if (index <= 0)
                index = count - 1;
            else
                index--;
        }
        GUILayout.Label("共" + count + "页,当前" + (index + 1) + "页");
        temp = EditorGUILayout.IntField("跳转:", temp);
        if (GUILayout.Button("跳转", GUILayout.Width(ItemWith), GUILayout.Height(ItemHeight)))
        {
            if ((temp - 1) <= 0)
                index = count - 1;
            else if (temp >= count)
                index = count - 1;
            else
                index = temp - 1;
            temp = index + 1;
        }
        if (GUILayout.Button("下一页", GUILayout.Width(ItemWith), GUILayout.Height(ItemHeight)))
        {
            if (index >= count - 1)
                index = 0;
            else
                index++;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

    }

    private void CheckEditConfigData()
    {
        TableConfigOtherInfo infos = TableConfigBase.tableConfigOtherInfo[chooseFileName];
        List<ConfigRowData> datas = currentEditConfigData.configRowDataList[index];
        List<string> fs = new List<string>(infos.fieldInfoDic.Keys);
        foreach (var item in fs)
        {
            if (!currentEditConfigData.IsHaveRow(item))
            {
                currentEditConfigData.AddRow(item, infos.fieldInfoDic[item].defultValue);
                return;
            }
        }
      
        for (int i = 0; i < datas.Count; i++)
        {
            ConfigRowData d = datas[i];
            if (!infos.fieldInfoDic.ContainsKey(d.fieldName))
            {
                currentEditConfigData.RemoveFieldValue(d.fieldName);
                break;
            }
            TableConfigFieldInfo field = infos.fieldInfoDic[d.fieldName];
            Type tempType = TableConfigTool.ConfigFieldValueType2Type(field.fieldValueType);
            if (d.value == null || tempType.FullName != d.value.GetType().FullName)
            {
                d.value = field.defultValue;
                break;
            }
        }
    }

    string newFileName = "";
    private TableConfigOtherInfo newConfigInfo;
    private void NewTableConfig()
    {
        GUILayout.BeginHorizontal("box");
        newFileName = EditorDrawGUIUtil.DrawBaseValue("新建配置文件:", newFileName).ToString();
        if (GUILayout.Button("确定", GUILayout.Width(50)))
        {
            if (ResourcePathManager.ContainsFileName(newFileName) || string.IsNullOrEmpty(newFileName))
            {
                EditorUtility.DisplayDialog("警告", "名字不能为空或重复", "OK");
            }
            else
            {
                chooseFileName = newFileName;
                fileNames.Add(newFileName);
            }
        }
        
            GUILayout.EndHorizontal();

        if ( string.IsNullOrEmpty(newFileName))
            return;
        if (ResourcePathManager.ContainsFileName(newFileName))
        {
            EditorGUILayout.HelpBox("文件名重复！！", MessageType.Error);
            return;
        }
        if(newConfigInfo == null)
        {
            newConfigInfo = new TableConfigOtherInfo();
        }

        newConfigInfo.configDescription = EditorDrawGUIUtil.DrawBaseValue("配置文件描述：", newConfigInfo.configDescription).ToString();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal("box");
        GUILayout.Box("添加字段：");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", GUILayout.Width(50)))
        {
            TableConfigFieldInfo f = new TableConfigFieldInfo();
            f.fieldName = "NewField" + newConfigInfo.fieldInfoDic.Count;
            while (newConfigInfo.fieldInfoDic.ContainsKey(f.fieldName))
            {
                f.fieldName += "_0";
            }
            newConfigInfo.fieldInfoDic.Add(f.fieldName, f);
        }
        GUILayout.EndHorizontal();

        DrawFieldData(newConfigInfo);

        if (newConfigInfo.fieldInfoDic.Count>0 && GUILayout.Button("创建"))
        {
            GlobalEvent.AddEvent(EditorGlobalEventEnum.OnResourcesAssetsChange, (t) =>
             {
                 Debug.Log("OnResourcesAssetsChange");
                 AssetsSortManagement.SetLayer(newFileName, configLayerName);
                 AssetsSortManagement.Save();

                 newFileName = "";
                 newConfigInfo = null;
             }, true);
           string content= TableConfigTool.ConfigInfo2TableText(newConfigInfo, null);
            string path = CreateConfigClassEditor.SaveConfigFilePath + "/" + newFileName + ".txt";
            FileUtils.CreateTextFile(path, content);
            CreateConfigClassEditor.CreateConfigClassFile(content, newFileName);
            AssetDatabase.Refresh();
        }
    }
    private void DrawFieldData(TableConfigOtherInfo info)
    {
        pos1 = GUILayout.BeginScrollView(pos1, "CurveEditorBackground");
        List<TableConfigFieldInfo> list = new List<TableConfigFieldInfo>(info.fieldInfoDic.Values);

        for (int i = 0; i < list.Count; i++)
        {
            TableConfigFieldInfo temp = list[i];
            temp = (TableConfigFieldInfo)EditorDrawGUIUtil.DrawBaseValue("◆", temp);
            Type tempType = TableConfigTool.ConfigFieldValueType2Type(temp.fieldValueType);
            if (temp.defultValue == null || tempType.FullName != temp.defultValue.GetType().FullName)
            {
                temp.defultValue = ReflectionUtils.CreateDefultInstance(tempType);
            }
            GUILayout.Space(3);
            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("警告", "是否删除字段", "OK", "Cancel"))
                {
                   // info.fieldInfoDic.Remove(temp.fieldName);
                    list.Remove(temp);
                }
            }

            GUILayout.Space(6);
        }
        GUILayout.EndScrollView();
        info.fieldInfoDic.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            TableConfigFieldInfo temp = list[i];
            info.fieldInfoDic.Add(temp.fieldName, temp);
        }
        }
        
    private Vector2 pos1;
    private void DrawFields()
    {
        if (string.IsNullOrEmpty(chooseFileName)) return;
        if (!TableConfigBase.tableConfigOtherInfo.ContainsKey(chooseFileName)) return;

        TableConfigOtherInfo infos = TableConfigBase.tableConfigOtherInfo[chooseFileName];

        infos.configDescription =EditorDrawGUIUtil.DrawBaseValue("配置文件描述", infos.configDescription).ToString();
        GUILayout.BeginHorizontal("box");
        GUILayout.Box("字段：");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", GUILayout.Width(50)))
        {
            TableConfigFieldInfo f = new TableConfigFieldInfo();
            f.fieldName = "NewField" + infos.fieldInfoDic.Count;
            while (infos.fieldInfoDic.ContainsKey(f.fieldName))
            {
                f.fieldName += "_0";
            }
            infos.fieldInfoDic.Add(f.fieldName, f);
        }
        GUILayout.EndHorizontal();
        DrawFieldData(infos);
    }
}



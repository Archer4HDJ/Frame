using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetsSortEidtorWindow : EditorWindow
{
    static AssetsSortEidtorWindow instance;
    [MenuItem("Tool/资源分类")]
    private static void OpenWindow()
    {
        instance= GetWindow<AssetsSortEidtorWindow>();
    }

    TreeModelController<FileData> resFileData;
    const string ResourcesAssetsPath = "Assets/Resources";


    void OnEnable()
    {
        instance = this;
        RefreshResData();
    }
   public static void RefreshResData()
    {  
        string[] paths = PathUtils.GetDirectoryFilePath(ResourcesAssetsPath);
        paths = PathUtils.RemovePathWithEnds(paths, new string[] { ".meta" });
        if(instance)
        instance.resFileData = EditorDrawFileDirectory.GetFileDirectoryInfo(paths, true);

        List<string> nameList = new List<string>();
        foreach (var item in paths)
        {
            string name = Path.GetFileNameWithoutExtension(item) ;
            AssetsSortManagement.AddNewDefultLayerTag(name);
            nameList.Add(name);
        }
        List<string> removeNames = new List<string>();
        foreach (var item in AssetsSortManagement.LayersTagsDataList)
        {
            if (nameList.Contains(item.name))
                continue;
            else
                removeNames.Add(item.name);
        }

        foreach (var item in removeNames)
        {
            AssetsSortManagement.DeleteLayersTagsItem(item);
        }

        //Debug.Log("RefreshResData");

    }

    private int toolbarOption = 0;
    private string[] toolbarTexts = { "设置分类", "查找", "新增类型" };

    void OnGUI()
    {
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));
        switch (toolbarOption)
        {
            case 0:
               
                SetLayersTagsGUI();
                break;
            case 1:
                SearchLayersTagesGUI();
                break;
            case 2:
                AddLayersTagesGUI();
                break;

        }

        if (GUILayout.Button("保存"))
        {
            AssetsSortManagement.Save();
        }
    }
    #region  设置分类
    private Vector2 pos0 = Vector2.zero;
    private string selectLayer = "";
    private string selectTag = "";
    void SetLayersTagsGUI()
    {
        pos0 = GUILayout.BeginScrollView(pos0, "box");
        EditorDrawFileDirectory.DrawFileDirectory(resFileData, ShowFileDirectoryType.ShowAllFile,null, SelectCallback  , true, ChooseCallBack);
        GUILayout.EndScrollView();

        if (chooseItemList.Count > 0)
        {
            GUILayout.Label("设置：");
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
           selectLayer =  EditorDrawGUIUtil.DrawPopup("Layer", selectLayer, AssetsSortManagement.Layers);
            selectTag = EditorDrawGUIUtil.DrawPopup("Tag", selectTag, AssetsSortManagement.Tags);
            GUILayout.EndVertical();
            if (GUILayout.Button("确定"))
            {
                for (int i = 0; i < chooseItemList.Count; i++)
                {
                    string name = chooseItemList[i].FileNameWithoutExtension ;
                    AssetsSortManagement.SetLayerTag(name, selectLayer, selectTag);
                }
            }
            GUILayout.EndHorizontal();

            for (int i = 0; i < chooseItemList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(chooseItemList[i].content);
                GUILayout.FlexibleSpace();
                string name = chooseItemList[i].FileNameWithoutExtension;
                LayersTagsData lt = AssetsSortManagement.GetLayersTagsData(name);
                if (lt!=null)
                {
                    GUILayout.Label("Layer: " + lt.layer+" Tag: "+lt.tag); 
                }
                GUILayout.EndHorizontal();
            }
        }
    }

    private List<FileData> chooseItemList = new List<FileData>();
    private void SelectCallback(FileData t)
    {
        foreach (var item in chooseItemList)
        {
            item.isChoose = false;
        }
        chooseItemList.Clear();
        if (!t.isDirectory)
        {
            t.isChoose = true;
            chooseItemList.Add(t);
        }
    }

    private void ChooseCallBack(FileData t)
    {
        if (t.isDirectory)
        {
            t.isChoose = false;
            return;
        }
        if (t.isChoose)
        {
            chooseItemList.Add(t);
        }
        else
        {
            chooseItemList.Remove(t);
        }

    }
    #endregion

    #region 查找
    private int toolbarOption1 = 0;
    private string[] toolbarTexts1 = { "LayerTagSearch", "NameSearch" };
    void SearchLayersTagesGUI()
    {
        toolbarOption1 = GUILayout.Toolbar(toolbarOption1, toolbarTexts1, GUILayout.Width(Screen.width));
        switch (toolbarOption1)
        {
            case 0:
                LayerTagSearchGUI();
                break;
            case 1:
                NameSearchGUI();
                break;

        }
    }
    bool useLayer = false;
    bool useTag = false;
    Vector2 pos1 = Vector2.zero;
    void LayerTagSearchGUI()
    {
        GUILayout.BeginHorizontal();
        useLayer = (bool)EditorDrawGUIUtil.DrawBaseValue("使用Layer", useLayer);
        useTag = (bool)EditorDrawGUIUtil.DrawBaseValue("使用Tag", useTag);
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical();
        if(useLayer)
        selectLayer = EditorDrawGUIUtil.DrawPopup("Layer", selectLayer, AssetsSortManagement.Layers);
        if(useTag)
        selectTag = EditorDrawGUIUtil.DrawPopup("Tag", selectTag, AssetsSortManagement.Tags);
        GUILayout.EndVertical();

        pos1 = GUILayout.BeginScrollView(pos1,"box");
        foreach (var item in AssetsSortManagement.LayersTagsDataList)
        {
            if (useLayer)
            {
                if (item.layer != selectLayer)
                    continue;
            }
            if (useTag)
            {
                if (item.tag != selectTag)
                    continue;
            }
            GUILayout.Space(9);
            ShowLayersTagsDataItemGUI(item);
        }
        GUILayout.EndScrollView();

    }

    string searchName = "";
    void NameSearchGUI()
    {
        searchName = EditorDrawGUIUtil.DrawBaseValue("SearchName", searchName).ToString();
        pos1 = GUILayout.BeginScrollView(pos1);
        foreach (var item in AssetsSortManagement.LayersTagsDataList)
        {
            if (item.name.Contains(searchName))
            {
                GUILayout.Space(9);
                ShowLayersTagsDataItemGUI(item);
            }
        }
        GUILayout.EndScrollView();
    }

    void ShowLayersTagsDataItemGUI(LayersTagsData item)
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("Name: " + item.name);
        GUILayout.BeginHorizontal();
        item.layer = EditorDrawGUIUtil.DrawPopup("Layer", item.layer, AssetsSortManagement.Layers);
        item.tag = EditorDrawGUIUtil.DrawPopup("Tag", item.tag, AssetsSortManagement.Tags);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
#endregion

    #region 新增类型
    private int toolbarOptionSec = 0;
    private string[] toolbarTextsSec = { "Layers", "Tags" };
    void AddLayersTagesGUI()
    {
        toolbarOptionSec = GUILayout.Toolbar(toolbarOptionSec, toolbarTextsSec, GUILayout.Width(Screen.width));
        switch (toolbarOptionSec)
        {
            case 0:
                ReNameGUI(AssetsSortManagement.ReNameLayerName);
                LayerTagDataShowGUI("Layers", AssetsSortManagement.Layers, AssetsSortManagement.AddLayer, AssetsSortManagement.RemoveLayer);
                break;
            case 1:
                ReNameGUI(AssetsSortManagement.ReNameTagName);
                LayerTagDataShowGUI("Tags",AssetsSortManagement.Tags, AssetsSortManagement.AddTags, AssetsSortManagement.RemoveTag);
                break;

        }
    }

    string chooseRenameItem;
    bool isRename = false;
    string newName = "";
    void ReNameGUI(CallBack<string, string> reNameCallBack)
    {
        if (isRename)
        {
            GUILayout.Label("修改名字：");
            GUILayout.BeginHorizontal();
            GUILayout.Label(chooseRenameItem);
          
            newName = EditorDrawGUIUtil.DrawBaseValue("New Name", newName).ToString();
            if (GUILayout.Button("确定"))
            {
                reNameCallBack(chooseRenameItem, newName);
                isRename = false;
                chooseRenameItem = null;
            }
            if (GUILayout.Button("取消"))
            {
                isRename = false;
                chooseRenameItem = null;
            }
            GUILayout.EndHorizontal();
        }
    }
    void LayerTagDataShowGUI(string name, List<string> data, CallBackR<bool, string> addNewDataCallBack,CallBackR<bool, string> deleteCallBack)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            string s = "new " + name + data.Count;
          bool res=  addNewDataCallBack(s);
            if (!res)
            {
                s += "_1";
                addNewDataCallBack(s);
            }
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < data.Count; i++)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("   "+data[i]);
            bool isDefult = data[i] == "None" ? true : false;
            if (!isDefult)
            {

                if (GUILayout.Button("ReName"))
                {
                    chooseRenameItem = data[i];
                    isRename = true;
                    return;
                }

                if ( GUILayout.Button("Delete"))
                {
                    deleteCallBack(data[i]);
                    return;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
#endregion

}

using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorDrawFileDirectory  {
    /// <summary>
    /// 获取文件目录信息
    /// </summary>
    /// <param name="assetPath">资源文件夹路径</param>
    /// <param name="isAllOpen">是否展开所有目录</param>
    /// <param name="iconName">当为空字符时使用文件夹目录图标，不为空时强制使用此图标</param>
    /// <returns></returns>
    public static TreeModelController<FileData> GetFileDirectoryInfo(string[] paths, bool isAllOpen =false,string iconName= "",CallBack<FileData> initParamsCallBack=null)
    {
        TreeModelController<FileData> control = new TreeModelController<FileData>();
        control.AddPathsToCreateNode(paths);
        control.ListForeachNode((node) =>
        {
            node.isSelected = isAllOpen;
            node.content = GetPathGUIContent(node.relativeRootPath, iconName);
            if (initParamsCallBack != null)
                initParamsCallBack(node);
            return true;
        });

        return control;
    }
    /// <summary>
    /// 绘制文件目录GUI
    /// </summary>
    /// <param name="control">文件信息</param>
    /// <param name="direType">显示文件夹或全部目录</param>
    /// <param name="selectCallback">选择一个文件回调</param>
    /// <param name="isShowChoose">是否打开勾选文件</param>
    /// <param name="chooseCallBack">勾选文件回调</param>
    public static void DrawFileDirectory(TreeModelController<FileData> control, ShowFileDirectoryType direType = ShowFileDirectoryType.ShowAllFile, string[] showEndsWith = null,CallBack < FileData> selectCallback=null, bool isShowChoose =false, CallBack<FileData> chooseCallBack =null)
    {            
        GUI.enabled = true;
        EditorGUIUtility.SetIconSize(Vector2.one * 16);
        control.TreeForeachNode((data) =>
        {
            if (direType == ShowFileDirectoryType.OnlyDirectory)
            {
                if (data.isDirectory)
                {
                    DrawGUIData(control, data, direType, selectCallback, isShowChoose, chooseCallBack);
                    if (!data.isSelected)
                        return false;
                }
            }
            else
            {
                if (data.isDirectory && !data.isSelected)
                {
                    DrawGUIData(control, data, direType, selectCallback, isShowChoose, chooseCallBack);
                    return false;
                }
                if (showEndsWith != null)
                {
                    if (!data.isDirectory)
                    {
                        if (OtherUtils.ArrayContains(showEndsWith, Path.GetExtension(data.relativeRootPath)))
                        {
                            DrawGUIData(control, data, direType, selectCallback, isShowChoose, chooseCallBack);

                        }
                        return true;
                    }
                }

                DrawGUIData(control, data, direType, selectCallback, isShowChoose, chooseCallBack);
            }
            return true;
        });
    }

    static FileData selectData;

  static  void DrawGUIData(TreeModelController<FileData> control,FileData data, ShowFileDirectoryType direType = ShowFileDirectoryType.ShowAllFile, CallBack<FileData> selectCallback = null, bool isShowChoose = false, CallBack<FileData> chooseCallBack = null)
    {
        GUIStyle style = "Label";
        style.richText = true;
        Rect rt = GUILayoutUtility.GetRect(data.content, style);
        if (data == selectData)
        {
            EditorGUI.DrawRect(rt, Color.gray);
        }
        rt.x += (24 * data.Deep);

        if (data.isDirectory)
        {
            Rect rt1 = new Rect(rt.x - 12, rt.y, 20, rt.height);
            if (direType == ShowFileDirectoryType.OnlyDirectory)
            {
                if (HaveChildDirectory(control,data))
                    data.isSelected = EditorGUI.Foldout(rt1, data.isSelected, "");
            }
            else
                data.isSelected = EditorGUI.Foldout(rt1, data.isSelected, "");
        }
        if (isShowChoose)
        {
           // rt.x += 20;
            Rect rt1 = new Rect(rt.x - 13, rt.y, 20, rt.height);
            if (data.isDirectory)
            {
                if (direType == ShowFileDirectoryType.OnlyDirectory)
                {
                    if (HaveChildDirectory(control, data))
                        rt1.x -= 13;
                }
                else
                {
                    rt1.x -= 13;
                }
            }
                bool oldChoose = data.isChoose;
            data.isChoose = EditorGUI.ToggleLeft(rt1, "", data.isChoose);
            if(data.isChoose != oldChoose)
            {
                if (chooseCallBack != null)
                    chooseCallBack(data);
            }
        }
        if (GUI.Button(rt, data.content, style))
        {
          //  data.isSelected = !data.isSelected;
            selectData = data;
            if (selectCallback != null)
                selectCallback(data);
        }

    }

    private static bool HaveChildDirectory(TreeModelController<FileData> control, FileData data)
    {
        bool isHave = false;
        control.SearchChilds(data, (node) =>
        {
            if (node.isDirectory)
            {
                isHave = true;
                return false;
            }
            return true;
        });
        return isHave;
}

  public static  GUIContent GetPathGUIContent(string path,string iconName ="")
    {
        string name = "";
        try
        {
            name = PathUtils.GetFileName(path);
        }catch(Exception e)
        {
            Debug.LogError("Path: " + path + "\n"+e);
        }
        if (string.IsNullOrEmpty(iconName))
        {
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            if (asset)
            {
               Texture tx = AssetDatabase.GetCachedIcon(path);
                GUIContent cc = new GUIContent(name, tx);
                return cc;
            }
            else
            {
                GUIContent cc = EditorGUIUtility.IconContent("DefaultAsset Icon");
                cc.text = name;
                return new GUIContent(cc);
            }
        }
        else
        {
            GUIContent cc = EditorGUIUtility.IconContent(iconName);
            cc.text = name;
            return new GUIContent( cc);
        }
       
    }



}
public class FileData : TreeNodeBase
{
    public bool isSelected = false;
    public bool isChoose = false;
    public bool isDirectory = false;
    public GUIContent content;
    public Dictionary<string, object> otherData = new Dictionary<string, object>();
    public string FileName { get { return Path.GetFileName(InternalFullPath); } }
    public string FileNameWithoutExtension { get { return Path.GetFileNameWithoutExtension(InternalFullPath); } }
    public FileData(int id,string path) : base(id, path)
    {
        isDirectory = Directory.Exists(path);
    }
}
/// <summary>
/// 文件显示类型
/// </summary>
public enum ShowFileDirectoryType
{
    /// <summary>
    /// 显示文件和文件目录
    /// </summary>
    ShowAllFile,
    /// <summary>
    /// 只显示目录
    /// </summary>
    OnlyDirectory,
}

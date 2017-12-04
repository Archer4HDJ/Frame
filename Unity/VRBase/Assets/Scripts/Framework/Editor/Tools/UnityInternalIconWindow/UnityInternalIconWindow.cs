using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

public class UnityInternalIconWindow : EditorWindow
{
    [MenuItem("Tool/工具/反射查看Unity内建图标")]
    static void Open()
    {
        GetWindow<UnityInternalIconWindow>();
    }

    void OnEnable()
    {
        Init();
    }
    GUIContent[] findTextureIcons;
    GUIContent[] iconContentIcons;
    GUIContent[] loadIconIcons;
    GUIContent[] internalWindowIcons;
    GUIContent[] allIcons;
    private void Init()
    {
        findTextureIcons = GetIconContent("FindTexture获取");
        iconContentIcons = GetIconContent("IconContent获取");
        loadIconIcons = GetIconContent("LoadIcon获取");
        internalWindowIcons = GetIconContent("内置窗口图标");
        List<GUIContent> list = new List<GUIContent>();
        list.AddRange(findTextureIcons);
        list.AddRange(iconContentIcons);
        list.AddRange(loadIconIcons);
        list.AddRange(internalWindowIcons);
        allIcons = list.ToArray();
    }
    private int toolbarOption = 0;
    private string[] toolbarTexts = { "全部内置图标", "搜索" };

    private int toolbarOptionSec = 0;
    private string[] toolbarTextsSec = { "传递给 EditorGUIUtility.FindTexture 的参数", "IconContent获取的", "传递给 EditorGUIUtility.LoadIcon 的参数" , "添加EditorWindowTitleAttribute 特性的窗口的图标" };
    Vector2 scrollPosition = new Vector2(0, 0);
    string search = "";
    void OnGUI()
    {
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));
        switch (toolbarOption)
        {
            case 0:

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                toolbarOptionSec = GUILayout.Toolbar(toolbarOptionSec, toolbarTextsSec, GUILayout.Width(Screen.width - 40));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                switch (toolbarOptionSec)
                {
                    case 0:
                        GUILayout.Space(10);
                        foreach (GUIContent content in findTextureIcons)
                        {
                            ShowStyleGUI(content);
                        }
                        break;
                    case 1:
                        GUILayout.Space(10);
                        foreach (GUIContent content in iconContentIcons)
                        {
                            ShowStyleGUI(content);
                        }

                        GUILayout.FlexibleSpace();
                        break;
                    case 2:
                        GUILayout.Space(10);
                        foreach (GUIContent content in loadIconIcons)
                        {
                            ShowStyleGUI(content);
                        }
                        break;
                    case 3:
                        GUILayout.Space(10);
                        foreach (GUIContent content in internalWindowIcons)
                        {
                            ShowStyleGUI(content);
                        }
                        break;
                }
                break;
            case 1:
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.Label("Click a right Button to copy its Name to your Clipboard", "MiniBoldLabel");
                GUILayout.FlexibleSpace();
                GUILayout.Label("Search:");
                search = EditorGUILayout.TextField(search);

                GUILayout.EndHorizontal();
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                foreach (GUIContent content in allIcons)
                {

                    if (content.text.ToLower().Contains(search.ToLower()))
                    {
                        ShowStyleGUI(content);
                    }
                }
                break;


        }
        GUILayout.EndScrollView();
    }
    void ShowStyleGUI( GUIContent content)
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Space(40);
        GUILayout.Label(content);
        GUILayout.FlexibleSpace();
        EditorGUILayout.SelectableLabel(content.text);
        GUILayout.Space(6);
        if (GUILayout.Button("复制到剪贴板"))
        {
           // EditorGUIUtility.systemCopyBuffer = style.text;
            TextEditor tx = new TextEditor();
            tx.text = content.text;
            tx.OnFocus();
            tx.Copy();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(11);
    }

    GUIContent[] GetIconContent(string fileName)
    {
        string[] ss = TextLoad(fileName);
        return GetGUIContent(ss);
    }
    private string[] TextLoad(string fileName)
    {
        TextAsset obj = Resources.Load(fileName) as TextAsset;
        string tt = obj.text;
        return tt.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
    }
    private GUIContent[] GetGUIContent(string[] iconNames)
    {
        List<GUIContent> list = new List<GUIContent>();
        for (int i = 0; i < iconNames.Length; i++)
        {
            try
            {
                GUIContent cc = EditorGUIUtility.IconContent(iconNames[i].Trim());
                cc.text = iconNames[i];
                list.Add(cc);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        return list.ToArray();
    }
   
}
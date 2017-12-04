using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HDJ.Framework.Modules;
using HDJ.Framework.Utils;
using System;
using System.IO;
using UnityEngine.UI;

public class UICreateEditorWindow : EditorWindow {
    [MenuItem("Tool/UI工具",priority =1001)]
	private static void OpenWindow()
    {
        GetWindow<UICreateEditorWindow>();
        

    }
    private void OnDestroy()
    {
        EditorUtility.ClearProgressBar();
    }
    private void OnEnable()
    {
        UIRoot.CreateUIFrame();
    }

    private const string SaveUIPrefabPathDir = "Assets/Resources/Prefabs/UI/";
    private const string SaveUIScriptPathDir = "Assets/Scripts/Game/UI/";

    private int toolbarOption = 0;
    private string[] toolbarTexts = { "新建UI", "显示所有UI" };
    private void OnGUI()
    {
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));
        switch (toolbarOption)
        {
            case 0:
                CreateUIGUI();
                break;
            case 1:
                ShowAllUIWindowPrefabGUI();
                break;
        }

        if (isCreating)
        {
            ShowProgress();
        }
        if (isCtreatingPrefab)
        {
            CreateUIPrefab(UIName);
        }
    }

    private string newUIName = "";
    private string UIName = "";
    private string description = "";
    //this page's type
    private UIType type = UIType.Normal;

    //how to show this page.
    private UIMode mode = UIMode.DoNothing;
    private void CreateUIGUI()
    {
        newUIName = EditorDrawGUIUtil.DrawBaseValue("UI名字：", newUIName).ToString();

        string tempName = newUIName + "Window";
        GUILayout.Label("显示名字：" + tempName);

        GUILayout.Space(5);

        type = (UIType)EditorDrawGUIUtil.DrawBaseValue("UI Type: " , type);
        mode = (UIMode)EditorDrawGUIUtil.DrawBaseValue("UI Mode: ", mode);

        string dis = "";
        if(mode == UIMode.DoNothing)
        {
            dis = "打开该界面时, 不加入backSequence队列";
        }
        if (mode == UIMode.HideOther)
        {
            dis = "打开该界面时, 闭其他队列的界面,加入backSequence队列";
        }
        if (mode == UIMode.NeedBack)
        {
            dis = "打开该界面时,不关闭其他界面,加入backSequence队列";
        }
        EditorGUILayout.HelpBox(dis, MessageType.Info);

        bool isCanUse = true;
        if (ResourcePathManager.ContainsFileName(tempName))
        {
            GUILayout.Space(5);
            isCanUse = false;
            EditorGUILayout.HelpBox("与资源名重复", MessageType.Error);
        }
        if (string.IsNullOrEmpty(newUIName))
        {
            isCanUse = false;
        }

        GUILayout.Label("描述：");
        description = GUILayout.TextArea(description);
        GUILayout.Space(5);
        if (isCanUse && !isCreating && GUILayout.Button("创建"))
        {
            UIName = tempName;
            CreateUI(tempName);
        }

       
    }
    private string showProgressContent="";
    private float progress = 0f;
    private void ShowProgress()
    {
        EditorUtility.DisplayProgressBar("正在创建", showProgressContent, progress);
    }

    private bool isCreating = false;
    private bool isCtreatingPrefab = false;
    private void CreateUI(string UIName)
    {
        isCreating = true;
        GlobalEvent.AddEvent(EditorGlobalEventEnum.OnCreateAsset, (param) =>
         {
             Debug.Log("OnCreateAsset");
             isCtreatingPrefab = true;
             showProgressContent = "创建预制";
             progress = 0.5f;
         },true);

        CreateUIScript(UIName);
        showProgressContent = "创建脚本";
        progress = 0.25f;
    }
    
    private void CreateUIScript(string className)
    {
        string classString = "using UnityEngine;\r\n";
        classString += "using HDJ.Framework.Modules;\r\n";
        classString += "\t/// <summary>\r\n \t/// " + description + "\r\n\t/// </summary>\r\n";
        classString += "public class " + className + " : " + typeof(UIWindowBase).Name + "\r\n{ \r\n";

        classString += "\tprotected override void OnOpen(params object[] param)\r\n\t{\r\n\t}";
     
        classString += "\r\n}";

        FileUtils.CreateTextFile(SaveUIScriptPathDir+ className +"/"+ className+".cs", classString);
        AssetDatabase.Refresh();
    }

    private void CreateUIPrefab(string prefabName)
    {
        Type t = ReflectionUtils.GetTypeByTypeFullName( prefabName);
            if (t == null)
            return;

        GameObject winObj = new GameObject(prefabName);
        winObj.AddComponent<Canvas>();
        winObj.AddComponent<GraphicRaycaster>();
        UIWindowBase winBase = (UIWindowBase)winObj.AddComponent(t);
        winBase.type = type;
        winBase.mode = mode;

        UIRoot.SetUIParentByUIType(winObj, type);
        SetRectTransform(winObj);

        GameObject backGroundRoot = new GameObject("BackGroundRoot");
        backGroundRoot.transform.SetParent(winObj .transform);
        SetRectTransform(backGroundRoot);
        winBase.backGroundRoot = backGroundRoot;

        GameObject root = new GameObject("Root");
        root.transform.SetParent(winObj.transform);
        SetRectTransform(root);
        winBase.root = root;
        winObj.layer = LayerMask.NameToLayer("UI");
        backGroundRoot.layer = LayerMask.NameToLayer("UI"); 
        root.layer = LayerMask.NameToLayer("UI"); 

        string p = SaveUIPrefabPathDir + prefabName + "/";
        if (!Directory.Exists(p))
        {
            Directory.CreateDirectory(p);
        }
        PrefabUtility.CreatePrefab(p + prefabName + ".prefab", winObj, ReplacePrefabOptions.ConnectToPrefab);
        AssetDatabase.Refresh();

        progress = 1;
        isCtreatingPrefab = false;
        isCreating = false;
        EditorUtility.ClearProgressBar();
        newUIName = "";
        UIName = "";
    }

    private void SetRectTransform(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect == null)
        {
            rect= obj.AddComponent<RectTransform>();
        }
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
    }

    List<GameObject> allUIPrefabs = new List<GameObject>();
    private Vector2 pos;
    private void ShowAllUIWindowPrefabGUI()
    {
        if (GUILayout.Button("刷新",GUILayout.Width(50)))
        {

            LoadAllUIPreabs();
        }

        GUILayout.Space(6);

        pos = GUILayout.BeginScrollView(pos,"box");
        for (int i = 0; i < allUIPrefabs.Count; i++)
        {
            GameObject obj = allUIPrefabs[i];

            GUILayout.BeginVertical("GameViewBackground");

            GUILayout.Label("UI Name ：" + obj.name);
            EditorGUILayout.ObjectField("UI Window:", obj, typeof(GameObject), true);
            UIWindowBase win = obj.GetComponent<UIWindowBase>();
            win.type = (UIType)EditorDrawGUIUtil.DrawBaseValue("UI Type: ", win.type);
            win.mode = (UIMode)EditorDrawGUIUtil.DrawBaseValue("UI Mode: ", win.mode);

            EditorUtility.SetDirty(obj);
            GUILayout.Space(4);
            if (GUILayout.Button("加载"))
            {
                GameObject t = Instantiate<GameObject>(obj);
                t.name = obj.name;
                UIRoot.SetUIParentByUIType(t, win.type);
                t = PrefabUtility.ConnectGameObjectToPrefab(t, obj);
            }
            GUILayout.Space(4);
            if (GUILayout.Button("删除"))
            {
                if(EditorUtility.DisplayDialog("警告","是否删除UI Window：" + obj.name, "Ok", "Cancel"))
                {
                    Directory.Delete(SaveUIPrefabPathDir + obj.name, true);
                    Directory.Delete(SaveUIScriptPathDir + obj.name, true);
                    AssetDatabase.Refresh();
                    LoadAllUIPreabs();
                    return;
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(6);
        }
        GUILayout.EndScrollView();
    }

    private void LoadAllUIPreabs()
    {
        allUIPrefabs.Clear();
        string[] paths = PathUtils.GetDirectoryFilePath(SaveUIPrefabPathDir, new string[] { ".prefab" });

        foreach (var item in paths)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(item);
            if (obj.GetComponent<UIWindowBase>())
                allUIPrefabs.Add(obj);
        }
    }
}

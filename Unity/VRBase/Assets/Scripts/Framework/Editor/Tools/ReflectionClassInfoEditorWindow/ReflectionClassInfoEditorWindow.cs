using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ReflectionClassInfoEditorWindow : EditorWindow {

    private static ReflectionClassInfoEditorWindow instance;
    [MenuItem("Tool/工具/反射查看类的信息")]
    private static void OpenWindow()
    {
        instance= GetWindow<ReflectionClassInfoEditorWindow>();
    }
  static  List<ToolBarData<Type>> scriptsNames = new List<ToolBarData<Type>>();
  static  ToolBarData<Type> selectCont;
    //System.Type ac = Assembly.Load("UnityEditor.Graphs").GetType("UnityEditor.Graphs.AnimatorControllerTool");
    public static void AddType(string assemblyName, string typeFullName)
    {
        Type ac1 = null;
        if (string.IsNullOrEmpty(assemblyName))
        {
            ac1 = ReflectionUtils.GetTypeByTypeFullName(typeFullName);
        }
        else
            ac1 = HDJ.Framework.Utils.ReflectionUtils.GetTypefromAssemblyFullName(assemblyName, typeFullName);
        AddType(ac1);
    }
    public static void AddType(Type t)
    {
        if (instance == null)
            OpenWindow();
        if (t == null)
        {
            Debug.LogError("Type is Null");
            return;
        }
        ToolBarData<Type> d = new ToolBarData<Type>(t.Name, t);
       scriptsNames.Insert(0, d);
    }

    private string assemblyName = "UnityEditor.Graphs";
    private string typeFullName = "UnityEditor.Graphs.AnimatorControllerTool";

    private FieldInfo[] fields;
    private PropertyInfo[] propertyInfos;
    private MethodInfo[] methodInfos;
    private ConstructorInfo[] constructorInfos;

    private int toolbarOption = 0;
    private string[] toolbarTexts = { "预览", "详细信息" };

    private int toolbarOptionSec = 0;
    private string[] toolbarTextsSec = { "主体","构造方法","字段", "属性" ,"方法"};
    private Vector2 pos = Vector2.zero;
    private Vector2 posS = Vector2.zero;
    void OnGUI()
    {
      
        assemblyName = EditorDrawGUIUtil.DrawBaseValue("程序集名:", assemblyName).ToString();
        typeFullName = EditorDrawGUIUtil.DrawBaseValue("类型全名:", typeFullName).ToString();

        if (GUILayout.Button("获取全部信息"))
        {
            AddType(assemblyName, typeFullName);
        }
        GUILayout.BeginVertical();

        selectCont = EditorDrawToolBarGUI.DrawToolBar(ref posS, scriptsNames, selectCont,(tt)=>
        {
            GetTypeAllInfo(tt.data);
        });
             
        Type ac=null;

        if (selectCont != null)
        {
            ac = selectCont.data;
        }
        if (ac == null || fields==null) return;
      
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));
       
        switch (toolbarOption)
        {
            case 0:
                string content = ReflectionToCodeDefineInfo.GetTypeAllInfo(ac, constructorInfos, fields, propertyInfos, methodInfos);
                GUILayout.Space(6);
                pos = EditorGUILayout.BeginScrollView(pos);
                GUILayout.BeginVertical("NotificationBackground");


                List<string> sList = new List<string>();
                int size = content.Length;
                int everyDrawSize = 5000;
                while (size >= everyDrawSize)
                {
                    string subs = content.Substring(0, everyDrawSize);
                    content = content.Remove(0, everyDrawSize);
                    int id = content.IndexOf("\n");
                    subs += content.Substring(0, id + 1);

                    content = content.Remove(0, id + 1);
                    size = content.Length;
                    sList.Add(subs);                  
                   
                }
                if (content.Length > 0)
                    sList.Add(content);
                for (int i = 0; i < sList.Count; i++)
                {
                    GUIStyle style1 = new GUIStyle("Label");
                    style1.richText = true;
                    style1.fontSize = 15;
                    EditorGUILayout.TextArea(sList[i], style1);
                }
             
                GUILayout.EndVertical();
                break;
            case 1:
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                toolbarOptionSec = GUILayout.Toolbar(toolbarOptionSec, toolbarTextsSec, GUILayout.Width(Screen.width - 40));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                pos = EditorGUILayout.BeginScrollView(pos);
                switch (toolbarOptionSec)
                {
                    case 0:
                        GUILayout.Space(6);
                        ShowMainType(ac);
                        break;
                    case 1:
                        GUILayout.Space(6);
                        ShowInfoGUI(ac,constructorInfos);
                        break;
                    case 2:
                        GUILayout.Space(6);
                        ShowInfoGUI(ac,fields);
                        break;
                    case 3:
                        GUILayout.Space(6);
                        ShowInfoGUI(ac,propertyInfos);
                        break;
                    case 4:
                        GUILayout.Space(6);
                        ShowInfoGUI(ac,methodInfos);
                        break;
                }
               
                break;
            case 2:

                break;

        }
        
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
       
    }

    void GetTypeAllInfo(Type t)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
        fields = t.GetFields(flags);
        propertyInfos = t.GetProperties(flags);
        methodInfos = t.GetMethods(flags);
        constructorInfos = t.GetConstructors(flags);
    }

    void ShowMainType(Type type)
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Space(6);
         ShowFieldANDPropertyInfo(type);
        GUILayout.EndHorizontal();
    }
    private ConstructorInfo selectC;
    void ShowInfoGUI(Type type, ConstructorInfo[] infos)
    {
        ShowInfoGUI(infos, ref selectC, (f) =>
        {
            return ReflectionToCodeDefineInfo.GetConstructorInfoToDeclaring(type, f);
        }, (f) =>
        {
            ShowFieldANDPropertyInfo(f, () =>
            {
                ParameterInfo[] pis = f.GetParameters();
                string ss = "";
                for (int i = 0; i < pis.Length; i++)
                {
                    Type tt = pis[i].ParameterType;
                    ss += (string.IsNullOrEmpty(tt.FullName)) ? tt.ToString() : tt.FullName;
                    if (i < (pis.Length - 1))
                        ss += ",";
                }
                GUILayout.Space(5);
                ShowLabel("参数信息", ss);
                for (int i = 0; i < pis.Length; i++)
                {
                    ParameterInfo tt = pis[i];
                    ShowLabel("参数" + i, tt.Name);
                    ShowFieldANDPropertyInfo(tt);
                    GUILayout.Space(6);
                }
                //MethodInfo m = f.get ();
                //if (!f.Equals(m))
                //{
                //    GUILayout.Space(10);
                //    ShowLabel("父方法", ReflectionToCodeDefineInfo.GetMethodInfoToDeclaring(type, m));
                //    ShowPropertyInfo(m);
                //}
            });


        });
    }
    private FieldInfo selectF;
    void ShowInfoGUI(Type type, FieldInfo[] infos)
    {
        ShowInfoGUI( infos, ref selectF,(f)=>
        {
            return ReflectionToCodeDefineInfo.GetFieldInfoToDeclaring(type,f);
        },
        (f) =>
        {
            ShowFieldANDPropertyInfo(f);
        });
    }
    private PropertyInfo selectP;
    void ShowInfoGUI(Type type, PropertyInfo[] infos)
    {
        ShowInfoGUI(infos, ref selectP, (f) =>
        {
            return ReflectionToCodeDefineInfo.GetPropertyInfoToDeclaring(type,f);
        }, (f) =>
        {
            ShowFieldANDPropertyInfo(f);
        });
    }
    private MethodInfo selectM;
    void ShowInfoGUI(Type type, MethodInfo[] infos)
    {
        ShowInfoGUI(infos, ref selectM, (f) =>
        {
            return ReflectionToCodeDefineInfo.GetMethodInfoToDeclaring(type, f);
        }, (f) =>
        {
            ShowFieldANDPropertyInfo(f, () =>
            {
                ParameterInfo[] pis = f.GetParameters();
                string ss = "";
                for (int i = 0; i < pis.Length; i++)
                {
                    Type tt = pis[i].ParameterType;
                    ss += (string.IsNullOrEmpty(tt.FullName)) ? tt.ToString() : tt.FullName;
                    if (i < (pis.Length - 1))
                        ss += ",";
                }
                GUILayout.Space(5);
                ShowLabel("参数信息", ss);
                for (int i = 0; i < pis.Length; i++)
                {
                    ParameterInfo tt = pis[i];
                    ShowLabel("参数" + i, tt.Name);
                    ShowFieldANDPropertyInfo(tt);
                    GUILayout.Space(6);
                }
                MethodInfo m = f.GetBaseDefinition();
                if (!f.Equals(m))
                {
                    GUILayout.Space(10);
                    ShowLabel("父方法", ReflectionToCodeDefineInfo.GetMethodInfoToDeclaring(type, m));
                    ShowFieldANDPropertyInfo(m);
                }
            });


        }, (f) =>
         {
             if (type.IsAbstract || type.IsInterface)
                 return;
                if (GUILayout.Button("调用测试"))
                {
                    ReflectionTestMethodRunnerWindow.AddTest(selectCont.data, f);
                }         
        });
    }

    void ShowFieldANDPropertyInfo(object obj,CallBack addOtherShowCallBack=null)
    {
        if (obj == null) return;
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        GUILayout.BeginVertical();
        Type t = obj.GetType();
       FieldInfo[] fs =  t.GetFields(flags);
        for (int i = 0; i < fs.Length; i++)
        {
            object value = fs[i].GetValue(obj);
            ShowLabel(fs[i].Name, value);
        }
        PropertyInfo[] propertyInfos = t.GetProperties(flags);
        for (int i = 0; i < propertyInfos.Length; i++)
        {
            try
            {
                if (propertyInfos[i].CanRead)
                {
                    object value = propertyInfos[i].GetValue(obj, null);
                    ShowLabel(propertyInfos[i].Name, value);
                }
               
            }
            catch //(Exception e)
            {
               // Debug.LogError("propertyInfos[i]:"+ propertyInfos[i].Name+"\n"+e.ToString());
                continue;
            }
        }
        if (addOtherShowCallBack != null)
        {
            addOtherShowCallBack();
        }
        GUILayout.EndVertical();
    }
    void ShowLabel(string name,object value)
    {
        GUIStyle style = "CN StatusWarn";
        style.richText = true;
        style.fontSize = 12;
        EditorGUILayout .TextArea(name+" : "+ value, style);
    }
    void ShowInfoGUI<T>(T[] info, ref T selectT, CallBackR<string,T> detailNameButtonGUICallBack, CallBack<T> detailInfoGUICallBack,CallBack<T> followFuctionButonUIExtend=null) where T : MemberInfo
    {
        if (info == null) return;
        foreach (var f in info)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            bool select = f.Equals(selectT);
            string str = select ? "▼" : "►";
            str +=" "+ detailNameButtonGUICallBack(f);
            GUIStyle style = "hostview";
            style.richText = true;
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = 14;
            if (GUILayout.Button(str, style))
            {
                if (select)
                    selectT = null;
                else
                    selectT = f;
            }
            if (followFuctionButonUIExtend != null)
                followFuctionButonUIExtend(f);
          
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal("Label");
            GUILayout.Space(5);

            if (select)
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical("box");
                ShowLabel("定义", detailNameButtonGUICallBack(f));
                if (detailInfoGUICallBack != null)
                    detailInfoGUICallBack(f);
                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }


    }
}

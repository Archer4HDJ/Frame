using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ReflectionTestMethodRunnerWindow : EditorWindow
{
    static ReflectionTestMethodRunnerWindow instance;
    public static void OpenWindow()
    {
        if (instance == null)
            instance = GetWindow<ReflectionTestMethodRunnerWindow>();
        else
            FocusWindowIfItsOpen<ReflectionTestMethodRunnerWindow>();
    }

    private TestMethodData mData;
    private Type classType;
    private MethodInfo method;
    private ConstructorInfo[] consInfos;
    private List<string> consDefineList = new List<string>();

    public static void AddTest(Type classType, MethodInfo method)
    {

        OpenWindow();
    
        instance.classType = classType;
        instance.method = method;

        instance.mData = new TestMethodData();
        instance.mData.assemblyName = classType.Assembly.FullName;
        instance.mData.typeFullName = classType.FullName;
        instance.mData.methodeName = method.Name;
      
        TestValue list = GetDefultTestValue(method);
        instance.mData.paramsData.Add(list);
        
        instance.consInfos = classType.GetConstructors();
        for (int i = 0; i < instance.consInfos.Length; i++)
        {
            instance.consDefineList.Add( instance.consInfos[i].Name);
        }
    }

    private static TestValue GetConstructorInfoTestValue(ConstructorInfo method)
    {
        TestValue testv = new TestValue();
        testv.isStatic = method.IsStatic;
        ParameterInfo[] pis = method.GetParameters();
        for (int i = 0; i < pis.Length; i++)
        {
            ParameterInfo p = pis[i];

            testv.parameterDatas.Add(GetParameterInfoAndCreateDefultValue(p));
        }

        testv.methodeName = method.Name;
        
        return testv;
    }
    private static TestValue GetDefultTestValue(MethodInfo method)
    {

        TestValue testv = new TestValue();
        testv.isStatic = method.IsStatic;
        ParameterInfo[] pis = method.GetParameters();
        for (int i = 0; i < pis.Length; i++)
        {
            ParameterInfo p = pis[i];

            testv.parameterDatas.Add(GetParameterInfoAndCreateDefultValue( p));
        }
        
        testv.methodeName = method.Name;
        testv.resultWant = GetParameterInfoAndCreateDefultValue(method.ReturnParameter);
        return testv;
    }
    private static ParameterData GetParameterInfoAndCreateDefultValue( ParameterInfo p)
    {
        ParameterData tv = new ParameterData();
        tv.parameterName = p.Name;
            if (p.ParameterType.IsByRef)
            {
                tv.isByRef = true;
            }
            tv.parameterType = p.ParameterType;
            object temp = p.GetType().GetField("DefaultValueImpl", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(p);
        if (temp != null && !string.IsNullOrEmpty(temp.ToString()))
        {
            tv.setValue = temp;
        }
        else
        {
            if (tv.parameterType.IsPrimitive)
            {
                tv.setValue = Activator.CreateInstance(tv.parameterType);
            }
            else if (tv.parameterType == typeof(string))
            {
                tv.setValue = "";
            }
        }
        return tv;
    }

    private string selectConsName = "";
    void OnGUI()
    {
        if (mData == null)
            return;
        GUILayout.Label("AssemblyName : "+ mData.assemblyName);
        GUILayout.Label("TypeFullName : " + mData.typeFullName);
        GUILayout.Label("MethodeName : " + mData.methodeName);
        //GUILayout.Label("IsStatic : " + mData.isStatic);
        EditorGUILayout.Separator();
        selectConsName = EditorDrawGUIUtil.DrawPopup(name, selectConsName, consDefineList);
        int index = consDefineList.IndexOf(selectConsName);
        mData.constructorParas = GetConstructorInfoTestValue(consInfos[index]);

        for (int i = 0; i < mData.constructorParas.parameterDatas.Count; i++)
        {
            ParameterData p = mData.constructorParas.parameterDatas[i];
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(p.parameterType.FullName + " " + p.parameterName);
            p.setValue = EditorDrawGUIUtil.DrawBaseValue("=", p.setValue);
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Separator();
        for (int i = 0; i < mData.paramsData.Count; i++)
        {
            GUILayout.Label("输入参数：");
            TestValue tv = mData.paramsData[i];
            for (int j = 0; j < tv.parameterDatas.Count; j++)
            {
                ParameterData p = tv.parameterDatas[j];
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(p.parameterType.FullName + " " + p.parameterName);
                p.setValue = EditorDrawGUIUtil.DrawBaseValue("=", p.setValue);
                GUILayout.EndHorizontal();
            }
            GUILayout.Label("预计结果：");
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(tv.resultWant.parameterType.FullName + " " + tv.resultWant.parameterName);
            tv.resultWant.setValue = EditorDrawGUIUtil.DrawBaseValue("=", tv.resultWant.setValue);
            GUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            if (GUILayout.Button("Run"))
            {
                object instance = null;
                if (!tv.isStatic)
                {
                    instance = GetTypeInstance(classType, mData.constructorParas);
                }
                TestRun(classType, instance, tv);
            }
            if (tv.testState == TestState.NoTest)
                GUILayout.Label("○");
            else if (tv.testState == TestState.NoPass)
                GUILayout.Label("X");
            else
                GUILayout.Label("√");
            EditorGUILayout.Separator();
        }
    }

    private object GetTypeInstance(Type classType, TestValue constructParas)
    {
        List<object> temp = new List<object>();
        for (int i = 0; i < constructParas.parameterDatas.Count; i++)
        {
            temp.Add(constructParas.parameterDatas[i].setValue);
        }

        constructParas.tempParameter = temp.ToArray();

       return Activator.CreateInstance(classType, constructParas.tempParameter);
    }

    private object TestRun(Type classType, object instance,TestValue tv)
    {
        List<object> temp = new List<object>();
        object res =null;
        for (int i = 0; i < tv.parameterDatas.Count; i++)
        {
            temp.Add(tv.parameterDatas[i].setValue);
        }

        tv.tempParameter = temp.ToArray();
        try
        {
          
               res =  HDJ.Framework.Utils.ReflectionUtils.InvokMethod(classType, instance, tv.methodeName, ref tv.tempParameter);
            if (res != null)
            {
               if(tv.resultWant.setValue.Equals(res))
                {
                    tv.testState = TestState.Pass;
                    tv.exceptionInfo = "";
                }
                else
                {
                    tv.testState = TestState.NoPass;
                }
            }
        }catch(Exception e)
        {
            Debug.LogError(e);
            
            tv.exceptionInfo = e.ToString();
         
        }
      
        return res;
    }
}

public class TestMethodData
{
    public string assemblyName = "";
    public string typeFullName = "";
    public string methodeName = "";
    public TestValue constructorParas = new TestValue();
    public List<TestValue> paramsData = new List<TestValue>();

}

public class TestValue
{
    public string methodeName = "";
    public List<ParameterData> parameterDatas = new List<ParameterData>();
    public ParameterData resultWant;
    public bool isStatic = false;
    //临时参数，当有ref或out 存在时可作为结果参考
    public object[] tempParameter;
    public TestState testState =  TestState.NoTest;
    public string exceptionInfo = "";
}
public class ParameterData
{
    public Type parameterType;
    public string parameterName;
    public object setValue;
    public bool isByRef = false;
}

public enum TestState
{
    NoTest,
    Pass,
    NoPass
}

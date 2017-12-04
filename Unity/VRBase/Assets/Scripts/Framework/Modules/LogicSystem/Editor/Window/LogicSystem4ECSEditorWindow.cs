using HDJ.Framework.Core.ECS;
using HDJ.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    public class LogicSystem4ECSEditorWindow : EditorWindow
    {
        [MenuItem("Tool/Logic System ECS 组件编辑(LS4ECS)")]
        static void OpenWindow()
        {
            GetWindow<LogicSystem4ECSEditorWindow>();
        }
        private Dictionary<string, LogicSystem4ECSData> lS4ECSDataDic;
        private List<ISystem> allSystem = new List<ISystem>();
        private void OnEnable()
        {
            lS4ECSDataDic = LogicSystem4ECSDataController.LS4ECSDataDic;

            Type[] systemTypes = ReflectionUtils.GetChildTypes(typeof(ISystem));
            for (int i = 0; i < systemTypes.Length; i++)
            {
                object obj = ReflectionUtils.CreateDefultInstance(systemTypes[i]);
                ISystem system = (ISystem)obj;
                allSystem.Add(system);
            }
        }
        public static int toolbarOption = 0;
        static public string[] toolbarTexts = { "编辑完成LS4ECS组件", "新建LS4ECS组件", "编辑组件" };


        private void OnGUI()
        {
            toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts);
            switch (toolbarOption)
            {
                case 0:
                    AllDataGUI();

                    break;
                case 1:

                    NewLS4ECSDataGUI();
                    break;
                case 2:
                    EditDataGUI();
                    break;
            }

        }

        private Vector2 pos;
        private void AllDataGUI()
        {
            if (lS4ECSDataDic == null)
                return;
            List<LogicSystem4ECSData> list = new List<LogicSystem4ECSData>(lS4ECSDataDic.Values);

            pos = GUILayout.BeginScrollView(pos, "box");
            foreach (var item in list)
            {
                GUILayout.Label("Name :" + item.name);
                GUILayout.BeginVertical("box");
                GUILayout.Label("ECS系统：");
                foreach (var d in item.systemList)
                {
                    GUILayout.Label("\tSystem:" + d);
                }
                GUILayout.EndVertical();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("删除", GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("警告", "是否删除？" + name, "OK", "Cancel"))
                    {
                        lS4ECSDataDic.Remove(item.name);
                    }

                    return;
                }

                if (GUILayout.Button("编辑", GUILayout.Width(60)))
                {
                    currentEditLS4ECSData = item;
                    toolbarOption = 2;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("保存"))
            {
                LogicSystem4ECSDataController.SaveData();
            }
        }

        private LogicSystem4ECSData currentNewLS4ECSData;
        private ISystem currentSelectSys = null;
        private void NewLS4ECSDataGUI()
        {
            if (currentNewLS4ECSData == null)
                currentNewLS4ECSData = new LogicSystem4ECSData();
            GUILayout.Space(4);
            currentNewLS4ECSData.name = EditorDrawGUIUtil.DrawBaseValue("Name:", currentNewLS4ECSData.name).ToString();
            if (string.IsNullOrEmpty(currentNewLS4ECSData.name) || lS4ECSDataDic.ContainsKey(currentNewLS4ECSData.name))
            {
                EditorGUILayout.HelpBox("名字不能为空或重复", MessageType.Error);
                return;
            }
            GUILayout.BeginVertical("box");
            foreach (var d in currentNewLS4ECSData.systemList)
            {
                GUILayout.Label("\tSystem:" + d);
            }
            GUILayout.EndVertical();



            GUILayout.BeginHorizontal("box");
            GUILayout.BeginVertical("box");
            GUILayout.Label("ECS系统：");
            foreach (var item in allSystem)
            {
                bool contains = false;
                string name = item.GetType().FullName;
                if (currentNewLS4ECSData.systemList.Contains(name))
                {
                    contains = true;
                    GUI.color = Color.red;
                }
                if (GUILayout.Button(name))
                {
                    if (contains)
                    {
                        currentNewLS4ECSData.systemList.Remove(name);
                    }
                    else
                    {
                        currentNewLS4ECSData.systemList.Add(name);
                    }
                    currentSelectSys = item;
                }

                GUI.color = Color.white;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            GUILayout.Label("ECS系统：" + (currentSelectSys == null ? "" : currentSelectSys.GetType().FullName));
            GUILayout.Label("ECS组件：");
            if (currentSelectSys != null)
            {
                foreach (var item in currentSelectSys.FilterComponentNames)
                {
                    GUILayout.Label("\t" + item);
                }

            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("创建"))
            {
                lS4ECSDataDic.Add(currentNewLS4ECSData.name, currentNewLS4ECSData);
                currentNewLS4ECSData = null;
                toolbarOption = 0;
            }
        }

        private LogicSystem4ECSData currentEditLS4ECSData;

        private void EditDataGUI()
        {
            if (currentEditLS4ECSData == null)
                return;
            GUILayout.Space(4);
            currentEditLS4ECSData.name = EditorDrawGUIUtil.DrawBaseValue("Name:", currentEditLS4ECSData.name).ToString();
            if (string.IsNullOrEmpty(currentEditLS4ECSData.name) || (lS4ECSDataDic.ContainsKey(currentEditLS4ECSData.name) && lS4ECSDataDic[currentEditLS4ECSData.name] != currentEditLS4ECSData))
            {
                EditorGUILayout.HelpBox("名字不能为空或重复", MessageType.Error);
                return;
            }
            GUILayout.BeginVertical("box");
            foreach (var d in currentEditLS4ECSData.systemList)
            {
                GUILayout.Label("\tSystem:" + d);
            }
            GUILayout.EndVertical();



            GUILayout.BeginHorizontal("box");
            GUILayout.BeginVertical("box");
            GUILayout.Label("ECS系统：");
            foreach (var item in allSystem)
            {
                bool contains = false;
                string name = item.GetType().FullName;
                if (currentEditLS4ECSData.systemList.Contains(name))
                {
                    contains = true;
                    GUI.color = Color.red;
                }
                if (GUILayout.Button(name))
                {
                    if (contains)
                    {
                        currentEditLS4ECSData.systemList.Remove(name);
                    }
                    else
                    {
                        currentEditLS4ECSData.systemList.Add(name);
                    }
                    currentSelectSys = item;
                }

                GUI.color = Color.white;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            GUILayout.Label("ECS系统：" + (currentSelectSys == null ? "" : currentSelectSys.GetType().FullName));
            GUILayout.Label("ECS组件：");
            if (currentSelectSys != null)
            {
                foreach (var item in currentSelectSys.FilterComponentNames)
                {
                    GUILayout.Label("\t" + item);
                }

            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("完成"))
            {
                currentEditLS4ECSData = null;
                toolbarOption = 0;
            }
        }
    }
}

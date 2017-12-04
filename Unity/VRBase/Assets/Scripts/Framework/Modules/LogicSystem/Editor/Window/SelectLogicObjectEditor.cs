using HDJ.Framework.Utils;
using System;
using UnityEditor;
using UnityEngine;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    public class SelectLogicObjectEditor : EditorWindow
    {
        public static SelectLogicObjectEditor instance;

        public static void OpenWindow(LogicComponentType componentType, ComponentObjectBase components, LogicComponentBase value = null, string componetPathName = "")
        {
            instance = GetWindow<SelectLogicObjectEditor>();
            instance.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);
            instance.value = value;
            instance.components = components;
            if (value == null)
            {
                instance.isNew = true;
            }
            instance.Init(componentType, componetPathName);

        }

        LogicComponentType logicComponetType;
        bool isNew = false;
        ComponentObjectBase components;
        void Init(LogicComponentType componentType, string componetPathName)
        {
            logicComponetType = componentType;
            string logicFileUseType = "";
            if (StateMachineEditorWindow.Instance)
                logicFileUseType = LogicSystemEditorWindow.logicFileUseType;
            componetNameArr = CompontNameAttributeUtils.GetCompontNameAttributeArray(componentType, logicFileUseType);

            if (isNew == false)
            {



                for (int i = 0; i < componetNameArr.Length; i++)
                {
                    if (componetNameArr[i] == componetPathName)
                    {
                        selectInt0 = i;
                        return;
                    }
                }

            }
        }
        private int selectInt0 = 0;
        string[] componetNameArr;
        public LogicComponentBase value = null;
        private EditorExtendBase editorExtendClassNameValue = null;
        void OnGUI()
        {
            if (componetNameArr.Length == 0)
                return;
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();

            selectInt0 = EditorGUILayout.Popup(logicComponetType.ToString(), selectInt0, componetNameArr);

            GUILayout.EndHorizontal();
            GUILayout.Space(15);

            ComponentNameAttribute tn = CompontNameAttributeUtils.GetCompontNameAttribute(logicComponetType, componetNameArr[selectInt0]);
            value = (LogicComponentBase)GetInstance(tn.className, value);
            Type extendEditorType = EditorExtendAttributeUtils.GetEditorExtendType(typeof(EditorExtendBase), value.GetType());// GetEditorExtendType(value.GetType()); 
            if (extendEditorType != null)
            {
                if (editorExtendClassNameValue == null || editorExtendClassNameValue.GetType() != extendEditorType)
                {
                    if (editorExtendClassNameValue != null)
                    {
                        editorExtendClassNameValue.OnClose();
                    }
                    editorExtendClassNameValue = (EditorExtendBase)Activator.CreateInstance(extendEditorType);
                    editorExtendClassNameValue.target = value;
                    editorExtendClassNameValue.OnAwak();
                }
                editorExtendClassNameValue.EditorOverrideClassGUI();
            }
            else
            {
                if (editorExtendClassNameValue != null)
                {
                    editorExtendClassNameValue.OnClose();
                }

                LogicSystemAttributeEditorGUI.DrawInternalVariableGUI(value);
            }
            GUILayout.Space(15);
            if (value != null)
            {
                //Debug.Log(" ValueType  :" + value.GetType() + "  baseType:" + value.GetType().BaseType);
                GUILayout.Label(((LogicComponentBase)value).ToExplain());
            }
            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("确定", GUILayout.Width(120)))
            {

                if (isNew)
                {
                    components.GetLogicComs().Add(value);
                }
                components.SaveComponentDataToClassValue();
                Close();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("取消", GUILayout.Width(120)))
            {
                Close();
            }

            GUILayout.EndHorizontal();
        }
        void OnDestroy()
        {
            if (editorExtendClassNameValue != null)
            {
                editorExtendClassNameValue.OnClose();
                editorExtendClassNameValue = null;
            }
        }

        private object GetInstance(string className, object obj)
        {
            Type t = ReflectionUtils.GetTypeByTypeFullName(className);

            if (obj == null)
                obj = Activator.CreateInstance(t);
            else
            {
                if (t.FullName != obj.GetType().FullName)
                {
                    obj = Activator.CreateInstance(t);
                }
            }

            return obj;
        }

    }
}

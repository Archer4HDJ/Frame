
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    [EditorExtend(typeof(LogicObjectBehaviour))]
    public class LogicObjectBehaviourEditor : StateBehaviourGUIBase
    {
        private static Dictionary<LogicComponentType, LogicComponentBase> copyDataDic = new Dictionary<LogicComponentType, LogicComponentBase>();

        private LogicObjectBehaviour targetObj;
        LogicObject logicObj;
        //是否是开始模块
        private bool isStartModel = false;
        public override void OnInspectorGUI()
        {
            targetObj = (LogicObjectBehaviour)target;
            logicObj = targetObj.logicObj;
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("ID :" + logicObj.id);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //logicObj.name = EditorDrawGUIUtil.DrawBaseValue ("Name", logicObj.name).ToString();
            if (LogicSystemEditorWindow.data != null)
            {
                isStartModel = LogicSystemEditorWindow.data.startId == logicObj.id;
            }
            stateGUI.NormalStateColor = isStartModel ? UnityEditor.Graphs.Styles.Color.Yellow : UnityEditor.Graphs.Styles.Color.Gray;
            if (!isStartModel)
            {
                bool oldState = logicObj.isSupportAlwaysActive;
                logicObj.isSupportAlwaysActive = (bool)EditorDrawGUIUtil.DrawBaseValue("是否支持永远激活状态", logicObj.isSupportAlwaysActive);
                if (oldState != logicObj.isSupportAlwaysActive && logicObj.isSupportAlwaysActive)
                {
                    foreach (var item in stateGUI.FromArrowLines)
                    {
                        MachineDataController.DeleteStateTransitionArrowLine(item);
                    }

                }
                if (logicObj.isSupportAlwaysActive)
                    stateGUI.NormalStateColor = UnityEditor.Graphs.Styles.Color.Red;

                DrawNameAndClassListGUI("事件：", logicObj.events, LogicComponentType.Event);
                DrawNameAndClassListGUI("条件：", logicObj.conditions, LogicComponentType.Condition);
            }
            DrawNameAndClassListGUI("动作：", logicObj.actions, LogicComponentType.Action);

            string tempStr = "";
            foreach (int ch in logicObj.childObjects)
                tempStr += ch + ",";
            GUILayout.Label("Child ID:" + tempStr);
            GUILayout.Label("Pos:" + targetObj.logicObj.editorPos);
            GUILayout.Space(6);
            SaveToBackupGUI();

        }

        private void DrawNameAndClassListGUI(string name, ComponentObjectBase componentObject, LogicComponentType triggerType)
        {
            List<LogicComponentBase> dataList = componentObject.GetLogicComs();
            GUILayout.Space(6);
            GUIStyle style = "flow overlay header lower left";
            GUILayout.BeginHorizontal(style);
            GUILayout.Label(name);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                SelectLogicObjectEditor.OpenWindow(triggerType, componentObject);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical("box");
            for (int i = 0; i < dataList.Count; i++)
            {
                LogicComponentBase value = dataList[i];
                GUILayout.BeginHorizontal();
                string menuName = "";


                ComponentNameAttribute cccc = CompontNameAttributeUtils.GetCompontNameAttributeByClassName(value.GetType().FullName);
                if (cccc != null)
                    menuName = cccc.menuName;
                GUILayout.Label("  " + menuName);
                GUILayout.Label(value.ToExplain());

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit"))
                {
                    SelectLogicObjectEditor.OpenWindow(triggerType, componentObject, dataList[i], menuName);
                }
                if (GUILayout.Button("-", GUILayout.Width(55)))
                {
                    if (EditorUtility.DisplayDialog("提示", "你要删除当前组件吗？", "是", "否"))
                    {
                        dataList.RemoveAt(i);
                        componentObject.SaveComponentDataToClassValue();
                    }
                    return;
                }
                if (GUILayout.Button("▲", GUILayout.Width(55)))
                {
                    if (i - 1 >= 0)
                    {
                        LogicComponentBase temps = dataList[i];
                        dataList.RemoveAt(i);
                        dataList.Insert(i - 1, temps);
                    }
                    return;
                }
                if (GUILayout.Button("复制", GUILayout.Width(55)))
                {
                    if (copyDataDic.ContainsKey(triggerType))
                    {
                        copyDataDic[triggerType] = value;
                    }
                    else
                        copyDataDic.Add(triggerType, value);
                }
                if (copyDataDic.ContainsKey(triggerType))
                {
                    if (GUILayout.Button("粘贴", GUILayout.Width(55)))
                    {
                        dataList[i] = copyDataDic[triggerType];
                        copyDataDic.Remove(triggerType);
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(6);
            }
            GUILayout.EndVertical();
        }

        private bool isBackup = false;
        private string bkName = "";
        private void SaveToBackupGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (!isBackup && GUILayout.Button("保存到备份", GUILayout.Width(120)))
            {
                isBackup = true;
                bkName = logicObj.name;
                Debug.Log("isBackup:" + isBackup);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (isBackup)
            {
                bkName = EditorDrawGUIUtil.DrawBaseValue("备份名字", bkName).ToString();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("确定", GUILayout.Width(100)))
                {
                    isBackup = false;
                    LogicObjectBackUpEditor.Instance.SaveData(bkName, logicObj);
                    StateMachineEditorWindow.Instance.Focus();
                }
                if (GUILayout.Button("取消", GUILayout.Width(100)))
                {
                    isBackup = false;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
    }
}
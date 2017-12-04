using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    [EditorExtend(typeof(Action_Debug_Log))]
    public class EditorAction_Debug_LogExtend : EditorExtendBase
    {
        public override void EditorOverrideClassGUI()
        {
            Action_Debug_Log vClass = (Action_Debug_Log)target;


            GUILayout.BeginVertical("box");
            vClass.isLogInternalVariable = (bool)EditorDrawGUIUtil.DrawBaseValue("是否使用内部变量", vClass.isLogInternalVariable);
            if (vClass.isLogInternalVariable)
            {
                vClass.internalVariableName = LogicSystemAttributeEditorGUI.DrawInternalValueMenu("内部变量", vClass.internalVariableName, null).ToString();
            }
            else
            {
                vClass.message = EditorDrawGUIUtil.DrawBaseValue("打印内容", vClass.message).ToString();
            }
            vClass.logType = (LogType)EditorDrawGUIUtil.DrawBaseValue("LogType", vClass.logType);
            GUILayout.EndVertical();

        }
    }
}

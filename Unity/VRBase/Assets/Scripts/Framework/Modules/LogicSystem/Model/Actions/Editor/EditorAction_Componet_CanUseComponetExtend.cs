using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    [EditorExtend(typeof(Action_System_ComponetControl))]
    public class EditorAction_System_ComponetControlExtend : EditorExtendBase
    {

        public override void EditorOverrideClassGUI()
        {
            Action_System_ComponetControl obj = (Action_System_ComponetControl)target;
            obj.componetIdList = (List<int>)EditorDrawGUIUtil.DrawBaseValue("组件ID", obj.componetIdList);

            obj.enableEvent = (bool)EditorDrawGUIUtil.DrawBaseValue("组件使用事件", obj.enableEvent);
            obj.enableCondition = (bool)EditorDrawGUIUtil.DrawBaseValue("组件使用条件", obj.enableCondition);
            obj.enableAction = (bool)EditorDrawGUIUtil.DrawBaseValue("组件使用动作", obj.enableAction);
            obj.runChild = (bool)EditorDrawGUIUtil.DrawBaseValue("组件使用子集", obj.runChild);

        }
    }
}
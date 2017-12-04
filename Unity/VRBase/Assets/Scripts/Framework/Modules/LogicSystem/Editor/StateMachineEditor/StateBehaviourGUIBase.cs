using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    public abstract class StateBehaviourGUIBase
    {
        public MachineStateGUI stateGUI;
        public object target;
        public virtual void OnInspectorGUI()
        {
            target = EditorDrawGUIUtil.DrawClassData("", target);
        }
    }
}

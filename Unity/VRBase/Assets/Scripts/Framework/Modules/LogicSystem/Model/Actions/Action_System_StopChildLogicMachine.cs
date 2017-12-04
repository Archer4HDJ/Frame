using System;
namespace HDJ.Framework.Game.LogicSystem
{
    [ComponentName(LogicComponentType.Action, "系统/停止子逻辑状态机")]
    [Serializable]
    public class Action_System_StopChildLogicMachine : ActionComponentBase
    {
        public string controlName = "";

        protected override void Action()
        {
            logicObject.logicManager.StopChildLogicMachine(controlName);
        }
    }
}
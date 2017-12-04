using System;
namespace HDJ.Framework.Game.LogicSystem
{
    [ComponentName(LogicComponentType.Action, "系统/添加子逻辑状态机")]
    [Serializable]
    public class Action_System_AddChildLogicMachine : ActionComponentBase
    {
        public string controlName = "";
        public string logicMachineFileName = "";

        protected override void Action()
        {
            LogicRuntimeMachine lom = LogicSystemManager.NewLogicRuntimeMachine(logicMachineFileName);
            logicObject.logicManager.AddChildLogicMachine(controlName, lom.Id);
            lom.Start();
        }
    }
}
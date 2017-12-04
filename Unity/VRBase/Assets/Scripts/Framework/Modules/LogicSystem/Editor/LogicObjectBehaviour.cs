
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    public class LogicObjectBehaviour : StateBaseBehaviour
    {
        public LogicObject logicObj = new LogicObject();

#if UNITY_EDITOR
        public MachineState state;

#endif

    }
}
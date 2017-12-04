
namespace HDJ.Framework.Game.LogicSystem
{
    public class ActionComponentBase : LogicComponentBase
    {
        public void RunAction()
        {
            UpdateInternalValue();
            Action();
        }
        protected virtual void Action() { }


    }
}

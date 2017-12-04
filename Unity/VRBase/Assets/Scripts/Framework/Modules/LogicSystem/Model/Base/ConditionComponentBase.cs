namespace HDJ.Framework.Game.LogicSystem
{
    public class ConditionComponentBase : LogicComponentBase
    {
        public bool CompareCondition(params object[] objs)
        {
            UpdateInternalValue();
            return Compare(objs);
        }
        protected virtual bool Compare(params object[] objs)
        {
            return true;
        }
    }
}

using System;
using System.Collections.Generic;

namespace HDJ.Framework.Game.LogicSystem
{
    [Serializable]
    public class ConditionsObject : ComponentObjectBase
    {

        private List<ConditionComponentBase> conditionComponentObjs = new List<ConditionComponentBase>();
        public void Init(LogicObject logicObject)
        {
            for (int i = 0; i < GetLogicComs().Count; i++)
            {
                ConditionComponentBase td = (ConditionComponentBase)GetLogicComs()[i];
                td.Initialize(logicObject);
                conditionComponentObjs.Add(td);
            }
        }
        public void OnPause(bool isPause)
        {
            for (int i = 0; i < conditionComponentObjs.Count; i++)
            {
                conditionComponentObjs[i].OnPause(isPause);
            }

        }
        public bool ConditionCompare(params object[] objs)
        {
            if (!enable)
                return false;
            for (int i = 0; i < conditionComponentObjs.Count; i++)
            {
                ConditionComponentBase td = conditionComponentObjs[i];
                if (!td.CompareCondition(objs)) return false;

            }
            return true;
        }
        public void Close()
        {
            for (int i = 0; i < conditionComponentObjs.Count; i++)
            {
                conditionComponentObjs[i].OnClose();
            }

            conditionComponentObjs.Clear();
        }
    }
}
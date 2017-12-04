
using System;
using System.Collections.Generic;

namespace HDJ.Framework.Game.LogicSystem
{
    [Serializable]
    public class ActionsObject : ComponentObjectBase
    {

        private List<ActionComponentBase> actionComponentObjs = new List<ActionComponentBase>();

        public void Init(LogicObject logicObject)
        {
            for (int i = 0; i < GetLogicComs().Count; i++)
            {
                ActionComponentBase td = (ActionComponentBase)GetLogicComs()[i];
                td.Initialize(logicObject);
                actionComponentObjs.Add(td);
            }
        }

        public void OnPause(bool isPause)
        {
            for (int i = 0; i < actionComponentObjs.Count; i++)
            {
                actionComponentObjs[i].OnPause(isPause);
            }
        }

        public void RunActions()
        {
            if (!enable)
                return;
            for (int i = 0; i < actionComponentObjs.Count; i++)
            {
                actionComponentObjs[i].RunAction();
            }
        }

        public void Close()
        {
            for (int i = 0; i < actionComponentObjs.Count; i++)
            {
                actionComponentObjs[i].OnClose();
            }
            actionComponentObjs.Clear();
        }

    }
}

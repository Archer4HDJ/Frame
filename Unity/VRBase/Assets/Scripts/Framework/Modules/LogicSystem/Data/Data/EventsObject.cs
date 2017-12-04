using System;
using System.Collections.Generic;

namespace HDJ.Framework.Game.LogicSystem
{
    [Serializable]
    public class EventsObject : ComponentObjectBase
    {
        private List<EventComponentBase> eventComponentObjs = new List<EventComponentBase>();

        public void Init(LogicObject logicObject)
        {
            for (int i = 0; i < GetLogicComs().Count; i++)
            {
                EventComponentBase td = (EventComponentBase)GetLogicComs()[i];
                td.Initialize(logicObject);
                eventComponentObjs.Add(td);
            }
        }
        public void OnPause(bool isPause)
        {
            for (int i = 0; i < eventComponentObjs.Count; i++)
            {
                eventComponentObjs[i].OnPause(isPause);
            }

        }
        public void Close()
        {
            for (int i = 0; i < eventComponentObjs.Count; i++)
            {
                eventComponentObjs[i].OnClose();
            }
            eventComponentObjs.Clear();
        }


    }
}
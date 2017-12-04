using HDJ.Framework.Modules;
using System;
using System.Collections.Generic;

namespace HDJ.Framework.Game.LogicSystem
{
    public class ComponentObjectBase
    {

        [NonSerialized]
        public bool enable = true;
        public int Count { get { return data.Count; } }
        public List<ClassValue> data = new List<ClassValue>();

        private List<LogicComponentBase> logicComs;
        public List<LogicComponentBase> GetLogicComs()
        {
            if (logicComs == null)
            {
                logicComs = new List<LogicComponentBase>();
                for (int i = 0; i < data.Count; i++)
                {
                    object temp = data[i].GetValue();
                    LogicComponentBase td = (LogicComponentBase)temp;
                    if (td != null)
                    {
                        logicComs.Add(td);
                    }
                }
            }

            return logicComs;
        }

        public void SetLogicComs(List<LogicComponentBase> list)
        {
            logicComs = list;
        }
        public void SaveComponentDataToClassValue()
        {
            data.Clear();
            if (logicComs == null)
                return;
            foreach (var item in logicComs)
            {
                data.Add(new ClassValue(item));
            }
        }


    }
}

using HDJ.Framework.Core;
using HDJ.Framework.Core.ECS;
using HDJ.Framework.Modules;
using System.Collections.Generic;


namespace HDJ.Framework.Game.LogicSystem
{
    [ComponentName(LogicComponentType.Action, "ECS/ECS_Model")]
    [System.Serializable]
    public class Action_ECS_ECSControl : ActionComponentBase
    {
        public string LS4ECS_Name;
        public List<ClassValue> ECSComponentData = new List<ClassValue>();

        protected override void Action()
        {
           Entity entity =  WorldManager.CurrentRunWorld.CreateEntity();

            for (int i = 0; i < ECSComponentData.Count; i++)
            {
                ClassValue cv = ECSComponentData[i];
                IComponent component = (IComponent)cv.GetValue();
                if (component != null)
                {
                    entity.AddComponent(component);
                }

            }
        }

    }
}


using HDJ.Framework.Core.ECS;
using HDJ.Framework.Modules;
using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    [EditorExtend(typeof(Action_ECS_ECSControl))]
    public class EditorAction_ECS_ECSControlExtend : EditorExtendBase
    {
        private Dictionary<string, LogicSystem4ECSData> lS4ECSDataDic;
        private Dictionary<string, ISystem> allSystem = new Dictionary<string, ISystem>();
        public override void OnAwak()
        {
            ecs = (Action_ECS_ECSControl)target;
            lS4ECSDataDic = LogicSystem4ECSDataController.LS4ECSDataDic;

            allSystem.Clear();
            Type[] systemTypes = ReflectionUtils.GetChildTypes(typeof(ISystem));
            for (int i = 0; i < systemTypes.Length; i++)
            {
                object obj = ReflectionUtils.CreateDefultInstance(systemTypes[i]);
                ISystem system = (ISystem)obj;
                allSystem.Add(system.GetType().FullName, system);
            }
            if (!string.IsNullOrEmpty(ecs.LS4ECS_Name) && lS4ECSDataDic.ContainsKey(ecs.LS4ECS_Name))
            {
                LS4ECS_Name = ecs.LS4ECS_Name;
                InitCompents(LS4ECS_Name);
            }
        }

        Dictionary<string, object> componentData = new Dictionary<string, object>();
        Action_ECS_ECSControl ecs;
        private string LS4ECS_Name;

        private Vector2 pos;
        public override void EditorOverrideClassGUI()
        {


            List<string> list = new List<string>(lS4ECSDataDic.Keys);

            LS4ECS_Name = EditorDrawGUIUtil.DrawPopup("LS4ECS_Name :", LS4ECS_Name, list, (str) =>
              {
                  InitCompents(str);
              });

            List<string> tempKeys = new List<string>(componentData.Keys);
            pos = GUILayout.BeginScrollView(pos, "box");
            foreach (var item in tempKeys)
            {
                componentData[item] = EditorDrawGUIUtil.DrawBaseValue("ECS组件：" + item, componentData[item]);

                //ClassValue classValue = new ClassValue(componentData[item]);
                //ecs.ECSComponentData.Add(classValue);
            }
            GUILayout.EndScrollView();

        }
        public override void OnClose()
        {

            ecs.ECSComponentData.Clear();
            ecs.LS4ECS_Name = LS4ECS_Name;
            foreach (var item in componentData.Keys)
            {
                ClassValue classValue = new ClassValue(componentData[item]);
                ecs.ECSComponentData.Add(classValue);
            }
        }

        private void InitCompents(string LS4ECSName0)
        {
            Debug.Log("InitCompents");
            componentData.Clear();
            if (LS4ECSName0 == ecs.LS4ECS_Name)
            {
                foreach (var c in ecs.ECSComponentData)
                {
                    if (componentData.ContainsKey(c.ScriptName))
                    {
                        Debug.LogError("key already exists : " + c.ScriptName);
                        continue;
                    }
                    componentData.Add(c.ScriptName, c.GetValue());
                }

            }
            else
            {
                LogicSystem4ECSData d = lS4ECSDataDic[LS4ECSName0];
                foreach (var item in d.systemList)
                {
                    ISystem system = allSystem[item];
                    foreach (var c in system.FilterComponentTypes)
                    {

                        string name = c.FullName;
                        if (componentData.ContainsKey(name))
                            continue;
                        object obj = ReflectionUtils.CreateDefultInstance(c);
                        componentData.Add(name, obj);
                    }
                }
            }
        }
    }
}

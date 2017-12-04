using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;
using HDJ.Framework.Utils;
using UnityEditor;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    public static class StateMachineUtils
    {
        public static Vector2 MousePos2MachineGridPos(Vector2 mPos)
        {
            Vector2 temp = mPos - StateMachineBGGUI.controlWindowRange.position;
            return temp - StateMachineBGGUI.bgRect.position;
            // return mPos + new Vector2(StateMachineBGGUI.extraLenth, StateMachineBGGUI.extraLenth);
        }

        public static bool MachineGridRectContainsMousePos(Vector2 mousePos, Rect gridRect)
        {
            Vector2 temp = MousePos2MachineGridPos(mousePos);
            return gridRect.Contains(temp);
        }

        public static Color GetColorByState(MachineStateUIStateEnum state)
        {
            Color c = Color.white;
            switch (state)
            {
                case MachineStateUIStateEnum.Normal:
                    c = Color.white;
                    break;
                case MachineStateUIStateEnum.Running:
                    c = Color.green;
                    break;
                case MachineStateUIStateEnum.select:
                    c = Color.blue;
                    break;
            }
            return c;
        }
        public static Styles.Color GetStyleColorByState(MachineStateUIStateEnum state)
        {
            Styles.Color c = Styles.Color.Gray;
            switch (state)
            {
                case MachineStateUIStateEnum.Normal:
                    c = Styles.Color.Gray;
                    break;
                case MachineStateUIStateEnum.Running:
                    c = Styles.Color.Green;
                    break;
                case MachineStateUIStateEnum.select:
                    c = Styles.Color.Blue;
                    break;
            }
            return c;
        }

        public static Type GetSelectObjectInspectorType(Type inspectorType)
        {
            Type[] types = ReflectionUtils.GetChildTypes(typeof(SelectObjectInspectorBase));
            foreach (var item in types)
            {
                object[] atts = item.GetCustomAttributes(false);
                foreach (var att in atts)
                {
                    if (att is CustomInspectorEditor)
                    {
                        CustomInspectorEditor temp = (CustomInspectorEditor)att;
                        if (temp.InspectorEditorType == inspectorType)
                            return item;
                    }
                }
            }

            return null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    public static class SelectObjectController
    {

        private static Object selectObject;

        public static Object SelectObject
        {
            get
            {
                return selectObject;
            }
        }

        public static bool IsSelectThis(Object item)
        {
            if (selectObject == null)
                return false;
            return item.Equals(selectObject);
        }

        public static void SelectItemObject(Object item)
        {
            SelectObjectCancel();
            selectObject = item;
            if (selectObject is MachineStateGUI)
            {
                ((MachineStateGUI)selectObject).uiState = MachineStateUIStateEnum.select;
            }
            else if (selectObject is StateTransitionArrowLine)
            {
                ((StateTransitionArrowLine)selectObject).uiState = MachineStateUIStateEnum.select;

            }
        }
        public static void SelectObjectCancel()
        {
            if (selectObject != null)
            {
                if (selectObject is MachineStateGUI)
                {
                    ((MachineStateGUI)selectObject).uiState = MachineStateUIStateEnum.Normal;
                }
                else if (selectObject is StateTransitionArrowLine)
                {
                    ((StateTransitionArrowLine)selectObject).uiState = MachineStateUIStateEnum.Normal;
                }
            }
            selectObject = null;
        }

        public static void DeleteSelectObjet()
        {
            if (selectObject != null)
            {
                if (selectObject is MachineStateGUI)
                {
                    MachineDataController.DeleteMachineStateGUI(((MachineStateGUI)selectObject));
                }
                else if (selectObject is StateTransitionArrowLine)
                {
                    MachineDataController.DeleteStateTransitionArrowLine(((StateTransitionArrowLine)selectObject));
                }
            }
        }
    }
}

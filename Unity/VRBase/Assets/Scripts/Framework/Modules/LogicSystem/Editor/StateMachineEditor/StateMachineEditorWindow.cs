using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Profiling;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    public class StateMachineEditorWindow : EditorWindow
    {
        public static CallBack OnDrawLeftPartGUI;
        public static CallBack OnDrawTopToolBarGUI;
        public static CallBack<MachineStateGUI> OnCreateMachineStateGUI;
        public static CallBack<MachineStateGUI> OnDeleteMachineStateGUI;
        public static CallBack<StateTransition> OnAddStateTransition;
        public static CallBack<StateTransition> OnDeleteStateTransition;
        public static CallBack<StateTransitionArrowLine> OnDeleteStateTransitionArrowLine;
        public static CallBack OnDestroyWindow;

        public static StateMachineEditorWindow Instance;
        [MenuItem("Tool/StateMachineEditorWindow")]
        public static void OpenWindow()
        {
            if (Instance == null)
            {
                Instance = GetWindow<StateMachineEditorWindow>();
                Instance.Show();
                Instance.Focus();
            }
            Instance.OnDestroy();
        }
        void OnEnable()
        {
            Instance = this;
        }
        Rect ndoeControlRange;
        void OnGUI()
        {
            MachineStateInputEventController.PlayerControlUse();
            DrawLeftToolAreaGUI();
            DrawToolBarGUI();
            DrawRightToolAreaGUI();
            ndoeControlRange = new Rect(leftToolAreaRect.xMax, topRightToolBarRect.yMax, Screen.width - leftToolAreaRect.width - RightToolAreaWith, Screen.height - topRightToolBarRect.height);

            StateMachineBGGUI.BeginGUI(ndoeControlRange, StateMachineEditorGUI.stateMachineMaxRange);
            MachineStateInputEventController.OnMachineStateMouseRightClickMenu();
            BeginWindows();
            StateTransitionGUI.DrawTempArrowTransition();
            StateMachineEditorGUI.DrawAllStateMachineGUI();

            EndWindows();
            StateMachineBGGUI.EndGUI();
        }

        public Rect leftToolAreaRect;
        private float leftToolAreaRect_with = 350;
        public float LeftToolAreaRect_with
        {
            get
            {
                return leftToolAreaRect_with;
            }

            set
            {
                leftToolAreaRect_with = Mathf.Clamp(value, 300, Screen.width - RightToolAreaWith);
            }
        }
        void DrawLeftToolAreaGUI()
        {
            leftToolAreaRect = new Rect(0, 0, leftToolAreaRect_with, Screen.height);

            GUILayout.BeginArea(leftToolAreaRect, Styles.graphBackground);
            if (OnDrawLeftPartGUI != null)
                OnDrawLeftPartGUI();
            GUILayout.EndArea();

        }

        public Rect topRightToolBarRect;
        void DrawToolBarGUI()
        {
            topRightToolBarRect = new Rect(leftToolAreaRect.xMax, 0, Screen.width - leftToolAreaRect.width, 25f);

            GUIStyle style = "flow overlay header lower left";
            GUILayout.BeginArea(topRightToolBarRect, style);
            GUILayout.BeginHorizontal(style);

            if (OnDrawTopToolBarGUI != null)
                OnDrawTopToolBarGUI();

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public Rect rightToolAreaRect;
        public int RightToolAreaWith = 300;
        private SelectObjectInspectorBase editorObject;
        void DrawRightToolAreaGUI()
        {
            rightToolAreaRect = new Rect(Screen.width - RightToolAreaWith, 0, RightToolAreaWith, Screen.height);
            GUILayout.BeginArea(rightToolAreaRect, Styles.graphBackground);

            if (SelectObjectController.SelectObject != null)
            {
                Type editorType = StateMachineUtils.GetSelectObjectInspectorType(SelectObjectController.SelectObject.GetType());
                if (editorObject == null || editorObject.GetType() != editorType || editorObject.target != SelectObjectController.SelectObject)
                {
                    editorObject = (SelectObjectInspectorBase)ReflectionUtils.CreateDefultInstance(editorType);
                }
                editorObject.target = SelectObjectController.SelectObject;

                editorObject.DrawHeaderGUI();

                editorObject.DrawInspectorGUI();
            }
            GUILayout.EndArea();

        }

        void OnDestroy()
        {
            OnDrawLeftPartGUI = null;
            OnDrawTopToolBarGUI = null;
            OnCreateMachineStateGUI = null;
            OnDeleteMachineStateGUI = null;
            OnAddStateTransition = null;
            OnDeleteStateTransition = null;
            OnDeleteStateTransitionArrowLine = null;
            MachineDataController.ClearAllData();

            if (OnDestroyWindow != null)
                OnDestroyWindow();
        }
    }
}
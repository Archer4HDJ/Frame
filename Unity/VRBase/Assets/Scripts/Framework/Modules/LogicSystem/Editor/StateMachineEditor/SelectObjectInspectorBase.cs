using UnityEditor;
using UnityEngine;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    public class SelectObjectInspectorBase
    {
        public Object target;
        public void DrawHeaderGUI()
        {
            GUILayout.Space(4);
            GUILayout.BeginVertical("box");
            OnHeaderGUI();
            GUILayout.EndVertical();
        }

        protected virtual void OnHeaderGUI()
        {

        }
        public void DrawInspectorGUI()
        {
            GUILayout.Space(4);
            GUILayout.BeginVertical("box");
            OnInspectorGUI();
            GUILayout.EndVertical();
            EditorUtility.SetDirty(target);
        }

        protected virtual void OnInspectorGUI()
        {

        }
    }
}
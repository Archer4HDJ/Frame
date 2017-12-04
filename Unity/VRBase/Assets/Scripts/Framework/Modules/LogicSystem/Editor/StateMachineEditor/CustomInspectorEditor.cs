using UnityEngine;
using UnityEditor;
using System;

namespace HDJ.Framework.Game.LogicSystem.Editor
{
    public sealed class CustomInspectorEditor : Attribute
    {
        private Type inspectorEditorType;
        public CustomInspectorEditor(Type inspectorEditorType)
        {
            this.inspectorEditorType = inspectorEditorType;
        }

        public Type InspectorEditorType
        {
            get
            {
                return inspectorEditorType;
            }

        }
    }
}
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    public class EditorExtendBase
    {

        public object target;
        public virtual void OnAwak() { }

        public virtual void EditorOverrideClassGUI()
        {
            EditorDrawGUIUtil.DrawClassData("", target);
        }

        public virtual void OnClose() { }

    }
}

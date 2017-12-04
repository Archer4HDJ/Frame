using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace HDJ.Framework.Editor
{
   public class ECSViewWindow : EditorWindow
    {
        [MenuItem("Tool/ECS Entity Window(2000)", priority = 2000)]
        static void OpenWindow()
        {
            GetWindow<ECSViewWindow>().Init();
        }

        private void OnEnable()
        {
            Init();
        }
        private void Init()
        {
           
        }
    }
}

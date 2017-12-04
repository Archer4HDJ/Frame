using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateMonoBehaviourExtendEditor  {
    const string ScriptTemplateName = "81-BehaviourExtend-NewBehaviourExtendScript.cs.txt";

    [InitializeOnLoadMethod]
    static void CreateMonoBehaviourExtend()
    {
        Type t = HDJ.Framework.Utils.ReflectionUtils.GetTypefromAssemblyFullName("UnityEditor.Graphs", "UnityEditor.Graphs.AnimatorControllerTool");
        string s = t.Module.FullyQualifiedName;
       s= PathUtils.CutPath(s, "Data", false, true);
        s += "/Resources/ScriptTemplates/81-BehaviourExtend-NewBehaviourExtendScript.cs.txt";
        Debug.Log(s);
        if (!File.Exists(s))
        {
            TextAsset ta = Resources.Load(PathUtils.RemoveExtension(ScriptTemplateName)) as TextAsset;
            if (ta == null || string.IsNullOrEmpty(ta.text))
            {
                Debug.LogError("脚本模板不见了！！ ScriptTemplateName: " + ScriptTemplateName);
                return;
            }

            FileUtils.CreateTextFile(s, ta.text);
            AssetDatabase.Refresh();
        }
    }
	
	
}

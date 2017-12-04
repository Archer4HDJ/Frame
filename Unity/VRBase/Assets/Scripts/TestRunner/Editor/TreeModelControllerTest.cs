using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using HDJ.Framework.Utils;

public class TreeModelControllerTest {

	[Test]
	public void TreeModelControllerTreeTestSimplePasses() {
        // Use the Assert class to test conditions.
        string[] paths = PathUtils.GetDirectoryFilePath("Assets/Resources");
        paths = PathUtils.RemovePathWithEnds(paths, new string[] { ".meta" });
        TreeModelController<TreeNodeBase> control = new TreeModelController<TreeNodeBase>();
        foreach (var i in paths)
        {
            TreeNodeBase node = control.GetNewNode(i);
            control.AddNode(node);
        }

        control.TreeForeachNode((n) =>
        {
            Debug.Log(n.Id + "  Path: " + n.relativeRootPath);
            return true;
        });
    }
    [Test]
    public void TreeModelControllerTestListSimplePasses()
    {
        // Use the Assert class to test conditions.
        string[] paths = PathUtils.GetDirectoryFilePath("Assets/Resources");
        paths = PathUtils.RemovePathWithEnds(paths, new string[] { ".meta" });
        TreeModelController<TreeNodeBase> control = new TreeModelController<TreeNodeBase>();
        foreach (var i in paths)
        {
            TreeNodeBase node = control.GetNewNode(i);
            control.AddNode(node);
        }

        control.ListForeachNode((n) =>
        {
            string c = "[";
            foreach (var item in n.childs)
            {
                c += item + ",";
            }
            c += "]";
            Debug.Log(n.Id + " "+c+"  Path: " + n.relativeRootPath);
            return true;
        });
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
	public IEnumerator TreeModelControllerTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}

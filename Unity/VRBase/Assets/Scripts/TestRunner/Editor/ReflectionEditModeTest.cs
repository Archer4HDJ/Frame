using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using HDJ.Framework.Core.ECS;
using System.Collections.Generic;
using HDJ.Framework.Utils;

public class ReflectionEditModeTest {

	[Test]
	public void ReflectionEditModeTestSimplePasses() {
        // Use the Assert class to test conditions.
        //Debug.Log(typeof(IComponent).IsAssignableFrom(typeof(Ttt)));
        //Debug.Log(typeof(Singleton<>).IsAssignableFrom(typeof(Ttt)));
        //Debug.Log((typeof(Ttt).IsSubclassOf(typeof( Singleton<>))));
        //Debug.Log((typeof(Ttt).Name));
        //Debug.Log((typeof(Ttt).FullName));
        //Debug.Log((typeof(Ttt).BaseType.Name));
        //Debug.Log((typeof(Ttt).BaseType.FullName));
        //Debug.Log((typeof(Singleton<>).Name));
        //Debug.Log((typeof(Singleton<>).FullName));
        //Debug.Log((typeof(Singleton<Ttt>).Name));
        //Debug.Log((typeof(Singleton<Ttt>).FullName));
        //Debug.Log((typeof(List<Ttt>).Name));
        //Debug.Log((typeof(List<Ttt>).FullName));
        //Debug.Log((typeof(List<>).Name));
        //Debug.Log((typeof(List<>).FullName));
        IComponent obj = (IComponent)ReflectionUtils.GetPropertyInfo(typeof(Ttt).BaseType, null, "Instance");
        Debug.Log(obj ==null);
    }

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator ReflectionEditModeTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}
public class Ttt : Singleton<Ttt>, IComponent
{

}
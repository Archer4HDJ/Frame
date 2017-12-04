using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using HDJ.Framework.Utils;

public class JsonEditModeTest {

	[Test]
	public void JsonEditModeTestSimplePasses() {
        // Use the Assert class to test conditions.
        TetsJson t = new TetsJson();
        Debug.Log(JsonUtils.ClassOrStructToJson(t));
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator JsonEditModeTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}

    public class TetsJson
    {
        [NotJsonSerialized]
        public string sss="555";

        public int ttt = 100;


        private float testfloat = 23.23f;
        [NotJsonSerialized]
        public float Testfloat
        {
            get
            {
                return testfloat;
            }

            set
            {
                testfloat = value;
            }
        }
    }
}

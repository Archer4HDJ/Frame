using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectUtils  {

	public static T LoadCreateScriptableObject<T>(string assetPath) where T: ScriptableObject
    {
      T  msData = AssetDatabase.LoadAssetAtPath<T>(assetPath);

        if (msData == null)
        {
            msData = ScriptableObject.CreateInstance<T>();
            string temp = Path.GetDirectoryName(assetPath);
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }
            AssetDatabase.CreateAsset(msData, assetPath);
            AssetDatabase.Refresh();
        }

        return msData;
    }


}

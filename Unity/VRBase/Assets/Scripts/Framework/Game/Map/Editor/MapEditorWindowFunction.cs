using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEditorWindowFunction  {

    private const string MapScenePath = "Assets/Scenes";
    //[MenuItem("Tool/场景检查", priority = 1001)]
    public static void CreateMapScene()
    {
        string tempPath = MapScenePath + "/Map.unity";
        if (!Directory.Exists(MapScenePath))
        {
            Directory.CreateDirectory(MapScenePath);
            AssetDatabase.Refresh();
        }
       if(!File.Exists(tempPath))
        {
            Scene s = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EditorSceneManager.SaveScene(s, tempPath);
            AssetDatabase.Refresh();
        }
        else
        {
            EditorSceneManager.OpenScene(tempPath);
        }
        
    }
   // [MenuItem("Tool/BakeLightMap", priority = 1001)]
    public static void BakeLightMap(Lightmapping.OnCompletedFunction complete)
    {
        string tempPath = MapScenePath + "/Map";
        if (Directory.Exists(tempPath))
        {
            Directory.Delete(tempPath,true);
            AssetDatabase.Refresh();
        }
        //Debug.Log("giWorkflowMode :" + Lightmapping.giWorkflowMode);
        Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
        Lightmapping.completed = complete;
        Lightmapping.Clear();
        Lightmapping.Bake();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorUpdater : EditorMonoBehaviour
{
    private double updateTime = 1f;
    private double temp = 0;
    public override void Update()
    {
        if (MapEditorWindow.instance)
        {
            if (temp > 0)
            {
                temp -= deltaTime;
                return;
            }
            temp = updateTime;

            

            GameObject[] objs = Object.FindObjectsOfType<GameObject>();
            reflectionProbeNames.Clear();
            foreach (GameObject obj in objs)
            {

                CkeckReflectionProbeName(obj);

                if (obj.layer != LayerMask.NameToLayer(MapPrefabSetUpdater.MapObjectLayer))
                    continue;
                CheckName(obj);
                CheckParent(obj.transform, MapEditorWindow.instance.mapNameRoot, MapEditorWindow.instance.mapLightRoot);

              
            }
        }
    }
 
    private void CheckParent(Transform obj,Transform mapParent, Transform lightParent)
    {
        if (obj.parent != null )
            return;
        if (obj.GetComponent<Light>() != null ||
            obj.GetComponent<LightProbeGroup>() != null ||
            obj.GetComponent<ReflectionProbe>() != null)
        {
            obj.SetParent(lightParent);
        }
        else
        {
            obj.SetParent(mapParent);
        }
    }
    private void CheckName(GameObject obj)
    {
        if (obj.name.Contains("(") && obj.name.Contains(")"))
        {
            int id = obj.name.IndexOf('(');
            int idend = obj.name.IndexOf(')');
            string temp = obj.name.Remove(id, idend - id + 1);
            temp = temp.Trim();
            obj.name = temp;
        }
    }
    List<string> reflectionProbeNames = new List<string>();
    //检查ReflectionProbe物体的名字不重复，没有空格
    private void CkeckReflectionProbeName(GameObject obj)
    {
        ReflectionProbe refPeob = obj.GetComponent<ReflectionProbe>();
        if (refPeob)
        {
            string name = obj.name;
            while (name.Contains(" "))
            {
                name= name.Replace(" ", "");
            }
            while (reflectionProbeNames.Contains(name))
            {
                name += "_" + reflectionProbeNames.Count;
            }
            reflectionProbeNames.Add(name);
            obj.name = name;
        }
    }
}

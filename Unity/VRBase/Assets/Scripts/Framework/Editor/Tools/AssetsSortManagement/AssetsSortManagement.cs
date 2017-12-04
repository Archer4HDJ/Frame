using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 资源分类
/// </summary>
public static class AssetsSortManagement  {
    const string AssetsLayersTagsDataPath = "Assets/Resources/Others/AssetsSort/LayersTagsData.asset";


   static AssetsLayersTagsData layersTagsData;
   static public void Init()
    {
        if (layersTagsData == null)
            layersTagsData = ScriptableObjectUtils.LoadCreateScriptableObject<AssetsLayersTagsData>(AssetsLayersTagsDataPath);

    }

    public static List<string> Layers { get { return layersTagsData.LayersList; } }

    public static List<string> Tags { get { return layersTagsData.tagsList; } }
    public static List<LayersTagsData> LayersTagsDataList { get { return layersTagsData.LayersTagsDataList; } }
    public static  bool AddLayer(string name)
    {
        if (layersTagsData.LayersList.Contains(name))
            return false;
        layersTagsData.LayersList.Add(name);
        return true;
    }
    static public bool AddTags(string name)
    {
        if (layersTagsData.tagsList.Contains(name))
            return false;
        layersTagsData.tagsList.Add(name);
        return true;
    }
    static public bool RemoveLayer(string name)
    {
        if (!layersTagsData.LayersList.Contains(name))
            return false;

        layersTagsData.LayersList.Remove(name);
        layersTagsData.ReplaceLayersName(name, "None");
        return true;
    }
    static public bool RemoveTag(string name)
    {
        if (!layersTagsData.tagsList .Contains(name))
            return false;

        layersTagsData.tagsList.Remove(name);
        layersTagsData.ReplaceTagsName (name, "None");
        return true;
    }

    static public void ReNameLayerName(string oldName,string newName)
    {
        if (layersTagsData.LayersList.Contains(oldName))
        {
          int i=  layersTagsData.LayersList.IndexOf(oldName);
            layersTagsData.LayersList[i] = newName;
            layersTagsData.ReplaceLayersName(oldName, newName);
        }
    }
    static public void ReNameTagName(string oldName, string newName)
    {
        if (layersTagsData.tagsList.Contains(oldName))
        {
            int i = layersTagsData.tagsList.IndexOf(oldName);
            layersTagsData.tagsList[i] = newName;
            layersTagsData.ReplaceTagsName(oldName, newName);
        }
    }

    static public void AddNewDefultLayerTag(string name)
    {
        if (!layersTagsData.ExistInLayersTagsDataList(name))
        {
            layersTagsData.AddNewLayersTagsData(name);
        }
    }
    static public LayersTagsData GetLayersTagsData(string name)
    {
      return  layersTagsData.GetLayersTagsData(name);
    }

    static public void SetLayerTag(string name,string layer,string tag)
    {
        LayersTagsData lt = GetLayersTagsData(name);
        if (lt == null || !Layers.Contains(layer) || !Tags.Contains(tag))
            return;
        lt.layer = layer;
        lt.tag = tag;
    }
    static public void SetLayer(string name, string layer)
    {
        LayersTagsData lt = GetLayersTagsData(name);
        if (lt == null || !Layers.Contains(layer))
        {
            Debug.Log("Name: " + name + " Data: " + lt + "  Layers.Contains(layer):"+ Layers.Contains(layer));
            return;
        }
        lt.layer = layer;
       
    }
    static public void SetLayerTag(string name,  string tag)
    {
        LayersTagsData lt = GetLayersTagsData(name);
        if (lt == null  || !Tags.Contains(tag))
            return;
        lt.tag = tag;
    }

    static public void DeleteLayersTagsItem(string name)
    {
        layersTagsData.DeleteLayersTagsItem(name);
    }


    static public string[] GetNamesByLayer(string layer)
    {
        List<string> temp = new List<string>();
        if (!string.IsNullOrEmpty(layer))
        {
            foreach (var item in LayersTagsDataList)
            {
                if (item.layer == layer)
                    temp.Add(item.name);
            }
        }
        return temp.ToArray();
    }
    static public string[] GetNamesByTag(string tag)
    {
        List<string> temp = new List<string>();
        if (!string.IsNullOrEmpty(tag))
        {
            foreach (var item in LayersTagsDataList)
            {
                if (item.tag == tag)
                    temp.Add(item.name);
            }
        }
        return temp.ToArray();
    }
    static public string[] GetNamesByLayerTag(string layer,string tag)
    {
        List<string> temp = new List<string>();
        if (!string.IsNullOrEmpty(layer))
        {
            foreach (var item in LayersTagsDataList)
            {
                if (item.layer == layer && item.tag == tag)
                    temp.Add(item.name);
            }
        }
        return temp.ToArray();
    }

    public static void Save()
    {
        
        EditorUtility.SetDirty(layersTagsData);
    }
}

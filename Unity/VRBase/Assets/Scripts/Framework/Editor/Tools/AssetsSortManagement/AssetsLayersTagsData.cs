using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsLayersTagsData : ScriptableObject
{
    public List<string> LayersList = new List<string>();
    public List<string> tagsList = new List<string>();

    public List<LayersTagsData> LayersTagsDataList = new List<LayersTagsData>();

    public AssetsLayersTagsData()
    {
        LayersList.Add("None");
        tagsList.Add("None");
    }

    public void ReplaceLayersName(string oldLayerName,string newLayerName)
    {
        foreach (var item in LayersTagsDataList)
        {
            if(item.layer== oldLayerName)
            {
                item.layer = newLayerName;
            }

        }
    }
    public void ReplaceTagsName(string oldTagName, string newTagName)
    {
        foreach (var item in LayersTagsDataList)
        {
            if (item.tag == oldTagName)
            {
                item.tag = newTagName;
            }

        }
    }

    public void DeleteLayersTagsItem(string name)
    {
        foreach (var item in LayersTagsDataList)
        {
            if (item.name == name)
            {
                LayersTagsDataList.Remove(item);
                break;
            }
        }
    }
    public bool ExistInLayersTagsDataList(string name)
    {
        foreach (var item in LayersTagsDataList)
        {
            if (item.name == name)
                return true;
        }
        return false;
    }

    public LayersTagsData AddNewLayersTagsData(string name,string layer= "None", string tag = "None")
    {
        if (ExistInLayersTagsDataList(name))
            return null;

        LayersTagsData lt = new LayersTagsData(name);
        lt.layer = layer;
        lt.tag = tag;
        LayersTagsDataList.Add(lt);
        return lt;
    }

    public LayersTagsData GetLayersTagsData(string name)
    {
        foreach (var item in LayersTagsDataList)
        {
            if (item.name == name)
                return item;
        }
        return null;
    }
}

[System.Serializable]
public class LayersTagsData
{
    public string name = "";
    public string layer = "None";
    public string tag = "None";

    public LayersTagsData()
    {
        layer = "None";
        tag = "None";
    }
    public LayersTagsData(string name)
    {
        this.name = name;
        layer = "None";
        tag = "None";
    }
}
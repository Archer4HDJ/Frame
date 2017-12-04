using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public static class UnityLayerTagUtils  {

   public static void AddTag(string tag)
    {
        if (!IsHasTag(tag))
        {
#if UNITY_5_0 || UNITY_2017

            InternalEditorUtility.AddTag(tag);
#else
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "tags")
                {
                    for (int i = 0; i < it.arraySize; i++)
                    {
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                        if (string.IsNullOrEmpty(dataPoint.stringValue))
                        {
                            dataPoint.stringValue = tag;
                            tagManager.ApplyModifiedProperties();
                            return;
                        }
                    }
                }
            }
#endif
        }
    }

    public static void AddLayer(string layer)
    {
        if (!IsHasLayer(layer))
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
#if UNITY_5_0 || UNITY_2017
  
                if (it.name=="layers")  
  
                {  
  
                    //层默认是32个，只能从第8个开始写入自己的层  
  
                    for (int i = 8; i < it.arraySize; i++)  
  
                    {  
  
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);//获取层信息  
  
                        if (string.IsNullOrEmpty(dataPoint.stringValue))//如果制定层内为空，则可以填写自己的层名称  
  
                        {  
  
                            dataPoint.stringValue = layer;//设置名字  
  
                            tagManager.ApplyModifiedProperties();//保存修改的属性  
  
                            return;  
  
                        }  
  
                    }  
  
                }  
 
#else

                if (it.name.StartsWith("User Layer"))

                {

                    if (it.type == "string")

                    {

                        if (string.IsNullOrEmpty(it.stringValue))

                        {

                            it.stringValue = layer;

                            tagManager.ApplyModifiedProperties();

                            return;

                        }

                    }

                }

#endif
            }
        }
    }

 

    public static bool RemoveTag(string tag)
    {
        if (IsHasTag(tag))
        {
            InternalEditorUtility.RemoveTag(tag);
            return true;
        }
        return false;
    }

    public static bool IsHasTag(string tag)
    {
        for (int i = 0; i < InternalEditorUtility.tags.Length; i++)
        {
            if (InternalEditorUtility.tags[i].Equals(tag))
                return true;
        }
        return false;
    }

    public static bool IsHasLayer(string layer)
    {
        for (int i = 0; i < InternalEditorUtility.layers.Length; i++)
        {
            if (InternalEditorUtility.layers[i].Equals(layer))
                return true;
        }
        return false;
    }
    public static void AddSortingLayer(string sLayer)
    {
        if (!IsHasSortingLayers(sLayer))
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            MethodInfo addSortingLayerMethod = internalEditorUtilityType.GetMethod("AddSortingLayer", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo setSortingLayerNameMethod = internalEditorUtilityType.GetMethod("SetSortingLayerName", BindingFlags.Static | BindingFlags.NonPublic);
            addSortingLayerMethod.Invoke(null, null);

            int index = GetSortingLayerCount() - 1;
            setSortingLayerNameMethod.Invoke(null, new object[] { index, sLayer });
        }

    }
    public static bool SetSortingLayerName(string oldName, string newName)
    {
        int index = GetSortingLayersIndex(oldName);
        if (index == -1)
            return false;
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        MethodInfo setSortingLayerNameMethod = internalEditorUtilityType.GetMethod("SetSortingLayerName", BindingFlags.Static | BindingFlags.NonPublic);
        setSortingLayerNameMethod.Invoke(null, new object[] { index, newName });
        return true;
    }
    public static int GetSortingLayerCount()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        MethodInfo getSortingLayerCountNameMethod = internalEditorUtilityType.GetMethod("GetSortingLayerCount", BindingFlags.Static | BindingFlags.NonPublic);
        return (int)getSortingLayerCountNameMethod.Invoke(null, null);
    }

    public static string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        string[] layers = (string[])sortingLayersProperty.GetValue(null, new object[0]);
        return layers;
    }
    public static bool IsHasSortingLayers(string sLayer)
    {
        string[] layers = GetSortingLayerNames();
        foreach (var item in layers)
        {
            if (item.Equals(sLayer))
                return true;
        }
        return false;
    }
    public static int GetSortingLayersIndex(string sLayer)
    {
        int index = -1;
        string[] layers = GetSortingLayerNames();
        for (int i = 0; i < layers.Length; i++)
        {
            if(layers[i].Equals(sLayer))
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public static bool SetSortingLayerLocked(string sLayer,bool locked)
    {
       int index =  GetSortingLayersIndex(sLayer);
        if (index == -1)
            return false;
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        MethodInfo setSortingLayerLockedMethod = internalEditorUtilityType.GetMethod("SetSortingLayerLocked", BindingFlags.Static | BindingFlags.NonPublic);
        setSortingLayerLockedMethod.Invoke(null, new object[] { index, locked });
        return true;
    }
    public static bool GetSortingLayerLocked(string sLayer,out string exception)
    {
        exception = "";
        int index = GetSortingLayersIndex(sLayer);
        if (index == -1)
        {
            exception = "don't have '"+sLayer+"'";
            return false;
        }
            
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        MethodInfo getSortingLayerLockedMethod = internalEditorUtilityType.GetMethod("GetSortingLayerLocked", BindingFlags.Static | BindingFlags.NonPublic);
        return (bool)getSortingLayerLockedMethod.Invoke(null, new object[] { index });
    }
}

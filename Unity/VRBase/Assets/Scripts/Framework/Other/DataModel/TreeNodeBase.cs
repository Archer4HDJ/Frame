using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TreeNodeBase
{
    private int id =0;
    public int Id { get { return id; } }
    public string relativeRootPath = "";
    public string InternalFullPath { get {
            if (string.IsNullOrEmpty(relativeRootPath))
                return "Root";
            return "Root/" + relativeRootPath; } }
    private int deep = -1;
    public int Deep
    {
        get
        {
            if (deep == -1)
            {
                deep = relativeRootPath.Split('/').Length;
            }
            return deep;
        }
    }
    //父级id
    public int parent = -1;
    public List<int> childs = new List<int>();

    public  TreeNodeBase(int id,string path) 
    {
        this.id = id;
        this.relativeRootPath = path;
    }

    public void CopyAllTo( TreeNodeBase to)
    {
       Dictionary<string,object> dic=  HDJ.Framework.Utils.ReflectionUtils.GetClassOrStructData(this);
        dic.Remove("parent");
        dic.Remove("childs");
        HDJ.Framework.Utils.ReflectionUtils.SetClassOrStructData(dic, to.GetType(),false,to);
    }

}

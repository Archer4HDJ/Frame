using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TreeModelController<T> where T: TreeNodeBase
{
    private T rootNode;
    public T RootNode { get { return rootNode; } }

    public List<T> TreeDataList { get { return treeDataList; } }

    public int GetCreateID { get { return maxIDNumber++; } }

    private List<T> treeDataList = new List<T>();
    private Dictionary<int, T> treeDic = new Dictionary<int, T>();
    private Dictionary<string, T> treePathDic = new Dictionary<string, T>();
    private int maxIDNumber = 0;
   public  TreeModelController(List<T> treeList)
    {
        this.treeDataList.Clear();
        treeDic.Clear();
        ListForeachNode((n) =>
        {
            if (n.parent ==-1)
            {
                rootNode = n;             
            }
            cacheAdd(n);
            return true;
        });
    }
    public TreeModelController()
    {
        this.treeDataList = new List<T>();
        treeDic.Clear();
        treePathDic.Clear();
        rootNode = GetNewNode("");
        cacheAdd(rootNode);
    }

    private void cacheAdd(T node)
    {
        treeDataList.Add(node);
        treeDic.Add(node.Id, node);
        treePathDic.Add(node.InternalFullPath, node);
    }
    private void RemoveNode(T node)
    {
        treeDic.Remove(node.Id);
        treeDataList.Remove(node);
        treePathDic.Remove(node.relativeRootPath);
    }
    public T GetNode(int id)
    {
        T node = null;
        treeDic.TryGetValue(id, out node);
        return node;
    }
    public bool ExistsNode(string path)
    {

        if (treePathDic.ContainsKey(path))
            return true;
        return false;
    }
    public T GetNode(string path)
    {
        T node = null;
        treePathDic.TryGetValue(path, out node);
        return node;
    }

    public void ListForeachNode(CallBackR<bool, T> eachItemCallBack) 
    {
        for (int i = 0; i < TreeDataList.Count; i++)
        {
            if (eachItemCallBack != null)
            {
                T t = TreeDataList[i];
                if (t == rootNode)
                    continue;
                if (!eachItemCallBack(t))
                    break;
            }
        }
    }

    public void TreeForeachNode(CallBackR<bool, T> eachItemCallBack)
    {
        SearchChilds(rootNode, eachItemCallBack);
    }

    public void SearchChilds(T node, CallBackR<bool, T> eachItemCallBack)
    {
        if (node == null) return;

        List<int> idList = node.childs;
        for (int i = 0; i < idList.Count; i++)
        {
            T n = GetNode(idList[i]);

            if (eachItemCallBack(n))
                SearchChilds(n, eachItemCallBack);

        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isAllDeepChild">false:只取下一层的，true：所有层的</param>
    /// <returns></returns>
    public T[] GetNodeChilds(int id,bool isAllDeepChild =false)
    {
        T t = GetNode(id);
        if (t == null)
            return null;
        List<T> list = new List<T>();
        if (!isAllDeepChild)
        {
            List<int> idList = t.childs;
            for (int i = 0; i < idList.Count; i++)
            {
                T n = GetNode(idList[i]);
                list.Add(n);

            }
        }
        else
        {
            SearchChilds(t, (n) =>
            {
                list.Add(n);
                return true;
            });
        }
        return list.ToArray();
    }
    public bool InserNode(int parentID,T node)
    {
        T t = GetNode(parentID);
        if (t == null)
            return false;
        t.childs.Add(node.Id);
        node.parent = parentID;
        cacheAdd(node);
        return true;
    }

    public bool MoveNode(int id, int toId)
    {
        T t = GetNode(id);
        T t1 = GetNode(toId);
        if (t == null || t1==null)
            return false;
        T t2 = GetNode(t.parent);
        t2.childs.Remove(id);
        t1.childs.Add(id);
        t.parent = toId;
        return true;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="deleteChilds">false:有子集时默认删除失败，true：连同本身和子集一起删除</param>
    /// <returns></returns>
    public bool DeleteNode(int id,bool deleteChilds =false)
    {
        T t = GetNode(id);
        if (t == null)
            return false;
        if (t.childs.Count > 0 && !deleteChilds)
            return false;
        T[] arr = GetNodeChilds(id, true);
        for (int i = 0; i < arr.Length; i++)
        {
            RemoveNode(arr[i]);
        }
        T t1 = GetNode(t.parent);
        if (t1 != null)
            t1.childs.Remove(id);
        RemoveNode(t);

        return true;
    }

    public void AddNode(T node)
    {     
        if (ExistsNode(node.InternalFullPath))
        {
            T n = GetNode(node.InternalFullPath);
            node.CopyAllTo( n);
            return;
        }
        string[] pathArr = node.relativeRootPath.Split('/');
        Type t = node.GetType();
        int deep = pathArr.Length;
        T now = rootNode;
        string nowSPath = now.InternalFullPath;
        for (int i = 0; i < deep; i++)
        {
            bool isHave = false;
            nowSPath = now.InternalFullPath + "/" + pathArr[i];
            T n = GetNode(nowSPath);
            if (n == null)
            {
                isHave = false;
            }
            else
            {
                isHave = true;
                now = n;
            }

            if (!isHave)
            {
                if (i == deep - 1)
                {
                    InserNode(now.Id, node);
                }
                else
                {
                   // Debug.LogError ("nowSPath: "+ nowSPath+"   ==:"+ Path.GetPathRoot (nowSPath));
                    T temp = GetNewNode(t, nowSPath.Substring(5));
                    InserNode(now.Id, temp);
                    now = temp;
                }
            }
        }
    }

    public void AddPathsToCreateNode(string[] paths)
    { 
        for (int i = 0; i < paths.Length; i++)
        {
            T node =(T) GetNewNode(typeof(T) , paths[i]);
            AddNode(node);
        }
    }
    private T GetNewNode(Type t, string path)
    {
        object obj = Activator.CreateInstance(t, new object[] { GetCreateID,path });
        T temp = (T)obj;
        return temp;
    }

    public T GetNewNode(string path)
    {
        return GetNewNode(typeof(T), path);
    }

    public T GetParentNode(T t)
    {
        T p = GetNode(t.parent);
        return p;
    }

}

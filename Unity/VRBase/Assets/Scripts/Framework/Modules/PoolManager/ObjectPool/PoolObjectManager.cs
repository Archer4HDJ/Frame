using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Modules
{
    public static class PoolObjectManager
    {
        private static int storeMaxNum = 50;
        //每种GameObject最多储存数目
        private const int perItemMaxNum = 5;
        /// <summary>
        /// 最大储存类型数目
        /// </summary>
        public static int StoreMaxNum
        {
            get { return storeMaxNum; }
            set
            {
                storeMaxNum = value;
                if (storeMaxNum < 1)
                    storeMaxNum = 1;
            }
        }
        private static Dictionary<string, Stack<GameObject>> dataDic = new Dictionary<string, Stack<GameObject>>();

        private static GameObject rootObj;
        /// <summary>
        /// 从对象池中获得实例化对象
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        public static GameObject GetObject(string prefabName)
        {
            if (!Application.isPlaying)
                return GetInstanceObject.GetInstance(prefabName);

            if (dataDic.ContainsKey(prefabName) && dataDic[prefabName].Count > 0)
            {
                GameObject po = dataDic[prefabName].Pop();
                if (po)
                {
                    ResetObject(po);
                    return po;
                }
                else
                {
                    return GetInstanceObject.GetInstance(prefabName);
                }
            }

            return GetInstanceObject.GetInstance(prefabName);
        }
        /// <summary>
        /// 异步从对象池中获得实例化对象
        /// </summary>
        /// <param name="prefabName"></param>
        /// <param name="callback"></param>
        public static void GetObjectAsync(string prefabName, CallBack<GameObject> callback)
        {


            if (dataDic.ContainsKey(prefabName) && dataDic[prefabName].Count > 0)
            {
                GameObject po = dataDic[prefabName].Pop();
                if (po)
                {
                    ResetObject(po);
                    if (callback != null)
                        callback(po);
                }
                else
                {
                    GetInstanceObject.GetInstanceAsync(prefabName, callback);
                }
            }
            else
            {
                GetInstanceObject.GetInstanceAsync(prefabName, callback);
            }
        }
        private static void ResetObject(GameObject obj)
        {
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            PoolObject[] pos = obj.GetComponentsInChildren<PoolObject>();
            for (int i = 0; i < pos.Length; i++)
            {
                pos[i].Reset();
            }
        }
        /// <summary>
        /// 对象回收到池里
        /// </summary>
        /// <param name="obj"></param>
        public static void DestroyObject(GameObject obj, bool isDestroyImmediate = false)
        {
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate((Object)obj);
                return;
            }
            if (isDestroyImmediate)
                Object.Destroy(obj.gameObject);

            if (rootObj == null)
            {
                rootObj = new GameObject("[PoolObjectManager]");
                Object.DontDestroyOnLoad(rootObj);
            }
            if (obj)
            {
                if (dataDic.ContainsKey(obj.name))
                {
                    if (dataDic[obj.name].Count < perItemMaxNum)
                    {
                        obj.transform.SetParent(rootObj.transform);
                        obj.gameObject.SetActive(false);
                        dataDic[obj.name].Push(obj);
                    }
                    else
                    {
                        Object.Destroy(obj.gameObject);
                    }
                }
                else
                {
                    List<string> names = new List<string>(dataDic.Keys);
                    if (dataDic.Count >= StoreMaxNum)
                    {
                        Remove(names[0]);
                    }
                    Stack<GameObject> st = new Stack<GameObject>();
                    st.Push(obj);
                    dataDic.Add(obj.name, st);
                }
            }
        }
        /// <summary>
        /// 移除一种类型的对象
        /// </summary>
        /// <param name="prefabName"></param>
        public static void Remove(string prefabName)
        {
            if (dataDic.ContainsKey(prefabName))
            {
                GameObject[] poArray = dataDic[prefabName].ToArray();

                for (int j = 0; j < poArray.Length; j++)
                {
                    if (poArray[j])
                        Object.Destroy(poArray[j].gameObject);
                }
                dataDic[prefabName].Clear();
                dataDic.Remove(prefabName);
            }
        }

        public static void Clear()
        {
            List<string> names = new List<string>(dataDic.Keys);
            for (int i = 0; i < names.Count; i++)
            {
                Remove(names[i]);
            }
            dataDic.Clear();
        }


    }
}

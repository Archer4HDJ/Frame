using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Core.ECS
{
    public class ECSGroupManager 
    {
        private World world;
        private Dictionary<int, ECSGroup> allGroupDic = new Dictionary<int, ECSGroup>();
       private Dictionary<ECSGroup, List<Entity>> groupToEntityDic = new Dictionary<ECSGroup, List<Entity>>();
      private  Dictionary<Entity, List<ECSGroup>> entityToGroupDic = new Dictionary<Entity, List<ECSGroup>>();
       
        public ECSGroupManager(World world)
        {
            this.world = world;

            for (int i = 0; i < world.Systems.Count; i++)
            {
                ISystem system = world.Systems[i];
                int key = StringArrayToInt( system.FilterComponentNames);
                if (allGroupDic.ContainsKey(key))
                {
                    continue;
                }
                ECSGroup group = new ECSGroup(key, system.FilterComponentNames);
                allGroupDic.Add(key, group);
                groupToEntityDic.Add(group, new List<Entity>());
            }

        }
        public int StringArrayToInt(string[] arr)
        {
            Array.Sort(arr);
            string tempS = string.Join("&", arr);
            return MD5Utils.GetStringToHash( tempS);
        }
        public List<Entity> GetEntityByFilter(int key, string[] filters)
        {
            ECSGroup group;
            if (!allGroupDic.TryGetValue(key, out group))
            {
                AddGroup(key, filters);
            }
            List<Entity> list;
            if (groupToEntityDic.TryGetValue(group, out list))
            {
                if (list != null)
                    return list;
            }
            return new List<Entity>();
        }
        public List<Entity> GetEntityByFilter(string[] filters)
        {
            int key = StringArrayToInt(filters);

            return GetEntityByFilter(key, filters);
        }

        public ECSGroup[] GetGroupByEntity(Entity entity)
        {
            List<ECSGroup> list;
            if(entityToGroupDic.TryGetValue(entity,out list))
            {
                if (list != null)
                    return list.ToArray();
            }

            return new ECSGroup[0];
        }

        private bool AddGroup(int key, string[] componentFilter)
        {
            if ( componentFilter == null || componentFilter.Length == 0)
            {
                Debug.LogError("AddGroup 失败，参数不能为空！");
                return false;
            }
            if (allGroupDic.ContainsKey(key))
            {
                Debug.LogError("AddGroup 失败，名字重复！");
                return false;
            }

            ECSGroup group = new ECSGroup(key, componentFilter);
            allGroupDic.Add(key, group);

            List<Entity> newListEntity = new List<Entity>();

            List<Entity> listEntity = new List<Entity>(entityToGroupDic.Keys);
            for (int i = 0; i < listEntity.Count; i++)
            {
                Entity entity = listEntity[i];
                List<ECSGroup> newGroupList = GetEntitySuportGroup(entity);
                bool isContains = true;
                for (int j = 0; j < componentFilter.Length; j++)
                {
                    if (!entity.Components.ContainsKey(componentFilter[j]))
                    {
                        isContains = false;
                    }
                }
                if (isContains)
                {
                    newListEntity.Add(entity);
                    entityToGroupDic[entity].Add(group);
                }
            }
            groupToEntityDic.Add(group, newListEntity);

            return true;
        }


        public void OnEntityCreate(Entity entity)
        {
            List<ECSGroup> newGroupList = GetEntitySuportGroup(entity);

            if (!entityToGroupDic.ContainsKey(entity))
                entityToGroupDic.Add(entity, newGroupList);
            else
                entityToGroupDic[entity] = newGroupList;

            for (int i = 0; i < newGroupList.Count; i++)
            {
                List<Entity> list = groupToEntityDic[newGroupList[i]];
                if (!list.Contains(entity))
                    list.Add(entity);
            }

        }
        public void OnEntityDestroy(Entity entity)
        {
            List<ECSGroup> list = entityToGroupDic[entity];
            for (int i = 0; i < list.Count; i++)
            {
                groupToEntityDic[list[i]].Remove(entity);
            }
            entityToGroupDic.Remove(entity);
        }

        public void OnEntityComponentChange(Entity entity,IComponent component)
        {
            List<ECSGroup> oldSystems;
            if (!entityToGroupDic.TryGetValue(entity,out oldSystems))
            {
                oldSystems = new List<ECSGroup>();
                entityToGroupDic.Add(entity, oldSystems);
            }

            List<ECSGroup> newGroupList = GetEntitySuportGroup(entity);

            entityToGroupDic[entity] = newGroupList;

            for (int i = 0; i < newGroupList.Count; i++)
            {
                ECSGroup sys = newGroupList[i];
                if (!oldSystems.Contains(sys))
                {
                    List<Entity> list = groupToEntityDic[sys];
                    if (list == null)
                    {
                        list = new List<Entity>();
                    }
                    list.Add(entity);
                }
            }


            for (int i = 0; i < oldSystems.Count; i++)
            {
                ECSGroup sys = oldSystems[i];
                if (!newGroupList.Contains(sys))
                {
                    List<Entity> list = groupToEntityDic[sys];
                    if (list == null)
                    {
                        list = new List<Entity>();
                    }

                    list.Remove(entity);
                }
            }
        }
        public List<ECSGroup> GetEntitySuportGroup(Entity entity)
        {
            List<ECSGroup> groupNames = new List<ECSGroup>();
            List<ECSGroup> groups = new List<ECSGroup>(allGroupDic.Values);

            for (int i = 0; i < groups.Count; i++)
            {
                string[] filterCom = groups[i].Components;
                bool isContains = true;
                for (int j = 0; j < filterCom.Length; j++)
                {
                    if (!entity.Components.ContainsKey(filterCom[j]))
                    {
                        isContains = false;
                    }
                }
                if (isContains)
                {
                    groupNames.Add(groups[i]);
                }
            }

            return groupNames;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using HDJ.Framework.Utils;
using UnityEngine;
using HDJ.Framework.Modules;

namespace HDJ.Framework.Core.ECS
{
    public class World
    {
        public CallBack<Entity> OnEntityCreate;
        public CallBack<Entity> OnEntityDestroy;
        public CallBack<Entity, IComponent> OnEntityAddComponent;
        public CallBack<Entity, IComponent> OnEntityRemoveComponent;

        private string name;
        private List<ISystem> systems = new List<ISystem>();

        private Dictionary<int, Entity> entitys = new Dictionary<int, Entity>();

        private PoolClassManager<Entity> poolEntity ;
        private ECSGroupManager groupES;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public List<ISystem> Systems
        {
            get
            {
                return systems;
            }
        }

        public ECSGroupManager GroupES
        {
            get
            {
                return groupES;
            }
        }

        public World(WorldSettingData settingdata)
        {
            this.name = settingdata.worldName;
            string[] useSystem = settingdata.useSystemList.ToArray();
            for (int i = 0; i < useSystem.Length; i++)
            {
                string systemName = useSystem[i];
                Type type = ReflectionUtils.GetTypeByTypeFullName(systemName);
                if (type == null)
                    continue;
                object obj = ReflectionUtils.CreateDefultInstance(type);
                ISystem system = (ISystem)obj;
                system.delayExecute = settingdata.allSystemSettingDatas[systemName].delayExecute / 1000f;
                system.Initialize(this);
                systems.Add(system);
            }

            groupES = new ECSGroupManager(this);
            OnEntityAddComponent += groupES.OnEntityComponentChange;
            OnEntityRemoveComponent += groupES.OnEntityComponentChange;
            OnEntityCreate += groupES.OnEntityCreate;
            OnEntityDestroy += groupES.OnEntityDestroy;

            poolEntity = new PoolClassManager<Entity>(60, ResetEntity);
        }


        private  int EntityIDCounter = 0;

        public Entity CreateEntity(string name = "New Entity", object resourceHandle = null)
        {
            Entity entity = poolEntity.New();
            entity.ID= EntityIDCounter;
            entity.ResourceHandle=resourceHandle;
            entity.world = this;
            entity.name = name;

            EntityIDCounter++;
            entitys.Add(entity.ID, entity);

            if (OnEntityCreate != null)
                OnEntityCreate(entity);

            return entity;
        }
        private static void ResetEntity(Entity t)
        {
            t.Components.Clear();
        }

        public Entity GetEntity(int id)
        {
            Entity entity = null;
            if(entitys.TryGetValue(id,out entity))
            {
                return entity;
            }
            else
            {
                Debug.LogError("未找到 Entity ：" + id);
                return null;
            }
        }

        public Entity[] GetEntitys(string name)
        {
            List<Entity> list = new List<Entity>();
            List<Entity> tempEntitys = new List<Entity>(entitys.Values);
            for (int i = 0; i < tempEntitys.Count; i++)
            {
                Entity entity = tempEntitys[i];
                if(entity.name == name)
                {
                    list.Add(entity);
                }
            }

            return list.ToArray();
         }
        public Entity GetEntity(string name)
        {
            List<Entity> tempEntitys = new List<Entity>(entitys.Values);
            for (int i = 0; i < tempEntitys.Count; i++)
            {
                Entity entity = tempEntitys[i];
                if (entity.name == name)
                {
                    return entity;
                }
            }

            return null;
        }

        public void DestroyEntity(int id)
        {
            if (entitys.ContainsKey(id))
            {
                poolEntity.Recycle(entitys[id]);
                entitys.Remove(id);
               

                if (OnEntityDestroy != null)
                    OnEntityDestroy(entitys[id]);
            }
        }
        public void DestroyEntity(Entity entity)
        {
            DestroyEntity(entity.ID);
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < systems.Count; i++)
            {
                systems[i].Update(deltaTime);
            }
        }

        public void FixedUpdate(int deltaTime)
        {
            for (int i = 0; i < systems.Count; i++)
            {
                systems[i].FixedUpdate(deltaTime);
            }
        }
    }
}

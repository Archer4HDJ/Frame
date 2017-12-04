using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Core.ECS
{
    public abstract class ISystem
    {
        private string[] filterComponentNames;

        public string[] FilterComponentNames
        {
            get
            {
                if (filterComponentNames == null)
                {
                    Type[] types = FilterComponentType();
                    if (types == null || types.Length == 0)
                    {
                        Debug.LogError("System Filter Type is null or Empty，System Name： " + GetType().FullName);
                        return null;
                    }
                    filterComponentNames = new string[types.Length];
                    for (int i = 0; i < types.Length; i++)
                    {
                        filterComponentNames[i] = types[i].FullName;
                    }
                }
                return filterComponentNames;
            }
        }
        public Type[] FilterComponentTypes
        {
            get
            {
                return FilterComponentType();
            }
        }

        protected World world;
        protected string systemName;
        public void Initialize(World world)
        {
            this.world = world;
            systemName = GetType().FullName;


            OnAwake();
        }
        private bool isGetHashCode = false;
        private int filterNameHashCode;
        protected List<Entity> GetSystemEntitys()
        {
            if (!isGetHashCode)
            {
                isGetHashCode = true;

                filterNameHashCode = world.GroupES.StringArrayToInt(FilterComponentNames);
            }
            return  world.GroupES.GetEntityByFilter(filterNameHashCode,FilterComponentNames);
        }
        protected abstract Type[] FilterComponentType();
        protected virtual void OnAwake() { }


        public float delayExecute = 0;
        private float tempTime=0f;
        /// <summary>
        /// Unity执行每帧
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if (tempTime <= 0)
            {
                tempTime = delayExecute;

                OnUpdate(delayExecute);

                List<Entity> entitys = GetSystemEntitys();
                for (int i = 0; i < entitys.Count; i++)
                {
                    OnExecuteEntityUpdate(entitys[i]);
                }
            }
            else
            {
                tempTime -= deltaTime;
            }
        }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnExecuteEntityUpdate(Entity entity) { }


        public void FixedUpdate(int deltaTime)
        {
            OnFixedUpdate(deltaTime);

            List<Entity> entitys = GetSystemEntitys();
            for (int i = 0; i < entitys.Count; i++)
            {
                OnExecuteEntityFixedUpdate(entitys[i]);
            }
        }
        protected virtual void OnFixedUpdate(int deltaTime) { }
        protected virtual void OnExecuteEntityFixedUpdate( Entity entity) { }

      
    }

}
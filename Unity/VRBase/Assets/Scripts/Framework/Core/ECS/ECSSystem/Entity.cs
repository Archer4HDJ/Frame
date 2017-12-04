using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HDJ.Framework.Utils;
using System;

namespace HDJ.Framework.Core.ECS
{
    public class Entity
    {
        public string name;

        private int id;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private object resourceHandle;
        public object ResourceHandle
        {
            get
            {
                return resourceHandle;
            }

            set
            {
                resourceHandle = value;
            }
        }
        public World world;

        private Dictionary<string, IComponent> components = new Dictionary<string, IComponent>();
        public Dictionary<string, IComponent> Components
        {
            get
            {
                return components;
            }
        }
        public Entity() { name = "New Entity"; }
        public Entity(int id,string name="New Entity", object resourceHandle = null)
        {
            this.id = id;
            this.name = name;
            this.resourceHandle = resourceHandle;
        }


        public Entity AddComponent<T>() where T : IComponent
        {
            string name = typeof(T).FullName;
            return AddComponent(name);
        }

        public Entity AddComponent(string componentName)
        {
            if (components.ContainsKey(componentName))
            {
                Debug.LogError("Entity :" + id + " 添加组件失败！组件已存在：" + componentName);
                return this;
            }

            Type type = ReflectionUtils.GetTypeByTypeFullName(componentName);
            if (type == null)
            {
                Debug.LogError("Entity :" + id + " 添加组件失败！组件不存在：" + componentName);
                return this;
            }

            if (!typeof(IComponent).IsAssignableFrom(type))
            {
                Debug.LogError("Entity :" + id + " 添加组件失败！[" + componentName + "] 没有直接或间接继承[IComponent]");
                return this;
            }
            IComponent obj=null;

            if (type.BaseType != null)
            {
                if(type.BaseType.Name == typeof(Singleton<>).Name)
                {
                    obj = (IComponent)ReflectionUtils.GetPropertyInfo(type, null, "Instance");
                }
            }

            if (obj == null)
            {
                obj = (IComponent)ReflectionUtils.CreateDefultInstance(type);
            }
            components.Add(componentName, obj);

            if (world.OnEntityAddComponent != null)
            {
                world.OnEntityAddComponent(this, obj);
            }
            return this;
        }

        public Entity AddComponent(IComponent component)
        {
            if (component != null)
            {
                string name = component.GetType().FullName;
                if (components.ContainsKey(name))
                {
                    Debug.LogError("Entity :" + id + " 添加组件失败！组件已存在：" + name);
                    return this;
                }
                components.Add(name, component);
                if (world.OnEntityAddComponent != null)
                {
                    world.OnEntityAddComponent(this, component);
                }
            }
            else
            {
                Debug.LogError("Entity :" + id + " 添加组件失败! component is null");
            }

            return this;
        }

        public T GetComponent<T>()
        {
            return (T)GetComponent(typeof(T).FullName);
        }
        public IComponent GetComponent(string componentName)
        {
            if (components.ContainsKey(componentName))
            {
                return components[componentName];
            }
            return null;
        }

        public Entity RemoveComponent<T>()
        {

            string name = typeof(T).Name;
            return RemoveComponent(name);
        }

        public Entity RemoveComponent(IComponent component)
        {
            if (component != null)
            {
                string name = component.GetType().Name;
                if (components.ContainsKey(name))
                {
                    components.Remove(name);

                    if (world.OnEntityRemoveComponent != null)
                    {
                        world.OnEntityRemoveComponent(this, component);
                    }
                    return this;
                }
                Debug.LogError("Entity :" + id + " 移除组件失败！组件不存在：" + name);
            }
            else
            {
                Debug.LogError("Entity :" + id + " 移除组件失败! component is null");
            }
            return this;
        }
        public Entity RemoveComponent(string componentName)
        {
            if (components.ContainsKey(componentName))
            {
                IComponent component = components[componentName];

                components.Remove(componentName);
                
                if (world.OnEntityRemoveComponent != null)
                {
                    world.OnEntityRemoveComponent(this, component);
                }
                return this;
            }
            Debug.LogError("Entity :" + id + " 移除组件失败！组件不存在：" + componentName);
            return this;
        }

    }
}

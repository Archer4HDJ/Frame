using System.Collections;
using System.Collections.Generic;

namespace HDJ.Framework.Core.ECS
{
    public static class WorldManager
    {
        static Dictionary<string, World> allWorldDic = new Dictionary<string, World>();

        private static World currentRunWorld;

        public static World CurrentRunWorld
        {
            get
            {
                return currentRunWorld;
            }
        }

       
        // Use this for initialization
        public static void Initialize(WorldManagerSettingData settingData)
        {
            for (int i = 0; i < settingData.allWorldSettingData.Count; i++)
            {
                WorldSettingData data = settingData.allWorldSettingData[i];
                World world = new World(data);
                allWorldDic.Add(world.Name, world);
            }

            currentRunWorld = allWorldDic[settingData.defaultFirstRunWorldName];
        }

        public static Dictionary<string, World> GatAllWorld()
        {
            return allWorldDic;
        }

        /// <summary>
        /// Unity的每帧更新，联网时服务器不使用，客户端使用
        /// </summary>
        /// <param name="deltaTime"></param>
        public static void Update(float deltaTime)
        {
            if (currentRunWorld != null)
                currentRunWorld.Update(deltaTime);
        }
        /// <summary>
        /// 固定时间更新，联网游戏和服务器对时后自定义固定时间更新服务器也会使用，单机时，使用Unity固定更新
        /// </summary>
        /// <param name="deltaTime">毫秒</param>
        public static void FixedUpdate(int deltaTime)
        {
            if (currentRunWorld != null)
                currentRunWorld.FixedUpdate(deltaTime);
        }
    }

}
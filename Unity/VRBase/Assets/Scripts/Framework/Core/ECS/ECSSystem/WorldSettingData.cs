using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDJ.Framework.Core.ECS
{
   public  class WorldManagerSettingData
    {
        /// <summary>
        /// 固定更新间隔时间（毫秒）
        /// </summary>
        public int fixedUpdateDeltaTime = 50;

        public string defaultFirstRunWorldName;

        public List<WorldSettingData> allWorldSettingData = new List<WorldSettingData>();
      
         

        public WorldSettingData GetWorldSettingData(string name)
        {
            for (int i = 0; i < allWorldSettingData.Count; i++)
            {
                if (allWorldSettingData[i].worldName == name)
                    return allWorldSettingData[i];
            }
            return null;
        }

        public List<string> GetAllWorldNames()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < allWorldSettingData.Count; i++)
            {
                list.Add(allWorldSettingData[i].worldName);
            }
            return list;
        }

        public bool IsHaveRepeatName(WorldSettingData data)
        {
            for (int i = 0; i < allWorldSettingData.Count; i++)
            {
                if (allWorldSettingData[i]!= data && allWorldSettingData[i].worldName == data.worldName)
                    return true;
            }
            return false;
        }
    }

    public class WorldSettingData
    {
        public string worldName;
        public List<string> useSystemList = new List<string>();

        public Dictionary<string, SystemsSettingData> allSystemSettingDatas = new Dictionary<string, SystemsSettingData>();
    }

    public class SystemsSettingData
    {
        public string systemName;
        /// <summary>
        /// Update 方法执行的间隔时间，仅用于客户端
        /// </summary>
        public int delayExecute = 0;
    }
}

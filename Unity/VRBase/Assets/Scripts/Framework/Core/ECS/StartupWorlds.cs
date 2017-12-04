using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HDJ.Framework.Modules;
using HDJ.Framework.Utils;
using UnityEngine;

namespace HDJ.Framework.Core.ECS
{
    public static class StartupWorlds
    {
        private const string SettingFileName = "WorldSettingConfig";
        private static System.Timers.Timer timer;
        private static WorldManagerSettingData worldSettingData;
        public static void Startup()
        {
           string data =  ResourcesManager.LoadTextFileByName(SettingFileName);

            if (!string.IsNullOrEmpty(data))
            {
                 worldSettingData = JsonUtils.JsonToClassOrStruct<WorldManagerSettingData>(data);
                WorldManager.Initialize(worldSettingData);
                MonoBehaviourRuntime.Instance.OnUpdate += SetUpdate;

                timer = new System.Timers.Timer(worldSettingData.fixedUpdateDeltaTime);
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            WorldManager.FixedUpdate(worldSettingData.fixedUpdateDeltaTime);
        }

        private static void SetUpdate()
        {
            WorldManager.Update(Time.deltaTime);
        }

        public static void Close()
        {
            timer.Stop();
            timer = null;

        }
    }
}

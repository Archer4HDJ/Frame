using HDJ.Framework.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HDJ.Framework.Game.LogicSystem
{
    public class LogicSystemBehavior : PoolObject
    {
        public string logicFileName = "";

        public LogicRuntimeMachine logicManager;
        // Use this for initialization
        void Start()
        {
            logicManager = LogicSystemManager.NewLogicRuntimeMachine(logicFileName);
            logicManager.AddRuntimeGameObjects("target", gameObject);
            logicManager.Start();
        }

        public override void Reset()
        {
            if (logicManager != null)
            {
                logicManager.Close();
            }
            Start();
        }

    }
}

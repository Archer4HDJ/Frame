using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HDJ.Framework.Utils;
using System;
using  HDJ.Framework.Modules;

namespace HDJ.Framework.Game.LogicSystem
{
    public class LogicObjectDataController
    {
        public static LogicObjectContainer GetDataFromFile(string dataName)
        {
            string content = ResourcesManager.LoadTextFileByName(dataName);
            if (string.IsNullOrEmpty(content))
                return new LogicObjectContainer();
            return JsonUtils.JsonToClassOrStruct<LogicObjectContainer>(content);
        }

    }
}

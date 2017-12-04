
using System.Collections.Generic;
using HDJ.Framework.Modules;
using HDJ.Framework.Utils;
namespace HDJ.Framework.Game.LogicSystem
{
    public static class LogicSystem4ECSDataController
    {
        private static Dictionary<string, LogicSystem4ECSData> lS4ECSDataDic;

        public static Dictionary<string, LogicSystem4ECSData> LS4ECSDataDic
        {
            get
            {
                LoadData();
                return lS4ECSDataDic;
            }
        }

        private static bool isInit = false;
        public static void LoadData()
        {
            if (isInit)
                return;
            isInit = true;
            string assets = ResourcesManager.LoadTextFileByName("LogicSystem4ECSData");
            lS4ECSDataDic = JsonUtils.JsonToDictionary<string, LogicSystem4ECSData>(assets);
            if (lS4ECSDataDic == null)
                lS4ECSDataDic = new Dictionary<string, LogicSystem4ECSData>();
        }

        public static void SaveData()
        {
            string assets = JsonUtils.DictionaryToJson(lS4ECSDataDic);
            FileUtils.CreateTextFile(LogicSystemConstData.LogicSystem4ECSDataPath, assets);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Game
{
    [System.Serializable]
    public class MapInfomation
    {

        public int id;
    
        public string mapFileName = "";
        //Light Map Info
        public bool isCreateLightMap = true;
        public int lightMapSize = 1024;
        public string mapLightMap = "";
        //Collider

        public string mapColliderInfoFileName = "";
    }
}


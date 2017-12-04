using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HDJ.Framework.Game
{
    [System.Serializable]
    public class MapWorld
    {

        public List<MapObject> mapObjects = new List<MapObject>();

        public string lightPrefab = "";

        public string colliderPrefab = "";
    }
}

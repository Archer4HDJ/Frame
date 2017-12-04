using HDJ.Framework.Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Game
{
    [System.Serializable]
    public class MapObject
    {
        public string name = "";
        public bool isStatic = false;
        public TransformInfo transformInfo = new TransformInfo();
        public List<ClassValue> behavirComponets = new List<ClassValue>();
    }

    [System.Serializable]
    public class TransformInfo
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }
}

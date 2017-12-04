using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Core.ECS
{
    public class ECSGroup
    {
        private int key;

        public int Key
        {
            get
            {
                return key;
            }
        }
        private string[] components;
        public string[] Components
        {
            get
            {
                return components;
            }
        }
        public ECSGroup(int key, string[] components)
        {
            this.key = key;
            this.components = components;
        }


    }
}

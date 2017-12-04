using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Modules
{
    /// <summary>
    /// 有需要使用对象池取出，重置参数的请继承此类
    /// </summary>
    public abstract class PoolObject : MonoBehaviour
    {

        public abstract void Reset();
    }
}

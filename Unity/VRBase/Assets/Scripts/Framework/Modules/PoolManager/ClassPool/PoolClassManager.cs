using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Modules
{
    public class PoolClassManager<T> where T : class, new()
    {
        private Queue<T> queue;

        private CallBack<T> m_resetAction;
        private CallBack<T> m_onetimeInitAction;

        private int initialBufferSize;
        public PoolClassManager(int initialBufferSize, CallBack<T>
            ResetAction = null, CallBack<T> OnetimeInitAction = null)
        {
            this.initialBufferSize = initialBufferSize;
            queue = new Queue<T>(initialBufferSize);
            m_resetAction = ResetAction;
            m_onetimeInitAction = OnetimeInitAction;

        }

        public T New()
        {
            T t;
            if (queue.Count > 0)
            {
                // an allocated object is already available; just reset it
                t = queue.Dequeue();
                if (m_resetAction != null)
                    m_resetAction(t);


            }
            else
            {
                // no allocated object is available
                t = new T();
                if (m_onetimeInitAction != null)
                    m_onetimeInitAction(t);
            }
            return t;
        }

        public void Recycle(T t)
        {
            if (queue.Count < initialBufferSize)
                queue.Enqueue(t);
        }

        public void Clear()
        {
            if (queue != null)
                queue.Clear();
        }
    }
}

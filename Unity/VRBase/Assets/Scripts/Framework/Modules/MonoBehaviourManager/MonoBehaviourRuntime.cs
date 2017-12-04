using UnityEngine;
using System.Collections;

namespace HDJ.Framework.Modules
{
    public class MonoBehaviourRuntime : MonoSingleton<MonoBehaviourRuntime>
    {
        public CallBack OnUpdate;
        public CallBack OnFixedUpdate;
        public CallBack OnLateUpdate;
        public CallBack OnGUIUpdate;
        public CallBack OnDrawGizmosUpdate;
        protected override void Init()
        {
            DontDestroyOnLoad(this);
        }


        // Update is called once per frame
        void Update()
        {
            if (OnUpdate != null)
                OnUpdate();
        }
         void FixedUpdate()
        {
            if (OnFixedUpdate != null)
                OnFixedUpdate();
        }

         void LateUpdate()
        {
            if (OnLateUpdate != null)
                OnLateUpdate();
        }

         private void OnGUI()
        {
            if (OnGUIUpdate != null)
                OnGUIUpdate();
        }
        private void OnDrawGizmos()
        {
            if (OnDrawGizmosUpdate != null)
                OnDrawGizmosUpdate();
        }
    }
}

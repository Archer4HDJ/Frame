using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HDJ.Framework.Modules {
    public class UIWindowBase : MonoBehaviourExtend {

        public List<GameObject> useGameObject = new List<GameObject>();

        [HideInInspector]
        public string windowName = string.Empty;

        //this page's type
        public UIType type = UIType.Normal;

        //how to show this page.
        public UIMode mode = UIMode.DoNothing;
        [HideInInspector]
        public UIWindowState state = UIWindowState.Closed;

        public GameObject root;
        public GameObject backGroundRoot;

        public GameObject GetGameObject(string name)
        {
            for (int i = 0; i < useGameObject.Count; i++)
            {
                if (useGameObject[i].name == name)
                    return useGameObject[i];
            }
            return null;
        }

        public T GetComponent<T>(string name) where T : Object
        {
            GameObject go = GetGameObject(name);
            if (go)
            {
                return go.GetComponent<T>();
            }
            return null;
        }
        public T[] GetComponents<T>(string name) where T : Object
        {
            GameObject go = GetGameObject(name);
            if (go)
            {
                return go.GetComponents<T>();
            }
            return null;
        }

        private Dictionary<string, List<UnityAction>> buttonCallBackBuff = new Dictionary<string, List<UnityAction>>();

        public void AddButtonListener(string name, UnityAction callBack)
        {
            Button bt = null;
            if (buttonCallBackBuff.ContainsKey(name))
            {
                if (buttonCallBackBuff[name].Contains(callBack))
                    return;

            }
            bt = GetComponent<Button>(name);
            bt.onClick.AddListener(callBack);
            if (buttonCallBackBuff.ContainsKey(name))
            {
                buttonCallBackBuff[name].Add(callBack);
            }
            else
            {
                buttonCallBackBuff.Add(name, new List<UnityAction>() { callBack });
            }

        }
        public void RemoveButtonListener(string name, UnityAction callBack)
        {
            Button bt = null;
            if (buttonCallBackBuff.ContainsKey(name))
            {
                if (buttonCallBackBuff[name].Contains(callBack))
                {
                    buttonCallBackBuff[name].Remove(callBack);

                    bt = GetComponent<Button>(name);
                    bt.onClick.RemoveListener(callBack);

                    if (buttonCallBackBuff[name].Count == 0)
                        buttonCallBackBuff.Remove(name);
                }

            }
        }
        public void RemoveButtonAllListener(string name)
        {
            Button bt = null;
            if (buttonCallBackBuff.ContainsKey(name))
            {
                bt = GetComponent<Button>(name);
                bt.onClick.RemoveAllListeners();

                buttonCallBackBuff.Remove(name);
            }
        }

        #region virtual api
        protected virtual void OnOpen(params object[] param) { }
        public virtual void OnOpenAfterAnimation() { }
        public virtual void OnExit() { }
        protected virtual void OnExitAfterAnimation() { }

        public virtual IEnumerator OpenAnimation()
        {
            yield return null;
        }
        public virtual IEnumerator ExitAnimation()
        {
            yield return null;
        }


        #endregion

        #region internal api

        internal void Init(params object[] param)
        {
            if (root)
                root.SetActive(true);
            if (backGroundRoot)
                backGroundRoot.SetActive(true);
            OnOpen(param);
        }

        internal void Closed()
        {
            if (root)
                root.SetActive(false);
            if (backGroundRoot)
                backGroundRoot.SetActive(false);
            OnExitAfterAnimation();
        }
    
        #endregion


    }
}

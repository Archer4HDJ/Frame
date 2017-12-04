using HDJ.Framework.Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Modules
{
    public class GameSceneBase
    {

        protected Dictionary<string, UIWindowBase> UIDic = new Dictionary<string, UIWindowBase>();

        public void OpenUIWindow<T>(params object[] param)
        {
            OpenUIWindow(typeof(T).Name, param);
        }

        public void OpenUIWindow(string winName, params object[] param)
        {
            UIManager.OpenWindow(winName, (win) =>
             {
                 UIDic.Add(winName, win);
             }, param);
        }
        public void CloseNodeWindow(bool isImmediately = false)
        {
            UIWindowBase win = UIManager.CloseNodeWindow(isImmediately);
            if (win)
            {
                if (UIDic.ContainsKey(win.name))
                    UIDic.Remove(win.name);
                else
                    Debug.LogError("GameScene don't hava UI Window!");
            }
        }
        public void CloseUIWindow<T>(bool isImmediately = false)
        {
            CloseUIWindow(typeof(T).Name, isImmediately);
        }
        public void CloseUIWindow(string winName, bool isImmediately = false)
        {
            if (UIDic.ContainsKey(winName))
            {
                UIManager.CloseWindow(winName, isImmediately);
                UIDic.Remove(winName);
            }
            else
            {
                Debug.LogError("GameScene don't hava UI Window!");
            }
        }


        public virtual void OnOpen() { }
        protected virtual void OnExit() { }
        public void OnExitEx()
        {
            List<UIWindowBase> wins = new List<UIWindowBase>(UIDic.Values);
            UIDic.Clear();
            for (int i = 0; i < wins.Count; i++)
            {
                UIManager.CloseWindow(wins[i], true);
            }
            OnExit();
        }
       
        public void UpdateEx()
        {
            OnUpdate();
        }
        protected virtual void OnUpdate() { }
        public void FixedUpdateEx()
        {
            OnFixedUpdate();
        }
        protected virtual void OnFixedUpdate() { }
        public void LateUpdateEx()
        {
            OnLateUpdate();
        }
        protected virtual void OnLateUpdate() { }

        public void OnGUIEx()
        {
            OnGUIUpdate();
        }
        protected virtual void OnGUIUpdate() { }


        public virtual IEnumerator OnLoadAssets()
        {
            yield return null;
        }
  

    }

}
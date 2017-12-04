using HDJ.Framework.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Modules
{
    #region define

    public enum UIType
    {
        Normal,
        Fixed,
        PopUp,
      //  None,      //独立的窗口
    }

    public enum UIMode
    {
        DoNothing, // 打开该界面时, 不加入backSequence队列
        HideOther,     // 打开该界面时, 闭其他队列的界面,加入backSequence队列
        NeedBack,      // 打开该界面时,不关闭其他界面,加入backSequence队列
       // NoNeedBack,    // 关闭TopBar,关闭其他界面,不加入backSequence队列
    }

    public enum UIWindowState
    {
        Openning,
        Ready,
        Exiting,
        Closed,
    }
    #endregion
    public static class UIManager 
    {
        //all pages with the union type
        private static Dictionary<string, UIWindowBase> m_allWindow = new Dictionary<string, UIWindowBase>();
        public static Dictionary<string, UIWindowBase> allWindow
        { get { return m_allWindow; } }

        //control 1>2>3>4>5 each page close will back show the previus page.
        private static List<UIWindowBase> m_currentWindowNodes = new List<UIWindowBase>();
        public static List<UIWindowBase> currentWindowNodes
        { get { return m_currentWindowNodes; } }


        #region static api

        private static bool CheckIfNeedBack(UIWindowBase win)
        {
            UIType type = win.type;
            UIMode mode = win.mode;
            if (type == UIType.Fixed || type == UIType.PopUp)// || type == UIType.None)
                return false;
            else if (mode == UIMode.DoNothing)// mode == UIMode.NoNeedBack ||)
                return false;
            return true;
        }

        /// <summary>
        /// make the target node to the top.
        /// </summary>
        private static void PopNode(UIWindowBase win)
        {
            //sub pages should not need back.
            if (!CheckIfNeedBack(win))
            {
                return;
            }

            bool _isFound = false;
            for (int i = 0; i < m_currentWindowNodes.Count; i++)
            {
                if (m_currentWindowNodes[i].Equals(win))
                {
                    m_currentWindowNodes.RemoveAt(i);
                    m_currentWindowNodes.Add(win);
                    _isFound = true;
                    break;
                }
            }

            //if dont found in old nodes
            //should add in nodelist.
            if (!_isFound)
            {
                m_currentWindowNodes.Add(win);
            }

            if (win.mode == UIMode.HideOther && m_currentWindowNodes.Count>=2)
            {
                //form bottm to top.
                for (int i = m_currentWindowNodes.Count - 2; i >= 0; i--)
                {
                    CloseWindow(m_currentWindowNodes[i]);
                }
            }
        }

        public static void ClearNodes()
        {
            m_currentWindowNodes.Clear();
        }

        public static void OpenWindow<T>(CallBack<UIWindowBase> callback = null, params object[] param) where T : UIWindowBase
        {
            Type t = typeof(T);
            OpenWindow(t.Name, callback, param);
        }

        public static void OpenWindow(string winName, CallBack<UIWindowBase> callback = null, params object[] param)
        {
            if (string.IsNullOrEmpty(winName))
            {
                Debug.LogError("[UI] show page error with :" + winName + " maybe null instance.");
                return;
            }

            UIWindowBase win = null;
            if (m_allWindow.ContainsKey(winName))
            {
                win = m_allWindow[winName];
            }
            else
            {
                GameObject winObj = PoolObjectManager.GetObject(winName);
                win = winObj.GetComponent<UIWindowBase>();
                UIRoot.SetUIParentByUIType(winObj, win.type);
                win.windowName = winName;
                m_allWindow.Add(winName, win);
            }
            win.Init(param);
            UIRoot.Instance.StartCoroutine(OpenningWindow(win, callback));
        }

        private static IEnumerator OpenningWindow(UIWindowBase win, CallBack<UIWindowBase> callback)
        {
            win.state = UIWindowState.Openning;
            
            yield return win.OpenAnimation();

            win.OnOpenAfterAnimation();
            PopNode(win);
            win.state = UIWindowState.Ready;

            if (callback != null)
                callback(win);
        }

        /// <summary>
        /// close current page in the "top" node.
        /// </summary>
        public static UIWindowBase CloseNodeWindow(bool isImmediately = false)
        {
            //Debug.Log("Back&Close PageNodes Count:" + m_currentPageNodes.Count);

            if ( m_currentWindowNodes.Count <= 1) return null;

            UIWindowBase closeWin = m_currentWindowNodes[m_currentWindowNodes.Count - 1];
            m_currentWindowNodes.RemoveAt(m_currentWindowNodes.Count - 1);

            //show older page.
            //TODO:Sub pages.belong to root node.
            if (m_currentWindowNodes.Count > 0)
            {
                UIWindowBase win = m_currentWindowNodes[m_currentWindowNodes.Count - 1];
                UIRoot.Instance.StartCoroutine(ClosingWindow(closeWin, isImmediately, win.windowName));  
            }

            return closeWin;
        }

        private static IEnumerator ClosingWindow(UIWindowBase closeWin,bool isImmediately =false,  string afterOpenWinName =null)
        {
            if (closeWin.state != UIWindowState.Closed)
            {
                closeWin.state = UIWindowState.Exiting;
                closeWin.OnExit();
                if (!isImmediately)
                    yield return closeWin.ExitAnimation();
                closeWin.Closed();
                closeWin.state = UIWindowState.Closed;
            }

            if (!string.IsNullOrEmpty(afterOpenWinName))
                OpenWindow(afterOpenWinName);
        }

        /// <summary>
        /// Close target page
        /// </summary>
        public static void CloseWindow(UIWindowBase target, bool isImmediately = false)
        {
            if (target == null) return;
            if (target.state == UIWindowState.Closed)
            {
                for (int i = 0; i < m_currentWindowNodes.Count; i++)
                {
                    if (m_currentWindowNodes[i] == target)
                    {
                        m_currentWindowNodes.RemoveAt(i);
                        break;
                    }
                }
                return;
            }

            if ( m_currentWindowNodes.Count >= 1 && m_currentWindowNodes[m_currentWindowNodes.Count - 1] == target)
            {
                CloseNodeWindow();
            }
            else if (CheckIfNeedBack(target))
            {
                for (int i = 0; i < m_currentWindowNodes.Count; i++)
                {
                    if (m_currentWindowNodes[i] == target)
                    {
                        m_currentWindowNodes.RemoveAt(i);
                        UIRoot.Instance.StartCoroutine(ClosingWindow(target, isImmediately));
                        return;
                    }
                }
            }

            UIRoot.Instance.StartCoroutine(ClosingWindow(target, isImmediately));
        }

        public static void CloseWindow<T>(bool isImmediately = false) where T : UIWindowBase
        {
            Type t = typeof(T);
            CloseWindow(t.Name, isImmediately);
        }

        public static void CloseWindow(string winName, bool isImmediately = false)
        {
            if (m_allWindow.ContainsKey(winName))
            {
                CloseWindow(m_allWindow[winName], isImmediately);
            }
            else
            {
                Debug.LogError(winName + " havnt show yet!");
            }
        }
        #endregion
    }
}

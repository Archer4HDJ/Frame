using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HDJ.Framework.Modules
{

    public class UIRoot : MonoSingleton<UIRoot>
    {
        public Transform root;
        public Transform fixedRoot;
        public Transform normalRoot;
        public Transform popupRoot;
        public Camera uiCamera;

        protected override void OnAwake()
        {
            gameObject.layer = LayerMask.NameToLayer("UI");
            gameObject.AddComponent<RectTransform>();

            Canvas can = gameObject.AddComponent<Canvas>();
            can.renderMode = RenderMode.ScreenSpaceCamera;
            can.pixelPerfect = true;

            gameObject.AddComponent<GraphicRaycaster>();

            root = gameObject.transform;

            GameObject camObj = new GameObject("UICamera");
            camObj.layer = LayerMask.NameToLayer("UI");
            camObj.transform.parent = gameObject.transform;
            camObj.transform.localPosition = new Vector3(0, 0, -100f);
            Camera cam = camObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Depth;
            cam.orthographic = true;
            cam.farClipPlane = 200f;
            can.worldCamera = cam;
            cam.cullingMask = 1 << 5;
            cam.nearClipPlane = -50f;
            cam.farClipPlane = 50f;

            uiCamera = cam;

            CanvasScaler cs = gameObject.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1136f, 640f);
            cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            GameObject subRoot;

            subRoot = CreateSubCanvasForRoot(gameObject.transform, 0);
            subRoot.name = "NormalRoot";
            normalRoot = subRoot.transform;
            normalRoot.transform.localScale = Vector3.one;

            subRoot = CreateSubCanvasForRoot(gameObject.transform, 250);
            subRoot.name = "FixedRoot";
            fixedRoot = subRoot.transform;
            fixedRoot.transform.localScale = Vector3.one;

            subRoot = CreateSubCanvasForRoot(gameObject.transform, 500);
            subRoot.name = "PopupRoot";
            popupRoot = subRoot.transform;
            popupRoot.transform.localScale = Vector3.one;
            //add Event System
            GameObject esObj = GameObject.Find("EventSystem");
            if (esObj != null)
            {
                if (Application.isPlaying)
                    Destroy(esObj);
                else
                    DestroyImmediate(esObj);
            }

            GameObject eventObj = new GameObject("EventSystem");
            eventObj.layer = LayerMask.NameToLayer("UI");
            eventObj.transform.SetParent(gameObject.transform);
            eventObj.AddComponent<EventSystem>();
            eventObj.AddComponent<StandaloneInputModule>();

            if (Application.isPlaying)
            {
                DontDestroyOnLoad(eventObj);
                DontDestroyOnLoad(this);
            }
        }
        private GameObject CreateSubCanvasForRoot(Transform root, int sort)
        {
            GameObject go = new GameObject("canvas");
            go.transform.parent = root;
            go.layer = LayerMask.NameToLayer("UI");

            RectTransform rect = go.AddComponent<RectTransform>();
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;

            //  Canvas can = go.AddComponent<Canvas>();
            //  can.overrideSorting = true;
            //  can.sortingOrder = sort;
            //  go.AddComponent<GraphicRaycaster>();

            return go;
        }

        public static void CreateUIFrame()
        {
            if (UIRoot.Instance.root == null)
                UIRoot.Instance.OnAwake();
        }

        public static void SetUIParentByUIType(GameObject ui, UIType type)
        {

            if (type == UIType.Fixed)
            {
                ui.transform.SetParent(UIRoot.Instance.fixedRoot);
            }
            else if (type == UIType.Normal)
            {
                ui.transform.SetParent(UIRoot.Instance.normalRoot);
            }
            else if (type == UIType.PopUp)
            {
                ui.transform.SetParent(UIRoot.Instance.popupRoot);
            }

        }

    }
}

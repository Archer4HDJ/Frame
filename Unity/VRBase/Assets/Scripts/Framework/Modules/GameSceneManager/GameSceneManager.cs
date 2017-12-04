using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HDJ.Framework.Utils;
using System;

namespace HDJ.Framework.Modules
{
    public class GameSceneManager : MonoBehaviourExtend
    {

        private static Dictionary<string, GameSceneBase> allScenes = new Dictionary<string, GameSceneBase>();
        private static GameSceneManager instance;

        public static GameSceneBase currentScene;
        public static string[] AllSceneNames
        {
            get
            {
                return new List<string>(allScenes.Keys).ToArray();
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            GameObject obj = new GameObject("[GameSceneManager]");
            instance = obj.AddComponent<GameSceneManager>();
            DontDestroyOnLoad(obj);

            Type[] types = ReflectionUtils.GetChildTypes(typeof(GameSceneBase));
            for (int i = 0; i < types.Length; i++)
            {
                object temp = ReflectionUtils.CreateDefultInstance(types[i]);
                allScenes.Add(types[i].Name, (GameSceneBase)temp);
            }
        }

        public static void OpenScene<T>()
        {
            OpenScene(typeof(T).Name);
        }

        public static void OpenScene(string sceneName)
        {
            if (allScenes.ContainsKey(sceneName))
            {
                if (isChangingScene)
                {
                    Debug.LogError(" is Changing Scene,can't Change to :" + sceneName);
                    return;
                }

                instance.StartCoroutine(ChangeScene(sceneName));
            }
            else
            {
                Debug.LogError("GameFlowControlSystem.OpenScene :" + sceneName + "  failed!, no such Scene");
            }
        }
        private static bool isChangingScene = false;
        private static IEnumerator ChangeScene(string nextScene)
        {
            isChangingScene = true;
            if (currentScene != null)
            {
                currentScene.OnExitEx();
            }

            currentScene = allScenes[nextScene];

            currentScene.OnOpen();
            yield return currentScene.OnLoadAssets();

            isChangingScene = false;
        }

        protected override void OnFixedUpdate()
        {
            if (currentScene != null)
            {
                currentScene.FixedUpdateEx();
            }
        }

        protected override void OnLateUpdate()
        {
            if (currentScene != null)
            {
                currentScene.LateUpdateEx();
            }
        }

        protected override void OnUpdate()
        {
            if (currentScene != null)
            {
                currentScene.UpdateEx();
            }
        }

        protected override void OnGUIUpdate()
        {
            if (currentScene != null)
            {
                currentScene.OnGUIEx();
            }
        }
    }
}

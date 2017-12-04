using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using HDJ.Framework.Tools;

namespace HDJ
{
    public class SceneLoadingController : MonoBehaviour
    {

        private static string nextSceneName;
        private UISceneLoadingBase loadingUI;

        IEnumerator Start()
        {
            loadingUI = GetComponent<UISceneLoadingBase>();

            CameraFade.Instance.FadeOut(1f);
            yield return new WaitForSeconds(0.9f);

            StartCoroutine(LoadingStart());

        }

        public static void Loading(string nextSceneName)
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneLoadingController.nextSceneName = nextSceneName;


                CameraFade.Instance.FadeIn(1f, () =>
                {
                    SceneManager.LoadScene("LoadingScene");
                });

            }
        }
        AsyncOperation op = null;
        bool isDone = false;
        IEnumerator LoadingStart()
        {
            op = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
            yield return op;
 
        }

        private void Update()
        {
            if (isDone)
                return;
            if (op == null)
                return;
            if (op.progress < 0.89f)
            {
                    if (loadingUI)
                        loadingUI.UpdateLoadingUI(op.progress);
            }
            else
            {
                if (loadingUI)
                    loadingUI.UpdateLoadingUI(1f);
                    isDone = true;
            }
        }
    }
}

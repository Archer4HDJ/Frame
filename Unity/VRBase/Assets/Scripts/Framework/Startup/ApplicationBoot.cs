using HDJ.Framework.Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationBoot : MonoBehaviour {
    public AssetsLoadType assetsLoadType = AssetsLoadType.Resources;
    public  StartupUIShowController uishow;

    // Use this for initialization
    void Awake () {

        ResourcesManager.assetsLoadType = assetsLoadType;

        if (assetsLoadType == AssetsLoadType.Resources)
        {
            SetGameSetting();
        }
        else
        {
            PreLoadResourceController.progressCallBack = uishow.ShowProgress;
            PreLoadResourceController.showContentCallBack = uishow.ShowContent;
            UpdateResourseController.progressCallBack = uishow.ShowProgress;
            UpdateResourseController.showContentCallBack = uishow.ShowContent;

            string serverPath = GameSettingStoreManager.GetValue( "ServerPath","");
            if (!string.IsNullOrEmpty(serverPath))
                UpdateResourseController.StartUpdating(this, serverPath, SetGameSetting);
            else
                SetGameSetting();
        }

    }
    void Start()
    {

     
   
    }

    void SetGameSetting()
    {
        InternalConfigManager.ReleaseAll();
        ResourcesManager.ReleaseAll();
        ResourcePathManager.ReLoadData();
        GameSettingStoreManager.Clear();
        Debug.unityLogger.logEnabled = GameSettingStoreManager.GetValue( "Debug",true);
        Application.runInBackground = GameSettingStoreManager.GetValue( "runInBackground",true);

        if (assetsLoadType == AssetsLoadType.AssetBundle)
        {
            PreLoadResourceController.StartLoadResource(IntoGameScene);
        }
        else
        {
            IntoGameScene();
        }
    }

    void IntoGameScene () {

        SceneManager.LoadScene("Game");
    }
}

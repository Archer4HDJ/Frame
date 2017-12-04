using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleSettingData : ScriptableObject
{

    //打成一个bundle包的目录
    public List<string> packageOnePaths = new List<string>();
    //要打包的资源文件后缀
    public List<string> packageFileExtension = new List<string>();
    //需要预加载的目录
    public List<string> preLoadResPaths = new List<string>();


    public void Init()
    {

        
    }
    public void Reset()
    {


    }

}

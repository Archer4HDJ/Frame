using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using HDJ.Framework.Utils;
using HDJ.Framework.Modules;

public class CreateConfigClassEditor :Editor {

	
     [MenuItem("Tool/Table配置文件/选择的配置文件生成对应的Class文件", priority = 1102)]
	static void CreateSelectConfigFileToClass () {
        var select = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(select);
        string fileName = Path.GetFileNameWithoutExtension(path);
        TextAsset textA = select as TextAsset;
        CreateConfigClassFile(textA.text, fileName);
    
     AssetDatabase.Refresh();

	}
    public const string SaveConfigFilePath = "Assets/Resources/Configs/TableConfig/";
    [MenuItem("Tool/Table配置文件/所有的配置文件生成对应的Class文件", priority = 1103)]
     static void CreateAllConfigFileToClass()
     {
         string[] temp = PathUtils.GetDirectoryFileNames(SaveConfigFilePath, new string[] { ".txt" });
         foreach (string name in temp)
         {
            AssetData[] res = ResourcesManager.LoadAssetsByName(name);
            if (res.Length>0 && res[0].asset)
            {
                TextAsset textA = res[0].asset as TextAsset;

                CreateConfigClassFile(textA.text, name);
            }
         }
         AssetDatabase.Refresh();

     }

    private const string SaveConfigClassPath = "Assets/Scripts/Game/ConfigClass/";
    public static void CreateConfigClassFile(string configFileData,string fileName)
    {
        string data = ParseConfigData(configFileData, fileName);

        string savePath = SaveConfigClassPath + fileName + ".cs";
        //Debug.Log("data :" + data);
        FileUtils.CreateTextFile(savePath, data);
    }

     private static string ParseConfigData(string data, string className)
     {
         //回车换行
         string[] temp0 = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        //Debug.Log("data :" + data);
         string[] needData = new string[3];
         if (temp0.Length >= 3)
         {
             for (int i = 1; i < 4; i++)
             {
                 needData[i-1] = temp0[i];
             }
         }

         //temp0.CopyTo(needData, 0);

         string[][] temp1 = new string[needData.Length][];

         for (int i = 0; i < needData.Length; i++)
         {
            //Debug.Log(needData[i]);
             //制表符
             temp1[i] = needData[i].Split(new char[] { '\t' });
         }


         string classString = "using UnityEngine;\r\n";
        classString += "using HDJ.Framework.Modules;\r\n";
         classString +="// 自动生成请勿更改\r\n";
        classString += "\t/// <summary>\r\n \t/// " + temp0[0] + "\r\n\t/// </summary>\r\n";
        classString +="public class " + className + " : "+ typeof( TableConfigBase).Name+ "\r\n{ \r\n";

         for (int j = 0; j < temp1[0].Length; j++)
         {
            classString+= "\t/// <summary>\r\n \t/// " + temp1[1][j] + "\r\n\t/// </summary>\r\n";
            classString += "\t public " + temp1[0][j] + " " + temp1[2][j] + ";\r\n";
         }
         classString += "}";

         //Debug.Log("classString :" + classString);
         return classString;
     }
}

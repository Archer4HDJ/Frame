using HDJ.Framework.Modules;
using HDJ.Framework.Game;
using HDJ.Framework.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

using UnityEngine;

public class MapEditorWindow : EditorWindow
{
    public static MapEditorWindow instance = null;
    [MenuItem("Tool/地图编辑窗口(1200)",priority = 1200)]
    static void OpenWindow()
    {
        instance = GetWindow<MapEditorWindow>();
        instance .Initialize();
        MapPrefabSetUpdater.CheckMapPrefabForder();
        MapEditorWindowFunction.CreateMapScene();


    }

    Color oldColor;
    private void Initialize()
    {
        Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
        MapManager.Init();
        oldColor = GUI.color;
    }
    private void OnEnable()
    {
        MapManager.Clear();
        currentLoadMap = null;
        instance = this;
        Initialize();
        CheckScenetMapRoot();
    }
    private GameObject mapRoot;
    public void CheckScenetMapRoot()
    {
        if (mapRoot == null)
        {
           mapRoot =  GameObject.Find(MapUseConstValues.MapRootObjectName);
        }
        if (mapRoot == null) return;
        MapManager.mapRoot = mapRoot;
        if (mapRoot.transform.childCount == 0)
        {
            return;
        }
        Transform idRoot = mapRoot.transform.GetChild(0);
       
        int id = int.Parse(idRoot.name);
        List<MapInfomation> mapInfos = new List<MapInfomation>(MapManager.MapInfosDic.Values);
        foreach (MapInfomation info in mapInfos)
        {
            if (info.id == id)
            {
                currentLoadMap = info;
                break;
            }
        }
        mapNameRoot = idRoot.Find(MapUseConstValues.MapObjectsObjectName);
        mapLightRoot = idRoot.Find(MapUseConstValues.LightPrefabNamePrefix + MapManager.MapInfosDic[currentLoadMap.id].mapFileName);
        mapColliderRoot = idRoot.Find(MapUseConstValues.ColliderPrefabNamePrefix + MapManager.MapInfosDic[currentLoadMap.id].mapFileName);
    }
    private int toolbarOption = 0;
    private string[] toolbarTexts = { "所有地图", "新建地图" };
    private void OnGUI()
    {
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));
        switch (toolbarOption)
        {
            case 0:
                ShowAllMapInfoGUI();
                if (currentLoadMap!=null && GUILayout.Button("保存"))
                {
                   MapInfomation mapInfo = MapManager.MapInfosDic[currentLoadMap.id];
                    if (mapInfo.isCreateLightMap)
                    {
                        mapInfo.mapLightMap = MapUseConstValues.LightMapFileNamePrefix + mapInfo.mapFileName;
                        MapEditorWindowFunction.BakeLightMap(() =>
                        {
                            GetCurrentEditMapRoot();
                            SaveLightMapInfo();
                        });
                        
                    }
                    SaveMapInfoData();
                    SaveMapData();
                    
                    AssetDatabase.Refresh();
                }
                break;
            case 1:
                NewMapFileGUI();
                break;
        }

    }
    string newFileName = "";
    bool NewMapFileGUI()
    {
        GUILayout.BeginHorizontal("box");
        newFileName = EditorDrawGUIUtil.DrawBaseValue("新建地图文件", newFileName).ToString();
        if (GUILayout.Button("确定", GUILayout.Width(50)))
        {
            if (ResourcePathManager.ContainsFileName(newFileName))
            {
                EditorUtility.DisplayDialog("错误", "文件名与其他文件重复", "OK");
                return true;
            }
            MapInfomation info = new MapInfomation();
            info.id = GetMaxID() + 1;
            //info.sceneName = "Scene_" + newFileName;
            info.mapFileName = MapUseConstValues .MapFileNamePrefix+ newFileName;
            MapManager.MapInfosDic.Add(info.id, info);

            GlobalEvent.AddEvent(EditorGlobalEventEnum.OnResourcesAssetsChange, (t) =>
            {
                Initialize();
            }, true);
            SaveMapInfoData();
         
            FileUtils.CreateTextFile(MapUseConstValues.MapDataDirectory + info.mapFileName+"/" + info.mapFileName + ".txt", "");
            AssetDatabase.Refresh();
           
            return false;
        }
        GUILayout.EndHorizontal();
        return true;
    }

    private  MapInfomation currentLoadMap =null;
    private Vector2 pos = Vector2.zero;

    private string[] lightMapSizeNames = new string[] { "32", "64", "128", "256", "1024", "2048", "4096" };
    private int[] lightMapSizeValue = new int[] {32,64,128,256,1024,2048,4096 };
    void ShowAllMapInfoGUI()
    {
        List<MapInfomation> mapInfos = new List<MapInfomation>(MapManager.MapInfosDic.Values);
        pos = GUILayout.BeginScrollView(pos, true, true);
        foreach(MapInfomation info in mapInfos)
        {
            bool isOpen = (currentLoadMap== info);

            if (isOpen) GUI.color = Color.red;
            


            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("box");
            GUILayout.Label("ID: " + info.id);
            //GUILayout.Label("Scene: " + info.sceneName);
            GUILayout.Label("MapFile: " + info.mapFileName);
            GUILayout.Space(4);
            GUILayout.Box("Light Map :");
            info.isCreateLightMap = (bool)EditorDrawGUIUtil.DrawBaseValue("是否生成光照贴图", info.isCreateLightMap);
            if (info.isCreateLightMap)
            {
                info.lightMapSize = EditorGUILayout.IntPopup("光照贴图单张最大尺寸:",info.lightMapSize, lightMapSizeNames, lightMapSizeValue);
                LightmapEditorSettings.maxAtlasHeight = info.lightMapSize;
                LightmapEditorSettings.maxAtlasWidth = info.lightMapSize;
                GUILayout.Label("光照贴图文件: " + info.mapLightMap);
            }

            GUILayout.Space(4);

            GUILayout.Label("Collider File :" + info.mapColliderInfoFileName);

            GUILayout.EndVertical();
            GUILayout.BeginVertical("box");
            if (GUILayout.Button(isOpen?"卸载地图": "加载地图",GUILayout.Width(120)))
            {
                if (!isOpen)
                {      
                    if(currentLoadMap != null)
                    {
                        if (EditorUtility.DisplayDialog("警告", "是否切换地图，请记得先保存！", "OK", "Cancel"))
                        {
                            MapManager.DestoryMapGameObjectInScene(currentLoadMap.id);
                            currentLoadMap = null;
                        }
                        else
                        {
                            return;
                        }
                    }
                    currentLoadMap=info;
                    MapManager.CreateMapGameObjectInScene(info.id);
                    GetCurrentEditMapRoot();
                }
                else
                {
                    if (EditorUtility.DisplayDialog("警告", "是否卸载地图，请记得先保存！", "OK", "Cancel"))
                    {
                        MapManager.DestoryMapGameObjectInScene(info.id);
                        currentLoadMap=null;
                    }
                }

            }
          
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (currentLoadMap==null && GUILayout.Button("删除"))
            {
                if (EditorUtility.DisplayDialog("警告", "是否删除地图:"+ info.mapFileName, "OK", "Cancel"))
                {
                    MapManager.MapInfosDic.Remove(info.id);
                    GlobalEvent.AddEvent(EditorGlobalEventEnum.OnResourcesAssetsChange, (t) =>
                    {
                        Initialize();
                    }, true);
                    SaveMapInfoData();

                    FileUtils.DeleteFile(MapUseConstValues.MapDataDirectory+ info.mapFileName+"/" + info.mapFileName + ".txt");
                    AssetDatabase.Refresh();
                    return;
                }
            }

            GUI.color = oldColor;
        }
        GUILayout.EndScrollView();
    }

    public Transform mapNameRoot;
    public Transform mapLightRoot;
    public Transform mapColliderRoot;
    
    public Transform GetCurrentEditMapRoot()
    {
        if (MapManager.mapRoot == null || currentLoadMap ==null)
            return null;
      
           Transform idRoot = MapManager.mapRoot.transform.Find(currentLoadMap.id.ToString());
        mapNameRoot = idRoot.Find(MapUseConstValues.MapObjectsObjectName);
        mapLightRoot = idRoot.Find(MapUseConstValues.LightPrefabNamePrefix + MapManager. MapInfosDic[currentLoadMap.id].mapFileName);
        mapColliderRoot = idRoot.Find(MapUseConstValues .ColliderPrefabNamePrefix+ MapManager.MapInfosDic[currentLoadMap.id].mapFileName);
        return idRoot;
    }

    void SaveMapInfoData()
    {
        List<MapInfomation> mapInfos = new List<MapInfomation>(MapManager.MapInfosDic.Values);
        string ss = JsonUtils.ListToJson(mapInfos);
        FileUtils.CreateTextFile(MapUseConstValues.MapDataDirectory+ MapUseConstValues .MapInfomationFileName+ ".txt", ss);
    }
    void SaveMapData()
    {
        string data = GetSceneInfo();

        MapInfomation info = MapManager.MapInfosDic[currentLoadMap.id];
        string path = MapUseConstValues.MapDataDirectory  + info.mapFileName + "/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
        PrefabUtility.CreatePrefab(path + mapLightRoot.name + ".prefab", mapLightRoot.gameObject, ReplacePrefabOptions.ConnectToPrefab);
        PrefabUtility.CreatePrefab(path + mapColliderRoot.name + ".prefab", mapColliderRoot.gameObject, ReplacePrefabOptions.ConnectToPrefab);
        FileUtils.CreateTextFile(path + info.mapFileName + ".txt", data);

    }

    private int GetMaxID()
    {
        List<int> ids = new List<int>(MapManager.MapInfosDic.Keys);
        if (ids.Count == 0)
            return -1;
        int temp = ids[0];
        foreach(int i in ids)
        {
            if (i >= temp)
                temp = i;
        }
        return temp;
    }
    /// <summary>
    /// 将一个场景物体转换为信息（string）
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public string GetSceneInfo()
    {
        MapWorld mapWorld = new MapWorld();

        List<MapObject> mapobjs = new List<MapObject>();
        int childCount = mapNameRoot.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform item = mapNameRoot.GetChild(i);
            MapObject mop = new MapObject();
            mop.name = item.name;
            mop.isStatic = item.gameObject.isStatic;
            mop.transformInfo.position = item.localPosition;
            mop.transformInfo.rotation = item.localRotation.eulerAngles;
            mop.transformInfo.scale = item.localScale;

            MonoBehaviour[] mos = item.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour m in mos)
            {
                ClassValue cv = new ClassValue(m);
                mop.behavirComponets.Add(cv);
            }
            mapobjs.Add(mop);
        }
        mapWorld.mapObjects = mapobjs;

        mapWorld.lightPrefab = mapLightRoot.name;
        return JsonUtils.ClassOrStructToJson(mapWorld);
    }
    public string GetLightMapInfo()
    {
        LightMapObject lightmapObj = new LightMapObject();
        int childCount = mapNameRoot.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform item = mapNameRoot.GetChild(i);
            MapObjectRendererLightMapInfo renderInfo = new MapObjectRendererLightMapInfo();
            if (item.gameObject.isStatic)
            {
                MeshRenderer[] renders = item.GetComponentsInChildren<MeshRenderer>();
                foreach (var r in renders)
                {
                    RendererLightMapInfo rInfo = new RendererLightMapInfo();
                    rInfo.lightmapIndex = r.lightmapIndex;
                    rInfo.lightmapScaleOffset = r.lightmapScaleOffset;
                    renderInfo.renderLightMapInfos.Add(rInfo);
                }
            }

            lightmapObj.mapObjectRendererLightMapInfo.Add(renderInfo);
        }

        lightmapObj.lightmapsMode = LightmapSettings.lightmapsMode;

        lightmapObj.fogInfo.fog = RenderSettings.fog;
        lightmapObj.fogInfo.fogMode = RenderSettings.fogMode;
        lightmapObj.fogInfo.fogColor = RenderSettings.fogColor;
        lightmapObj.fogInfo.fogStartDistance = RenderSettings.fogStartDistance;
        lightmapObj.fogInfo.fogEndDistance = RenderSettings.fogEndDistance;

        //Debug.Log("customReflection :" + AssetDatabase.GetAssetPath(RenderSettings.customReflection));
        for (int i = 0; i < LightmapSettings.lightmaps.Length; i++)
        {
            LightmapData data = LightmapSettings.lightmaps[i];
            if (data.lightmapDir != null)
            {
                lightmapObj.lightmapDir.Add(MapUseConstValues.LightmapDirNamePrefix + MapManager.MapInfosDic[currentLoadMap.id].mapFileName + "_" + i);
            }

            if (data.lightmapColor != null)
            {
                lightmapObj.lightmapColor.Add(MapUseConstValues .LightmapColorPrefix+ MapManager.MapInfosDic[currentLoadMap.id].mapFileName + "_" + i);
            }
        }

        ReflectionProbe[] reflectionProbes = mapLightRoot.gameObject.GetComponentsInChildren<ReflectionProbe>();
        foreach (var item in reflectionProbes)
        {
            ReflectionProbeInfo info = new ReflectionProbeInfo();
            info.gameObjectName = item.gameObject.name;
            if (item.bakedTexture != null)
            {
               string sPath=  AssetDatabase.GetAssetPath(item.bakedTexture);
                info.reflectionProbeFileName= Path.GetFileNameWithoutExtension(sPath);
            }
            else
            {
                info.reflectionProbeFileName = "";
            }
            lightmapObj.reflectionProbeInfos.Add(info);
        }

        return JsonUtils.ClassOrStructToJson(lightmapObj);
    }

    private void SaveLightMapInfo()
    {
       

        MapInfomation info = MapManager.MapInfosDic[currentLoadMap.id];
        string path = MapUseConstValues.MapDataDirectory  + info.mapFileName + "/LightMap/";
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        Directory.CreateDirectory(path);
        AssetDatabase.Refresh();
    
        string mapLightDir = "";
        for (int i = 0; i < LightmapSettings.lightmaps.Length; i++)
        {
            LightmapData data = LightmapSettings.lightmaps[i];
            if (data.lightmapDir != null)
            {
                Texture2D tempTex = data.lightmapDir;

                string texPath = AssetDatabase.GetAssetPath(tempTex);
                if (string.IsNullOrEmpty(mapLightDir))
                {
                    mapLightDir = Path.GetDirectoryName(texPath);
                    //Debug.Log("mapLightDir :" + mapLightDir);
                }
                string temp = path + MapUseConstValues.LightmapDirNamePrefix + MapManager.MapInfosDic[currentLoadMap.id].mapFileName + "_" + i + ".png";
                AssetDatabase.MoveAsset(texPath, temp);
            }

            if (data.lightmapColor != null)
            {
                Texture2D tempTex = data.lightmapColor;

                string texPath = AssetDatabase.GetAssetPath(tempTex);
                //TextureImporter import = AssetImporter.GetAtPath(texPath) as TextureImporter;
                //Debug.Log("Name :" + tempTex.name);
                //Debug.Log("textureType :" + import.textureType);
                //Debug.Log("textureShape :" + import.textureShape);
                //import.isReadable = true;
                //import.textureCompression = TextureImporterCompression.Uncompressed;
                //import.SaveAndReimport();
                string temp = path + MapUseConstValues.LightmapColorPrefix + MapManager.MapInfosDic[currentLoadMap.id].mapFileName + "_" + i + ".exr";
                AssetDatabase.MoveAsset(texPath, temp);
            }
        }
        AssetDatabase.Refresh();

        string[] exrPaths = PathUtils.GetDirectoryFilePath(mapLightDir, new string[] { ".png", ".exr" });
        for (int i = 0; i < exrPaths.Length; i++)
        {
            string temp = exrPaths[i];
            string name = Path.GetFileNameWithoutExtension(temp);
            string extension = Path.GetExtension(temp);
            name = name + "_" + MapManager.MapInfosDic[currentLoadMap.id].mapFileName + "_" + i + extension;
            string newPath = path + name;
            AssetDatabase.MoveAsset(temp, newPath);
        }
        AssetDatabase.Refresh();

        string lightmapData = GetLightMapInfo();
        FileUtils.CreateTextFile(path + MapUseConstValues.LightMapFileNamePrefix + info.mapFileName + ".txt", lightmapData);
    }
    void OnDestroy()
    {
        MapManager.Clear();
    }
  
}

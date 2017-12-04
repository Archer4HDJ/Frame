using HDJ.Framework.Modules;
using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Game
{
    public class MapManager
    {
        private static Dictionary<int, MapInfomation> mapInfosDic = new Dictionary<int, MapInfomation>();
        private static Dictionary<int, MapWorld> mapDataBuffer = new Dictionary<int, MapWorld>();
        private static Dictionary<int, LightMapObject> lightMapObjectDic = new Dictionary<int, LightMapObject>();
        private static Dictionary<int, List<GameObject>> mapInstanceBuffer = new Dictionary<int, List<GameObject>>();


        public static Dictionary<int, MapInfomation> MapInfosDic { get { return mapInfosDic; } set { mapInfosDic = value; } }

        public static void Init()
        {
            MapInfosDic.Clear();
            string s = ResourcesManager.LoadTextFileByName(MapUseConstValues.MapInfomationFileName);
            List<MapInfomation> m_infos = JsonUtils.JsonToList<MapInfomation>(s);
            if (m_infos == null)
                return;
            for (int i = 0; i < m_infos.Count; i++)
            {
                MapInfosDic.Add(m_infos[i].id, m_infos[i]);
            }
        }
        public static MapWorld GetMapObject(int id, out LightMapObject lightMapObj)
        {
            MapInfomation mapInfo = MapInfosDic[id];
            if (!Application.isPlaying)
            {
                mapDataBuffer.Clear();
                lightMapObjectDic.Clear();
            }
            if (mapDataBuffer.ContainsKey(id))
            {
                if (mapInfo.isCreateLightMap)
                {
                    lightMapObj = lightMapObjectDic[id];
                }
                else
                {
                    lightMapObj = null;
                }
                return mapDataBuffer[id];
            }


            if (!string.IsNullOrEmpty(mapInfo.mapFileName))
            {
                string s = ResourcesManager.LoadTextFileByName(mapInfo.mapFileName);
                MapWorld temp = JsonUtils.JsonToClassOrStruct<MapWorld>(s);
                if (temp == null)
                    temp = new MapWorld();
                mapDataBuffer.Add(id, temp);
                if (mapInfo.isCreateLightMap)
                {
                    s = ResourcesManager.LoadTextFileByName(mapInfo.mapLightMap);
                    LightMapObject lt = JsonUtils.JsonToClassOrStruct<LightMapObject>(s);
                    lightMapObjectDic.Add(id, lt);
                    lightMapObj = lt;
                }
                else
                {
                    lightMapObj = null;
                }
                return temp;
            }
            lightMapObj = null;
            return null;

        }

        public static GameObject mapRoot;



        /// <summary>
        /// 生成场景物体
        /// </summary>
        /// <param name="id"></param>
        public static void CreateMapGameObjectInScene(int id)
        {
            List<GameObject> listObjs = new List<GameObject>();
            LightMapObject lightMapObject;
            MapWorld mapWorld = GetMapObject(id, out lightMapObject);
            if (mapWorld == null)
            {
                mapWorld = new MapWorld();
            }
            List<MapObject> mapObjs = mapWorld.mapObjects;
            if (mapRoot == null)
            {
                mapRoot = new GameObject(MapUseConstValues.MapRootObjectName);
            }
            Transform idRoot = mapRoot.transform.Find(id.ToString());
            if (idRoot == null)
            {
                idRoot = new GameObject(id.ToString()).transform;
                idRoot.SetParent(mapRoot.transform);
            }
            if (mapObjs == null)
            {
                mapObjs = new List<MapObject>();
                return;
            }
            GameObject mapObj = new GameObject(MapUseConstValues.MapObjectsObjectName);
            mapObj.transform.SetParent(idRoot);
            string lightPrefabName = MapUseConstValues.LightPrefabNamePrefix + MapInfosDic[id].mapFileName;
            GameObject mapLights = null;
            mapLights = PoolObjectManager.GetObject(lightPrefabName);
            if (mapLights == null)
                mapLights = new GameObject(lightPrefabName);
            mapLights.transform.SetParent(idRoot);

            string colliderPrefabName = MapUseConstValues.ColliderPrefabNamePrefix + MapInfosDic[id].mapFileName;
            GameObject mapColliderObj = null;
            mapColliderObj = PoolObjectManager.GetObject(colliderPrefabName);
            if (mapColliderObj == null)
                mapColliderObj = new GameObject(colliderPrefabName);
            mapColliderObj.transform.SetParent(idRoot);

            if (lightMapObject != null)
            {
                LightmapSettings.lightmapsMode = lightMapObject.lightmapsMode;

                RenderSettings.fog = lightMapObject.fogInfo.fog;
                RenderSettings.fogMode = lightMapObject.fogInfo.fogMode;
                RenderSettings.fogColor = lightMapObject.fogInfo.fogColor;
                RenderSettings.fogStartDistance = lightMapObject.fogInfo.fogStartDistance;
                RenderSettings.fogEndDistance = lightMapObject.fogInfo.fogEndDistance;
                RenderSettings.fogDensity = lightMapObject.fogInfo.fogDensity;

                List<Texture2D> lightmapDir = new List<Texture2D>();
                for (int i = 0; i < lightMapObject.lightmapDir.Count; i++)
                {
                    string name = lightMapObject.lightmapDir[i];
                    Texture2D tex = ResourcesManager.LoadUnityAssetByName<Texture2D>(name);
                    lightmapDir.Add(tex);
                }
                List<Texture2D> lightmapColor = new List<Texture2D>();
                for (int i = 0; i < lightMapObject.lightmapColor.Count; i++)
                {
                    string name = lightMapObject.lightmapColor[i];
                    Texture2D tex = ResourcesManager.LoadUnityAssetByName<Texture2D>(name);
                    lightmapColor.Add(tex);
                }

                LightmapData[] lightmapData = new LightmapData[lightmapDir.Count > lightmapColor.Count ? lightmapDir.Count : lightmapColor.Count];
                for (int i = 0; i < lightmapData.Length; i++)
                {
                    lightmapData[i] = new LightmapData();
                    lightmapData[i].lightmapColor = i < lightmapColor.Count ? lightmapColor[i] : null;
                    lightmapData[i].lightmapDir = i < lightmapDir.Count ? lightmapDir[i] : null;
                }
                LightmapSettings.lightmaps = lightmapData;
            }

            for (int i = 0; i < mapObjs.Count; i++)
            {
                MapObject mp = mapObjs[i];
                GameObject obj = PoolObjectManager.GetObject(mp.name);
                obj.isStatic = mp.isStatic;
                obj.transform.SetParent(mapObj.transform);
                obj.transform.localPosition = mp.transformInfo.position;
                obj.transform.localRotation = Quaternion.Euler(mp.transformInfo.rotation);
                obj.transform.localScale = mp.transformInfo.scale;

                for (int j = 0; j < mp.behavirComponets.Count; j++)
                {
                    ClassValue cv = mp.behavirComponets[j];
                    cv.GetValue((type)=>
                    {
                        object v = obj.GetComponent(type);
                        if (v == null)
                            v = obj.AddComponent(type);
                        return v;
                    });
                }
                listObjs.Add(obj);

                if (lightMapObject != null && obj.isStatic)
                {
                    MapObjectRendererLightMapInfo renderInfo = lightMapObject.mapObjectRendererLightMapInfo[i];

                    MeshRenderer[] msR = obj.GetComponentsInChildren<MeshRenderer>();
                    for (int j = 0; j < msR.Length; j++)
                    {
                        RendererLightMapInfo r = renderInfo.renderLightMapInfos[j];
                        msR[j].lightmapIndex = r.lightmapIndex;
                        msR[j].lightmapScaleOffset = r.lightmapScaleOffset;
                    }
                }
            }
            if (lightMapObject != null)
            {
                Dictionary<string, string> reflectionProbesInfoDic = new Dictionary<string, string>();
                for (int i = 0; i < lightMapObject.reflectionProbeInfos.Count; i++)
                {
                    ReflectionProbeInfo info = lightMapObject.reflectionProbeInfos[i];
                    reflectionProbesInfoDic.Add(info.gameObjectName, info.reflectionProbeFileName);
                }
                ReflectionProbe[] reflectionProbes = mapLights.GetComponentsInChildren<ReflectionProbe>();
                for (int i = 0; i < reflectionProbes.Length; i++)
                {
                    ReflectionProbe re = reflectionProbes[i];
                    string name = reflectionProbesInfoDic[re.gameObject.name];
                    Texture tex = ResourcesManager.LoadUnityAssetByName<Texture>(name);
                    re.bakedTexture = tex;
                }
            }

            if (!mapInstanceBuffer.ContainsKey(id))
                mapInstanceBuffer.Add(id, listObjs);
        }


        /// <summary>
        /// 摧毁场景物体
        /// </summary>
        /// <param name="id"></param>
        public static void DestoryMapGameObjectInScene(int id)
        {
            if (mapInstanceBuffer.ContainsKey(id))
            {
                List<GameObject> listObjs = mapInstanceBuffer[id];
                mapInstanceBuffer.Remove(id);
                for (int i = 0; i < listObjs.Count; i++)
                {
                    PoolObjectManager.DestroyObject(listObjs[i]);
                }
            }
            if (mapRoot == null)
            {
                mapRoot = GameObject.Find(MapUseConstValues.MapRootObjectName);
            }
            if (mapRoot == null)
                return;
            Transform idRoot = mapRoot.transform.Find(id.ToString());
            if (idRoot != null)
            {
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(idRoot.gameObject);
                else
                    UnityEngine.Object.DestroyImmediate(idRoot.gameObject);
            }
        }
        public static void Clear()
        {
            List<int> list = new List<int>(mapInstanceBuffer.Keys);
            foreach (int i in list)
            {
                DestoryMapGameObjectInScene(i);
            }

            mapInstanceBuffer.Clear();
            mapDataBuffer.Clear();
            mapInfosDic.Clear();

            if (mapRoot)
            {
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(mapRoot);
                else
                    UnityEngine.Object.DestroyImmediate(mapRoot);
            }
        }
    }

}
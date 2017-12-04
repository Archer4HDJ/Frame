using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Game
{
    [System.Serializable]
    public class LightMapObject
    {
        public FogInfo fogInfo = new FogInfo();
        public List<string> lightmapDir = new List<string>();
        public List<string> lightmapColor = new List<string>();
        public LightmapsMode lightmapsMode;
        public List<ReflectionProbeInfo> reflectionProbeInfos = new List<ReflectionProbeInfo>();

        public List<MapObjectRendererLightMapInfo> mapObjectRendererLightMapInfo = new List<MapObjectRendererLightMapInfo>();
    }

    //场景中的Fog信息  
    [System.Serializable]
    public struct FogInfo
    {
        public bool fog;
        public FogMode fogMode;
        public Color fogColor;
        public float fogStartDistance;
        public float fogEndDistance;
        public float fogDensity;
    }
    //每个MapObject的光照贴图信息
    [System.Serializable]
    public class MapObjectRendererLightMapInfo
    {
        public List<RendererLightMapInfo> renderLightMapInfos = new List<RendererLightMapInfo>();
    }
    //MeshRenderer 上的LightMap信息  
    [System.Serializable]
    public class RendererLightMapInfo
    {
        public int lightmapIndex;
        public Vector4 lightmapScaleOffset;
    }

    [System.Serializable]
    public struct ReflectionProbeInfo
    {
        public string gameObjectName;
        public string reflectionProbeFileName;
    }
}

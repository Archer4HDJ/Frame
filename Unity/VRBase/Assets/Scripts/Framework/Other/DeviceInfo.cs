using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeviceInfo  {

    public static string GetSystemInfo()
    {
        string systemInfo =
            "设备模型：" + SystemInfo.deviceModel +
            "\n设备名称：" + SystemInfo.deviceName +
            "\n设备类型：" + SystemInfo.deviceType +
            "\n设备唯一标识符：" + SystemInfo.deviceUniqueIdentifier +
            "\n显卡标识符：" + SystemInfo.graphicsDeviceID +
            "\n显卡设备名称：" + SystemInfo.graphicsDeviceName +
            "\n显卡厂商：" + SystemInfo.graphicsDeviceVendor +
            "\n显卡厂商ID:" + SystemInfo.graphicsDeviceVendorID +
            "\n显卡支持版本:" + SystemInfo.graphicsDeviceVersion +
            "\n显存（M）：" + SystemInfo.graphicsMemorySize +
            "\n显卡支持Shader层级：" + SystemInfo.graphicsShaderLevel +
            "\n支持最大图片尺寸：" + SystemInfo.maxTextureSize +
            "\nnpotSupport：" + SystemInfo.npotSupport +
            "\n操作系统：" + SystemInfo.operatingSystem +
            "\nCPU处理核数：" + SystemInfo.processorCount +
            "\nCPU类型：" + SystemInfo.processorType +
            "\n内存大小：" + SystemInfo.systemMemorySize +
            "\nsupportedRenderTargetCount：" + SystemInfo.supportedRenderTargetCount +
            "\nsupports3DTextures：" + SystemInfo.supports3DTextures +
            "\nsupportsAccelerometer：" + SystemInfo.supportsAccelerometer +
            "\nsupportsComputeShaders：" + SystemInfo.supportsComputeShaders +
            "\nsupportsGyroscope：" + SystemInfo.supportsGyroscope +
            "\nsupportsImageEffects：" + SystemInfo.supportsImageEffects +
            "\nsupportsInstancing：" + SystemInfo.supportsInstancing +
            "\nsupportsLocationService：" + SystemInfo.supportsLocationService +
            "\nsupportsRenderToCubemap：" + SystemInfo.supportsRenderToCubemap +
            "\n是否支持内置阴影：" + SystemInfo.supportsShadows +
            "\nsupportsSparseTextures：" + SystemInfo.supportsSparseTextures +
            "\nsupportsVibration：" + SystemInfo.supportsVibration +
            "\n系统语言：" + Application.systemLanguage +
            "\n沙盒地址：" + Application.persistentDataPath;

        return systemInfo;
    }
}

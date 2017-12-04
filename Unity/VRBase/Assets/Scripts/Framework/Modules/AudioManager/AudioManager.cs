using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using  HDJ.Framework.Modules;
using HDJ.Framework.Utils;

namespace HDJ.Framework.Modules
{

    public class AudioManager : MonoBehaviour
    {
        static GameObject obj;
        static AudioManager audioManager;
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            obj = new GameObject("[AudioManager]");
            audioManager = obj.AddComponent<AudioManager>();
            GameObject.DontDestroyOnLoad(obj);
            Audio2DPlayer.audioObject = obj;
            Audio2DPlayer.mono = audioManager;
            Audio3DPlayer.mono = audioManager;
            Volume = GameRuntimeStoreManager.GetValue("AudioVolume", 1f);
        }
        private static float volume = 1f;
        public static float Volume
        {
            get { return volume; }
            set
            {
                volume = Mathf.Clamp01(value);
                Audio2DPlayer.SetVolume(volume);
                Audio3DPlayer.SetVolume(volume);
                GameRuntimeStoreManager.SetValue("AudioVolume", volume);

            }
        }
        public static void SaveSetting()
        {
            GameRuntimeStoreManager.Save();
        }

        public static void PlayMusic2D(string name, int channel, float volumeScale = 1, bool isLoop = true, float fadeTime = 0.5f)
        {
            Audio2DPlayer.PlayMusic(channel, name, isLoop, volumeScale, fadeTime);
        }
        public static void PauseMusic2D(int channel, bool isPause)
        {
            Audio2DPlayer.PauseMusic(channel, isPause);
        }
        public static void PauseMusicAll2D(bool isPause)
        {
            Audio2DPlayer.PauseMusicAll(isPause);
        }

        public static void StopMusic2D(int channel)
        {

            Audio2DPlayer.StopMusic(channel);
        }

        public static void StopMusicAll2D()
        {
            Audio2DPlayer.StopMusicAll();
        }

        public static void PlaySFX2D(string name, float volumeScale = 1f)
        {
            Audio2DPlayer.PlaySFX(name, volumeScale);
        }
        public static void PauseSFXAll2D(bool isPause)
        {
            Audio2DPlayer.PauseSFXAll(isPause);

        }

        public static void PlayMusic3D(GameObject owner, string audioName, int channel = 0, float delay = 0f, float volumeScale = 1, bool isLoop = true, float fadeTime = 0.5f)
        {
            Audio3DPlayer.PlayMusic(owner, audioName, channel, isLoop, volumeScale, delay, fadeTime);
        }
        public static void PauseMusic3D(GameObject owner, int channel, bool isPause)
        {
            Audio3DPlayer.PauseMusic(owner, channel, isPause);
        }
        public static void PauseMusicAll3D(bool isPause)
        {
            Audio3DPlayer.PauseMusicAll(isPause);
        }

        public static void StopMusic3D(GameObject owner, int channel)
        {
            Audio3DPlayer.StopMusic(owner, channel);

        }
        public static void StopMusicOneAll3D(GameObject owner)
        {
            Audio3DPlayer.StopMusicOneAll(owner);
        }
        public static void StopMusicAll3D()
        {
            Audio3DPlayer.StopMusicAll();
        }
        public static void ReleaseMusic3D(GameObject owner)
        {
            Audio3DPlayer.ReleaseMusic(owner);
        }
        public static void ReleaseMusicAll3D()
        {
            Audio3DPlayer.ReleaseMusicAll();
        }

        public static void PlaySFX3D(GameObject owner, string name, float delay = 0f, float volumeScale = 1f)
        {
            Audio3DPlayer.PlaySFX(owner, name, volumeScale, delay);
        }
        public static void PlaySFX3D(Vector3 position, string name, float delay = 0f, float volumeScale = 1)
        {
            Audio3DPlayer.PlaySFX(position, name, volumeScale, delay);
        }

        public static void PauseSFXAll3D(bool isPause)
        {
            Audio3DPlayer.PauseSFXAll(isPause);
        }
        public static void ReleaseSFX3D(GameObject owner)
        {
            Audio3DPlayer.ReleaseSFX(owner);
        }
        public static void ReleaseSFXAll3D()
        {
            Audio3DPlayer.ReleaseSFXAll();
        }


        public static AudioClip GetAudioClip(string name)
        {
            AssetData[] red = ResourcesManager.LoadAssetsByName(name);
            if (red.Length > 0 && red[0].asset != null)
            {
                return red[0].asset as AudioClip;
            }
            Debug.LogError("Can not find AudioClip:" + name);
            return null;
        }

        public static AudioAsset CreateAudioAsset(GameObject gameObject, bool is3D)
        {
            AudioAsset au = new AudioAsset();
            au.audioSource = gameObject.AddComponent<AudioSource>();
            au.audioSource.spatialBlend = is3D ? 1 : 0;
            return au;
        }

        public static IEnumerator EaseToChangeVolume(AudioAsset au, string name, bool isLoop, float volumeScale, float delay, float fadeTime)
        {
            AudioClip ac = AudioManager.GetAudioClip(name);
            float target = au.Volume;
            if (au.audioSource && au.IsPlay)
            {
                while (target > 0f)
                {
                    float speed = Volume * volumeScale / fadeTime;
                    target = target - speed * Time.fixedDeltaTime;
                    au.Volume = target;
                    yield return new WaitForFixedUpdate();
                }
                au.Stop();
            }
            au.assetName = name;
            au.audioSource.clip = ac;
            au.audioSource.loop = isLoop;
            au.Play(delay);
            target = 0;
            yield return new WaitForSeconds(delay);

            while (target < Volume * volumeScale)
            {
                float speed = Volume * volumeScale / fadeTime * 1.2f;
                target = target + speed * Time.fixedDeltaTime;
                au.Volume = target;
                yield return new WaitForFixedUpdate();
            }
            au.VolumeScale = volumeScale;
        }

        float runTimeCount = 0;
        void Update()
        {
            TimeUtils.UpdateRunTimerDelayFunction(ref runTimeCount, 3, false, Audio3DPlayer.ClearDestroyObjectData);
        }
    }
}

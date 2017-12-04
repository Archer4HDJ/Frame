using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Modules {

    public class Audio2DPlayer
    {
        public static GameObject audioObject;
        public static MonoBehaviour mono;
        private static Dictionary<int, AudioAsset> bgMusicDic = new Dictionary<int, AudioAsset>();
        private static List<AudioAsset> sfxList = new List<AudioAsset>();
        public static int maxSFXAudioAssetNum = 10;

        public static void SetVolume(float volume)
        {
            List<AudioAsset> list = new List<AudioAsset>(bgMusicDic.Values);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Volume = volume * list[i].VolumeScale;
            }
            for (int i = 0; i < sfxList.Count; i++)
            {
                sfxList[i].Volume = volume * sfxList[i].VolumeScale;
            }
        }

        public static void PlayMusic(int channel, string audioName, bool isLoop = true, float volumeScale = 1, float delay = 0f, float fadeTime = 0.5f)
        {
            AudioAsset au;

            if (bgMusicDic.ContainsKey(channel))
            {
                au = bgMusicDic[channel];
            }
            else
            {
                au = AudioManager.CreateAudioAsset(audioObject, false);
                bgMusicDic.Add(channel, au);
            }
            if (au.assetName == audioName)
            {
                if (au.PlayState != AudioPlayState.Playing)
                    au.Play(delay);
            }
            else
            {
                au.assetName = audioName;
                au.SetPlaying();
                mono.StartCoroutine(AudioManager.EaseToChangeVolume(au, audioName, isLoop, volumeScale, delay, fadeTime));
            }
        }
        public static void PauseMusic(int channel, bool isPause)
        {
            if (bgMusicDic.ContainsKey(channel))
            {
                if (isPause)
                {
                    if (bgMusicDic[channel].PlayState == AudioPlayState.Playing)
                        bgMusicDic[channel].Pause();
                }
                else
                {
                    if (bgMusicDic[channel].PlayState == AudioPlayState.Pause)
                        bgMusicDic[channel].Play();
                }

            }
        }
        public static void PauseMusicAll(bool isPause)
        {
            foreach (int i in bgMusicDic.Keys)
            {
                PauseMusic(i, isPause);
            }
        }

        public static void StopMusic(int channel)
        {
            if (bgMusicDic.ContainsKey(channel))
            {
                bgMusicDic[channel].Stop();
            }

        }

        public static void StopMusicAll()
        {
            foreach (int i in bgMusicDic.Keys)
            {
                StopMusic(i);
            }
        }

        public static void PlaySFX(string name, float volumeScale = 1f, float delay = 0f)
        {
            AudioClip ac = AudioManager.GetAudioClip(name);
            AudioAsset aa = GetEmptyAudioAssetFromSFXList();
            aa.audioSource.clip = ac;
            aa.Play(delay);
            aa.VolumeScale = volumeScale;
            ClearMoreAudioAsset();
        }
        public static void PauseSFXAll(bool isPause)
        {
            for (int i = 0; i < sfxList.Count; i++)
            {
                if (isPause)
                {
                    if (sfxList[i].PlayState == AudioPlayState.Playing)
                        sfxList[i].Pause();
                }
                else
                {
                    if (sfxList[i].PlayState == AudioPlayState.Pause)
                        sfxList[i].Play();
                }
            }
        }

        private static AudioAsset GetEmptyAudioAssetFromSFXList()
        {
            AudioAsset au = null;
            if (sfxList.Count > 0)
            {
                for (int i = 0; i < sfxList.Count; i++)
                {
                    if (sfxList[i].PlayState == AudioPlayState.Stop)
                        au = sfxList[i];
                }
            }
            if (au == null)
            {
                au = AudioManager.CreateAudioAsset(audioObject, false);
                sfxList.Add(au);
            }
            return au;
        }

        private static void ClearMoreAudioAsset()
        {
            if (sfxList.Count > maxSFXAudioAssetNum)
            {
                List<AudioAsset> temp = new List<AudioAsset>();

                for (int i = 0; i < sfxList.Count; i++)
                {
                    if (sfxList[i].PlayState == AudioPlayState.Stop)
                    {
                        temp.Add(sfxList[i]);
                    }
                }

                for (int i = 0; i < temp.Count; i++)
                {
                    if (sfxList.Count <= maxSFXAudioAssetNum)
                        break;
                    Object.Destroy(temp[i].audioSource);
                    sfxList.Remove(temp[i]);
                }
            }
        }


    }  
}

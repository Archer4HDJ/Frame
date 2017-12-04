using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HDJ.Framework.Modules
{

    public class Audio3DPlayer
    {
        public static MonoBehaviour mono;
        private static Dictionary<GameObject, Dictionary<int, AudioAsset>> bgMusicDic = new Dictionary<GameObject, Dictionary<int, AudioAsset>>();
        private static Dictionary<GameObject, List<AudioAsset>> sfxDic = new Dictionary<GameObject, List<AudioAsset>>();
        public static int maxSFXAudioAssetNum = 10;

        public static void SetVolume(float volume)
        {
            List<Dictionary<int, AudioAsset>> dic = new List<Dictionary<int, AudioAsset>>(bgMusicDic.Values);
            for (int i = 0; i < dic.Count; i++)
            {
                List<AudioAsset> list = new List<AudioAsset>(dic[i].Values);
                for (int j = 0; j < list.Count; j++)
                {
                    list[j].Volume = volume * list[j].VolumeScale;
                }

            }
            List<List<AudioAsset>> tempList = new List<List<AudioAsset>>(sfxDic.Values);
            for (int i = 0; i < tempList.Count; i++)
            {
                List<AudioAsset> sfxList = tempList[i];
                for (int j = 0; j < sfxList.Count; j++)
                {
                    sfxList[j].Volume = volume * sfxList[j].VolumeScale;
                }

            }
        }

        public static void PlayMusic(GameObject owner, string audioName, int channel = 0, bool isLoop = true, float volumeScale = 1, float delay = 0f, float fadeTime = 0.5f)
        {
            if (owner == null)
            {
                Debug.LogError("can not play 3d player, owner is null");
                return;
            }
            AudioAsset au;
            Dictionary<int, AudioAsset> tempDic;
            if (bgMusicDic.ContainsKey(owner))
            {
                tempDic = bgMusicDic[owner];
            }
            else
            {
                tempDic = new Dictionary<int, AudioAsset>();
                bgMusicDic.Add(owner, tempDic);
            }
            if (tempDic.ContainsKey(channel))
            {
                au = tempDic[channel];
            }
            else
            {
                au = AudioManager.CreateAudioAsset(owner, true);
                tempDic.Add(channel, au);
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
        public static void PauseMusic(GameObject owner, int channel, bool isPause)
        {
            if (owner == null)
            {
                Debug.LogError("can not Pause , owner is null");
                return;
            }
            if (bgMusicDic.ContainsKey(owner))
            {
                Dictionary<int, AudioAsset> tempDic = bgMusicDic[owner];
                if (tempDic.ContainsKey(channel))
                {
                    if (isPause)
                    {
                        if (tempDic[channel].PlayState == AudioPlayState.Playing)
                            tempDic[channel].Pause();
                    }
                    else
                    {
                        if (tempDic[channel].PlayState == AudioPlayState.Pause)
                            tempDic[channel].Play();
                    }
                }

            }
        }
        public static void PauseMusicAll(bool isPause)
        {
            foreach (GameObject i in bgMusicDic.Keys)
            {
                foreach (int t in bgMusicDic[i].Keys)
                    PauseMusic(i, t, isPause);
            }
        }

        public static void StopMusic(GameObject owner, int channel)
        {
            if (bgMusicDic.ContainsKey(owner))
            {
                Dictionary<int, AudioAsset> tempDic = bgMusicDic[owner];
                if (tempDic.ContainsKey(channel))
                {
                    tempDic[channel].Stop();
                }
            }

        }
        public static void StopMusicOneAll(GameObject owner)
        {
            if (bgMusicDic.ContainsKey(owner))
            {
                List<int> list = new List<int>(bgMusicDic[owner].Keys);
                for (int i = 0; i < list.Count; i++)
                {
                    StopMusic(owner, list[i]);
                }
            }

        }
        public static void StopMusicAll()
        {
            List<GameObject> list = new List<GameObject>(bgMusicDic.Keys);
            for (int i = 0; i < list.Count; i++)
            {
                StopMusicOneAll(list[i]);
            }
        }
        public static void ReleaseMusic(GameObject owner)
        {
            if (bgMusicDic.ContainsKey(owner))
            {
                StopMusicOneAll(owner);
                List<AudioAsset> list = new List<AudioAsset>(bgMusicDic[owner].Values);
                for (int i = 0; i < list.Count; i++)
                {
                    Object.Destroy(list[i].audioSource);
                }
                list.Clear();
            }
            bgMusicDic.Remove(owner);
        }
        public static void ReleaseMusicAll()
        {
            List<GameObject> list = new List<GameObject>(sfxDic.Keys);
            for (int i = 0; i < list.Count; i++)
            {
                ReleaseMusic(list[i]);
            }
            bgMusicDic.Clear();
        }

        public static void PlaySFX(GameObject owner, string name, float volumeScale = 1f, float delay = 0f)
        {
            AudioClip ac = AudioManager.GetAudioClip(name);
            AudioAsset aa = GetEmptyAudioAssetFromSFXList(owner);
            aa.audioSource.clip = ac;
            aa.assetName = name;
            aa.Play(delay);
            aa.VolumeScale = volumeScale;
            ClearMoreAudioAsset(owner);
        }
        public static void PlaySFX(Vector3 position, string name, float volumeScale = 1f, float delay = 0f)
        {
            AudioClip ac = AudioManager.GetAudioClip(name);
            if (ac)
                mono.StartCoroutine(PlaySFXIEnumerator(position, ac, AudioManager.Volume * volumeScale, delay));
        }
        private static IEnumerator PlaySFXIEnumerator(Vector3 position, AudioClip ac, float volume, float delay)
        {
            yield return new WaitForSeconds(delay);
            AudioSource.PlayClipAtPoint(ac, position, volume);
        }
        public static void PauseSFXAll(bool isPause)
        {
            List<GameObject> list = new List<GameObject>(sfxDic.Keys);
            for (int j = 0; j < list.Count; j++)
            {
                List<AudioAsset> sfxList = sfxDic[list[j]];
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
        }
        public static void ReleaseSFX(GameObject owner)
        {
            if (owner && sfxDic.ContainsKey(owner))
            {
                List<AudioAsset> sfxList = sfxDic[owner];
                for (int i = 0; i < sfxList.Count; i++)
                {
                    Object.Destroy(sfxList[i].audioSource);
                }
                sfxList.Clear();
                sfxDic.Remove(owner);
            }
        }
        public static void ReleaseSFXAll()
        {
            List<GameObject> list = new List<GameObject>(sfxDic.Keys);
            for (int i = 0; i < list.Count; i++)
            {
                ReleaseSFX(list[i]);
            }
            sfxDic.Clear();
        }
        private static AudioAsset GetEmptyAudioAssetFromSFXList(GameObject owner)
        {
            AudioAsset au = null;
            List<AudioAsset> sfxList = null;
            if (sfxDic.ContainsKey(owner))
            {
                sfxList = sfxDic[owner];
                if (sfxList.Count > 0)
                {
                    for (int i = 0; i < sfxList.Count; i++)
                    {
                        if (sfxList[i].PlayState == AudioPlayState.Stop)
                            au = sfxList[i];
                    }

                }

            }
            else
            {
                sfxList = new List<AudioAsset>();
                sfxDic.Add(owner, sfxList);

            }
            if (au == null)
            {
                au = AudioManager.CreateAudioAsset(owner, true);
                sfxList.Add(au);
            }

            return au;
        }

        private static void ClearMoreAudioAsset(GameObject owner)
        {
            if (sfxDic.ContainsKey(owner))
            {
                List<AudioAsset> sfxList = sfxDic[owner];
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

        public static void ClearDestroyObjectData()
        {
            List<GameObject> list1 = new List<GameObject>(bgMusicDic.Keys);
            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] == null)
                    bgMusicDic.Remove(list1[i]);
            }

            List<GameObject> list = new List<GameObject>(sfxDic.Keys);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    sfxDic.Remove(list[i]);
            }
        }

    }
}

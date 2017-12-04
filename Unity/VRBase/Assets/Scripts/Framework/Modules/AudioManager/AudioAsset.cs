using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HDJ.Framework.Modules
{

    public enum AudioPlayState
    {
        Playing,
        Pause,
        Stop,
    }
    public class AudioAsset
    {
        public AudioSource audioSource;

        public string assetName = "";

        public float Volume
        {
            get { return audioSource.volume; }
            set { audioSource.volume = value; }
        }
        private float volumeScale = 1f;
        public float VolumeScale
        {
            get { return volumeScale; }
            set
            {
                volumeScale = Mathf.Clamp01(value);
                Volume = AudioManager.Volume * volumeScale;
            }
        }
        public bool IsPlay
        {
            get { return audioSource.isPlaying; }
        }

        private AudioPlayState playState = AudioPlayState.Stop;
        public AudioPlayState PlayState
        {
            get
            {
                if (audioSource==null ||( !audioSource.isPlaying && playState != AudioPlayState.Pause))
                    playState = AudioPlayState.Stop;

                return playState;
            }

        }

        public void SetPlaying()
        {
            playState = AudioPlayState.Playing;
        }

        public void Play(float delay = 0f)
        {
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayDelayed(delay);
                playState = AudioPlayState.Playing;

            }
        }
        public void Pause()
        {
            if (audioSource != null && audioSource.clip != null && audioSource.isPlaying)
            {
                audioSource.Pause();
                playState = AudioPlayState.Pause;
            }
        }
        public void Stop()
        {
            if (audioSource)
                audioSource.Stop();
            playState = AudioPlayState.Stop;
        }

    }
    [System.Serializable]
    public class AudioClipData
    {
        public string clipName = "";
        public AudioClip clip;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//音频类型枚举
public enum SoundType
{
    //背景音乐
    BGMain,
}


public class SoundController : SingletonManager<SoundController>
{
    //是否开启声音
    public bool IsSoundOn
    {
        get
        {
            return (PlayerPrefs.GetInt("IsSoundOn", 1) == 1);
        }
        set
        {
            if (IsSoundOn != value)
            {
                PlayerPrefs.SetInt("IsSoundOn", value ? 1 : 0);
                if (value)
                {
                    PlayMusic(SoundType.BGMain);
                }
                else
                {
                    PauseMusic();
                }
            }
        }
    }
    //背景音乐
    private AudioSource musicAudioSource;
    //音效
    private ComponentPool<AudioSource> audioSourcePool = new ComponentPool<AudioSource>();

    //音频路径
    private Dictionary<SoundType, string> audioClipPaths = new Dictionary<SoundType, string>();
    //已经加载的音频
    private Dictionary<SoundType, AudioClip> audioClipDict = new Dictionary<SoundType, AudioClip>();

    protected override void Init()
    {
        base.Init();
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        audioSourcePool.Init(CreateAudioSource);
        LoadSoundPath();
    }

    private void Start()
    {
        PlayMusic(SoundType.BGMain);
    }

    private AudioSource CreateAudioSource()
    {
        return gameObject.AddComponent<AudioSource>();
    }

    private void LoadSoundPath()
    {
        foreach (var item in SystemData.SoundInfoList)
        {
            SoundType soundType = (SoundType)Enum.Parse(typeof(SoundType), item.enumName);
            string path = item.path;
            if (path.EndsWith("/"))
            {
                path += item.name;
            }
            else
            {
                path = path + "/" + item.name;
            }
            audioClipPaths.Add(soundType, path);
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="soundType"></param>
    public void PlayMusic(SoundType soundType)
    {
        if (IsSoundOn)
        {
            musicAudioSource.loop = true;
            musicAudioSource.mute = false;
            musicAudioSource.clip = GetAudioClip(soundType);
            musicAudioSource.Play();
        }
    }

    /// <summary>
    /// 延迟播放音效
    /// </summary>
    /// <param name="soundType"></param>
    /// <param name="delaySeconds"></param>
    /// <param name="pitch"></param>
    public void PlaySoundDelay(SoundType soundType, float delaySeconds, float pitch = 1)
    {
        StartCoroutine(PlaySoundDelayCo(soundType, delaySeconds, pitch));
    }

    private IEnumerator PlaySoundDelayCo(SoundType soundType, float delaySeconds, float pitch = 1)
    {
        yield return new WaitForSeconds(delaySeconds);
        PlaySound(soundType, false, pitch);
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="soundType"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>
    public AudioSource PlaySound(SoundType soundType, bool loop = false, float pitch = 1)
    {
        if (IsSoundOn)
        {
            AudioSource audioSource = audioSourcePool.Get();
            audioSource.loop = loop;
            audioSource.clip = GetAudioClip(soundType);
            audioSource.Play();
            if (!loop)
            {
                StartCoroutine(PlaySoundCallback(audioSource.clip.length, audioSource));
            }
            audioSource.pitch = pitch;
            return audioSource;
        }
        return null;
    }

    public void StopAudioSource(AudioSource audioSource)
    {
        if (audioSource)
        {
            audioSource.loop = false;
            audioSource.Pause();
            audioSourcePool.Release(audioSource);
        }
    }

    /// <summary>
    /// 获取音频片段
    /// </summary>
    /// <param name="soundType"></param>
    /// <returns></returns>
    AudioClip GetAudioClip(SoundType soundType)
    {
        if (audioClipDict.ContainsKey(soundType))
        {
            return audioClipDict[soundType];
        }

        if (audioClipPaths.TryGetValue(soundType, out string path))
        {
            AudioClip audioClip = ResourceManager.LoadResource<AudioClip>(path, true);
            return audioClip;
        }
        else
        {
            Debug.LogError("audioClipPaths doesn't contains soundType :" + soundType.ToString());
            return null;
        }
    }

    private IEnumerator PlaySoundCallback(float delayTime, AudioSource audioSource)
    {
        yield return new WaitForSeconds(delayTime);
        audioSourcePool.Release(audioSource);
    }

    //暂停背景音乐
    private void PauseMusic()
    {
        musicAudioSource.Pause();
    }

    //开启背景音乐
    private void UnPauseMusic()
    {
        musicAudioSource.UnPause();
    }

    public void MuteMusic()
    {
        musicAudioSource.mute = true;
    }

    public void UnMuteMusic()
    {
        musicAudioSource.mute = false;
    }



}



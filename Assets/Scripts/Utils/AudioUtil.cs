

using System;
using System.Collections.Generic;
using UnityEngine;
public class AudioInfo
{
    public string tag;
    public AudioSource AudioSource;
    public AudioClip AudioClip;
    public UnityTimer UnityTimer;

    public AudioInfo Play()
    {
        AudioUtil.Instance.Play(this);
        return this;
    }
    public AudioInfo Pause()
    {
        AudioUtil.Instance.Pause(this);
        return this;
    }
    public AudioInfo Stop()
    {
        AudioUtil.Instance.Stop(this);
        return this;
    }
};
public class AudioUtil : Singleton<AudioUtil>
{
    public const string DEFAULT_TAG = "Default";

    private List<AudioInfo> _audioList = new List<AudioInfo>();

    public AudioUtil()
    {
        var prefab = new GameObject("AudioClient");
        var audioSource = prefab.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        PoolUtil.Instance.CreateObject("AudioClient", prefab, "AudioRoot");
        GameObject.Destroy(prefab);
    }
    public AudioInfo Play(AssetInfo info, Action<AudioInfo> onTrigger = null, bool loop = false, string tag = DEFAULT_TAG, float volume = 1f)
    {
        var clip = AssetUtil.Instance.Load<AudioClip>(AssetCode.ClickClip);
        return Play(clip, onTrigger, loop, tag, volume);
    }
    public AudioInfo Play(AudioClip clip, Action<AudioInfo> onTrigger = null, bool loop = false, string tag = DEFAULT_TAG, float volume = 1f)
    {
        if (clip == null)
        {
            return null;
        }
        var go = PoolUtil.Instance.Load("AudioClient");
        var audioSource = go.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = loop;

        var audioInfo = new AudioInfo();
        audioInfo.tag = tag;
        audioInfo.AudioSource = audioSource;
        audioInfo.AudioClip = clip;
        audioInfo.UnityTimer = null;

        if (!loop)
        {
            audioInfo.UnityTimer = TimerUtil.Instance.On(clip.length, (o, e) =>
            {
                if (onTrigger != null) onTrigger(audioInfo);
                // 关闭定时器
                audioInfo.UnityTimer.Stop();
                removeAudioInfo(audioInfo);
            });
        }

        _audioList.Add(audioInfo);

        audioSource.Play();

        return audioInfo;
    }
    public void Play(AudioInfo info)
    {
        if (_audioList.IndexOf(info) == -1) return;
        var audioSource = info.AudioSource;
        var timer = info.UnityTimer;
        if (timer != null)
        {
            timer.Play();
        }
        audioSource.Play();
    }
    public void Pause(AudioInfo info)
    {
        if (_audioList.IndexOf(info) == -1) return;
        var audioSource = info.AudioSource;
        var timer = info.UnityTimer;
        if (timer != null)
        {
            timer.Pause();
        }
        audioSource.Pause();
    }
    public void Stop(AudioInfo info)
    {
        if (_audioList.IndexOf(info) == -1) return;
        var audioSource = info.AudioSource;
        var timer = info.UnityTimer;
        if (timer != null)
        {
            timer.Stop();
        }
        audioSource.Stop();

        removeAudioInfo(info);
    }

    public void PlayWithTag(string tag)
    {
        foreach (var info in _audioList)
        {
            if (info.tag == tag)
            {
                info.Play();
            }
        }
    }
    public void PauseWithTag(string tag)
    {
        foreach (var info in _audioList)
        {
            if (info.tag == tag)
            {
                info.Pause();
            }
        }
    }
    public void StopWithTag(string tag)
    {
        foreach (var info in _audioList)
        {
            if (info.tag == tag)
            {
                info.Stop();
            }
        }
    }



    private void removeAudioInfo(AudioInfo info)
    {
        _audioList.Remove(info);
    }
}
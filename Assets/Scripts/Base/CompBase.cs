
using System;
using System.Collections.Generic;
using UnityEngine;

public class CompBase : MonoBehaviour
{
    private List<BroadcastInfo> _broadcastInfoList = new List<BroadcastInfo>();
    private List<UnityTimer> _timerList = new List<UnityTimer>();

    private List<AudioInfo> _audioList = new List<AudioInfo>();
    // ------------------------- 重写记得base. -------------------------
    protected void OnEnable()
    {
        foreach (var broadcastInfo in _broadcastInfoList)
        {
            BroadcastUtil.Instance.On(broadcastInfo);
        }
        foreach (var timer in _timerList)
        {
            timer.Play();
        }
        foreach (var audioInfo in _audioList)
        {
            audioInfo.Play();
        }
    }
    protected void OnDisable()
    {
        foreach (var broadcastInfo in _broadcastInfoList)
        {
            BroadcastUtil.Instance.Off(broadcastInfo);
        }
        foreach (var timer in _timerList)
        {
            timer.Pause();
        }
        foreach (var audioInfo in _audioList)
        {
            audioInfo.Pause();
        }
    }
    protected void OnDestroy()
    {
        foreach (var broadcastInfo in _broadcastInfoList)
        {
            Broadcast_Off(broadcastInfo);
        }
        foreach (var timer in _timerList)
        {
            timer.Stop();
        }
        foreach (var audioInfo in _audioList)
        {
            audioInfo.Stop();
        }
    }
    // ------------------------- 重写记得base. -------------------------

    public BroadcastInfo Broadcast_On(BroadcastCode code, CallBack callBack, int order = 0, bool isOnce = false)
    {
        var info = new BroadcastInfo() { Code = code, CallBack = callBack, Order = order, IsOnce = isOnce };
        _broadcastInfoList.Add(info);
        return BroadcastUtil.Instance.On(info);
    }
    public BroadcastInfo Broadcast_On<T1>(BroadcastCode code, CallBack<T1> callBack, int order = 0, bool isOnce = false)
    {
        var info = new BroadcastInfo() { Code = code, CallBack = callBack, Order = order, IsOnce = isOnce };
        _broadcastInfoList.Add(info);
        return BroadcastUtil.Instance.On(info);
    }
    public BroadcastInfo Broadcast_On<T1, T2>(BroadcastCode code, CallBack<T1, T2> callBack, int order = 0, bool isOnce = false)
    {
        var info = new BroadcastInfo() { Code = code, CallBack = callBack, Order = order, IsOnce = isOnce };
        _broadcastInfoList.Add(info);
        return BroadcastUtil.Instance.On(info);
    }
    public BroadcastInfo Broadcast_On<T1, T2, T3>(BroadcastCode code, CallBack<T1, T2, T3> callBack, int order = 0, bool isOnce = false)
    {
        var info = new BroadcastInfo() { Code = code, CallBack = callBack, Order = order, IsOnce = isOnce };
        _broadcastInfoList.Add(info);
        return BroadcastUtil.Instance.On(info);
    }
    public BroadcastInfo Broadcast_On<T1, T2, T3, T4>(BroadcastCode code, CallBack<T1, T2, T3, T4> callBack, int order = 0, bool isOnce = false)
    {
        var info = new BroadcastInfo() { Code = code, CallBack = callBack, Order = order, IsOnce = isOnce };
        _broadcastInfoList.Add(info);
        return BroadcastUtil.Instance.On(info);
    }
    public bool Broadcast_Off(BroadcastInfo info)
    {
        _broadcastInfoList.Remove(info);
        return BroadcastUtil.Instance.Off(info);
    }
    public int Broadcast_Notify(BroadcastCode code)
    {
        return BroadcastUtil.Instance.Notify(code);
    }
    public int Broadcast_Notify<T1>(BroadcastCode code, T1 arg1)
    {
        return BroadcastUtil.Instance.Notify(code, arg1);
    }
    public int Broadcast_Notify<T1, T2>(BroadcastCode code, T1 arg1, T2 arg2)
    {
        return BroadcastUtil.Instance.Notify(code, arg1, arg2);
    }
    public int Broadcast_Notify<T1, T2, T3>(BroadcastCode code, T1 arg1, T2 arg2, T3 arg3)
    {
        return BroadcastUtil.Instance.Notify(code, arg1, arg2, arg3);
    }
    public int Broadcast_Notify<T1, T2, T3, T4>(BroadcastCode code, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        return BroadcastUtil.Instance.Notify(code, arg1, arg2, arg3, arg4);
    }

    public UnityTimer Timer_On(float time, int loopCount, Action<object, TimerEventArgs> onTrigger)
    {
        var timer = TimerUtil.Instance.On(time, loopCount, onTrigger);
        _timerList.Add(timer);
        return timer;
    }
    public UnityTimer Timer_On(float time, bool isLoop, Action<object, TimerEventArgs> onTrigger)
    {
        var timer = TimerUtil.Instance.On(time, isLoop, onTrigger);
        _timerList.Add(timer);
        return timer;
    }
    public UnityTimer Timer_On(float time, Action<object, TimerEventArgs> onTrigger)
    {
        var timer = TimerUtil.Instance.On(time, onTrigger);
        _timerList.Add(timer);
        return timer;
    }

    public AudioInfo Audio_Play(AudioClip clip, Action<AudioInfo> onTrigger = null, bool loop = false, string tag = AudioUtil.DEFAULT_TAG, float volume = 1f)
    {
        var info = AudioUtil.Instance.Play(clip, (info) =>
        {
            if (onTrigger != null) onTrigger(info);
            _audioList.Remove(info);
        }, loop, tag, volume);
        _audioList.Add(info);
        return info;
    }
    public AudioInfo Audio_Play(AssetInfo assetInfo, Action<AudioInfo> onTrigger = null, bool loop = false, string tag = AudioUtil.DEFAULT_TAG, float volume = 1f)
    {
        var info = AudioUtil.Instance.Play(assetInfo, (info) =>
        {
            if (onTrigger != null) onTrigger(info);
            _audioList.Remove(info);
        }, loop, tag, volume);
        _audioList.Add(info);
        return info;
    }


    // 延时
    private float _stime = 0;
    private float _dtime = 0;
    public CompBase Delay(float time)
    {
        _stime += time;
        return this;
    }

    public CompBase Event(Action action)
    {
        _dtime += _stime;
        Timer_On(_dtime, (o, e) =>
        {
            if (action != null) action();
            _dtime -= _stime;
        });

        _stime = 0;
        return this;
    }

}
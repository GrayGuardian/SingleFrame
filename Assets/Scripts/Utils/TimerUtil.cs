using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class TimerUtil : Singleton<TimerUtil>
{
    /// <summary>
    /// 临时添加队列 防止检测正式队列时添加产生错误
    /// </summary>
    private List<UnityTimer> _timerAddTempList;
    /// <summary>
    /// 正式时间队列
    /// </summary>
    private List<UnityTimer> _timerList;
    /// <summary>
    /// 临时删除队列 防止检测正式队列时删除产生错误
    /// </summary>
    private List<UnityTimer> _timerDelTempList;
    public TimerUtil()
    {
        BroadcastUtil.Instance.On(BroadcastCode.MONO_UPDATE, () => { Update(); });
        _timerAddTempList = new List<UnityTimer>();
        _timerList = new List<UnityTimer>();
        _timerDelTempList = new List<UnityTimer>();
    }
    private void Update()
    {
        //将临时添加队列放入正式队列中
        foreach (var timer in _timerAddTempList)
        {
            if (!_timerList.Exists(x => x == timer))
                _timerList.Add(timer);
        }
        //清空临时添加队列
        _timerAddTempList = new List<UnityTimer>();
        //更新Timer队列信息
        foreach (var timer in _timerList)
        {
            //通过剩余循环次数判断是否结束 -1为无限循环 不触发
            if (timer.RemainCount == 0)
            {
                //时钟结束
                StopTimer(timer);
            }
            if (timer.IsOver || timer.IsPause)
            {
                //时钟结束或暂停，跳过判断
                continue;
            }
            timer.RunTime += Time.deltaTime;
            timer.CycleRunTime += Time.deltaTime;
            if (timer.CycleRemainTime == 0f)
            {
                //当前周期结束 执行周期函数
                timer.OverCount++;
                timer.CycleRunTime = 0f;
                if (timer.OnTrigger != null)
                {
                    TimerEventArgs e = new TimerEventArgs(timer.RunTime, timer.OverCount, timer.RemainCount);
                    timer.OnTrigger((object)timer, e);
                }
            }
        }
        foreach (var timer in _timerDelTempList)
        {
            timer.IsOver = true;
            if (_timerList.Exists(x => x == timer))
            {
                _timerList.Remove(timer);
            }
        }
        _timerDelTempList = new List<UnityTimer>();
    }

    /// <summary>
    /// 生成时钟
    /// </summary>
    public UnityTimer On(float time, int loopCount, Action<object, TimerEventArgs> onTrigger)
    {
        UnityTimer timer = new UnityTimer(time, loopCount, onTrigger);
        _timerAddTempList.Add(timer);
        return timer;
    }
    public UnityTimer On(float time, bool isLoop, Action<object, TimerEventArgs> onTrigger)
    {
        int loopCount = isLoop ? -1 : 1;
        return On(time, loopCount, onTrigger);
    }
    public UnityTimer On(float time, Action<object, TimerEventArgs> onTrigger)
    {
        int loopCount = 1;
        return On(time, loopCount, onTrigger);
    }
    public void PlayTimer(UnityTimer timer)
    {
        if (!TimerIsExists(timer)) return;
        timer.IsPause = false;
    }
    public void PauseTimer(UnityTimer timer)
    {
        if (!TimerIsExists(timer)) return;
        timer.IsPause = true;
    }

    public void StopTimer(UnityTimer timer)
    {
        if (!TimerIsExists(timer)) return;
        timer.IsOver = true;
        _timerDelTempList.Add(timer);
    }

    /// <summary>
    /// 时钟是否存在
    /// </summary>
    public bool TimerIsExists(UnityTimer timer)
    {
        return _timerList.Exists(x => x == timer) || _timerAddTempList.Exists(x => x == timer);
    }
}

public class UnityTimer
{
    #region 参数定义
    /// <summary>
    /// 周期时长
    /// </summary>
    public float Time;
    /// <summary>
    /// 运行总时长
    /// </summary>
    public float RunTime;
    /// <summary>
    /// 当前周期运行时长
    /// </summary>
    public float CycleRunTime;
    /// <summary>
    /// 当前周期剩余时长
    /// </summary>
    public float CycleRemainTime
    {
        get
        {
            float time = Time - CycleRunTime;
            return time < 0f ? 0f : time;
        }
    }
    /// <summary>
    /// 剩余时长（无限循环则返回-1）
    /// </summary>
    public float RemainTime
    {
        get
        {
            if (IsLoop) return -1;
            float time = RemainCount * Time + CycleRemainTime;
            return time < 0f ? 0f : time;
        }
    }
    /// <summary>
    /// 循环次数
    /// </summary>
    public int LoopCount;
    /// <summary>
    /// 循环完成次数
    /// </summary>
    public int OverCount;
    /// <summary>
    /// 剩余循环次数（无限循环则返回-1）
    /// </summary>
    public int RemainCount
    {
        get
        {
            if (IsLoop) return -1;
            int count = LoopCount - OverCount;
            return count < 0 ? 0 : count;
        }
    }
    /// <summary>
    /// 是否结束
    /// </summary>
    public bool IsOver;
    /// <summary>
    /// 是否暂停
    /// </summary>
    public bool IsPause;
    /// <summary>
    /// 是否无限循环
    /// </summary>
    public bool IsLoop
    {
        get { return LoopCount == -1; }
    }
    /// <summary>
    /// 周期完成事件
    /// </summary>
    public Action<object, TimerEventArgs> OnTrigger;

    #endregion

    #region 构造函数

    public UnityTimer(float time, int loopCount, Action<object, TimerEventArgs> onTrigger)
    {
        Time = time;
        LoopCount = loopCount;
        OnTrigger = onTrigger;
    }
    public UnityTimer(float time, bool isLoop, Action<object, TimerEventArgs> onTrigger)
    {
        int loopCount = isLoop ? -1 : 1;
        Time = time;
        LoopCount = loopCount;
        OnTrigger = onTrigger;
    }
    public UnityTimer(float time, Action<object, TimerEventArgs> onTrigger)
    {
        int loopCount = 1;
        Time = time;
        LoopCount = loopCount;
        OnTrigger = onTrigger;
    }

    #endregion

    #region 链式调用
    public UnityTimer SetTriggerEvent(Action<object, TimerEventArgs> onTrigger)
    {
        OnTrigger = onTrigger;
        return this;
    }

    public UnityTimer SetTime(float time)
    {
        Time = time;
        CycleRunTime = 0f;
        return this;
    }
    public UnityTimer SetLoop(bool isLoop)
    {
        bool loop = IsLoop;
        if (loop)
        {
            //本身就是无限循环的情况 
            LoopCount = isLoop ? -1 : OverCount + 1;
        }
        else
        {
            //本身不是无限循环
            LoopCount = isLoop ? -1 : LoopCount;
        }
        return this;
    }

    public UnityTimer SetLoop(int loopCount)
    {
        LoopCount = loopCount;
        return this;
    }

    public UnityTimer Play()
    {
        IsPause = false;
        return this;
    }

    public UnityTimer Pause()
    {
        IsPause = true;
        return this;
    }

    public UnityTimer Stop()
    {
        TimerUtil.Instance.StopTimer(this);
        return this;
    }
    #endregion


}

public class TimerEventArgs : EventArgs
{
    /// <summary>
    /// 运行总时长
    /// </summary>
    public float RunTime { get; private set; }
    /// <summary>
    /// 循环完成次数
    /// </summary>
    public int OverCount { get; private set; }
    /// <summary>
    /// 剩余循环次数（仅适用于有限循环数量的情况）
    /// </summary>
    public int RemainCount { get; private set; }

    public TimerEventArgs(float runTime, int overCount, int remainCount)
    {
        RunTime = runTime;
        OverCount = overCount;
        RemainCount = remainCount;
    }
}
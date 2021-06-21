using System;
using System.Collections.Generic;

public delegate void CallBack();
public delegate void CallBack<T1>(T1 arg1);
public delegate void CallBack<T1, T2>(T1 arg1, T2 arg2);
public delegate void CallBack<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
public delegate void CallBack<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public class BroadcastInfo
{
    public BroadcastCode Code;
    public int Order;
    public Delegate CallBack;
    public bool IsOnce;
}
public class BroadcastUtil : Singleton<BroadcastUtil>
{
    private Dictionary<BroadcastCode, List<BroadcastInfo>> _infoDic = new Dictionary<BroadcastCode, List<BroadcastInfo>>();
    #region 添加监听事件
    public BroadcastInfo On(BroadcastInfo info)
    {
        if (!_infoDic.ContainsKey(info.Code))
        {
            _infoDic.Add(info.Code, new List<BroadcastInfo>());
        }
        var list = _infoDic[info.Code];
        if (list.Count > 0 && list[0].CallBack.GetType() != info.CallBack.GetType())
        {
            throw new Exception("事件类型不同，无法监听");
        }
        int index = list.Count;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Order > info.Order)
            {
                index = i;
                break;
            }
        }
        list.Insert(index, info);
        return info;
    }
    public BroadcastInfo On(BroadcastCode code, CallBack callBack, int order = 0, bool isOnce = false)
    {
        return On(new BroadcastInfo() { Code = code, CallBack = callBack, Order = order, IsOnce = isOnce });
    }
    public BroadcastInfo On<T1>(BroadcastCode code, CallBack<T1> callBack, int order = 0, bool isOnce = false)
    {
        return On(new BroadcastInfo() { Code = code, CallBack = callBack, Order = order, IsOnce = isOnce });
    }
    public BroadcastInfo On<T1, T2>(BroadcastCode code, CallBack<T1, T2> callBack, int order = 0, bool isOnce = false)
    {
        return On(new BroadcastInfo() { Code = code, CallBack = callBack, Order = order, IsOnce = isOnce });
    }
    public BroadcastInfo On<T1, T2, T3>(BroadcastCode code, CallBack<T1, T2, T3> callBack, int order = 0, bool isOnce = false)
    {
        return On(new BroadcastInfo() { Code = code, CallBack = callBack, Order = order, IsOnce = isOnce });
    }
    public BroadcastInfo On<T1, T2, T3, T4>(BroadcastCode code, CallBack<T1, T2, T3, T4> callBack, int order = 0, bool isOnce = false)
    {
        return On(new BroadcastInfo() { Code = code, CallBack = callBack, Order = order, IsOnce = isOnce });
    }
    #endregion
    #region 移出监听事件
    public bool Off(BroadcastInfo info)
    {
        if (!_infoDic.ContainsKey(info.Code))
        {
            return false;
        }
        var flag = _infoDic[info.Code].Remove(info);
        return flag;
    }
    #endregion
    #region 广播事件
    public int Notify(BroadcastCode code)
    {
        int count = 0;
        if (!_infoDic.ContainsKey(code))
        {
            return count;
        }
        var list = _infoDic[code];

        foreach (var info in list)
        {
            var callBack = (CallBack)info.CallBack;
            if (callBack != null)
            {
                callBack();
                count++;
                if (info.IsOnce)
                {
                    Off(info);
                }
            }
        }
        return count;
    }
    public int Notify<T1>(BroadcastCode code, T1 arg)
    {
        int count = 0;
        if (!_infoDic.ContainsKey(code))
        {
            return count;
        }
        var list = _infoDic[code];
        foreach (var info in list)
        {
            var callBack = (CallBack<T1>)info.CallBack;
            if (callBack != null)
            {
                callBack(arg);
                count++;
                if (info.IsOnce)
                {
                    Off(info);
                }
            }
        }
        return count;
    }
    public int Notify<T1, T2>(BroadcastCode code, T1 arg1, T2 arg2)
    {
        int count = 0;
        if (!_infoDic.ContainsKey(code))
        {
            return count;
        }
        var list = _infoDic[code];
        foreach (var info in list)
        {
            var callBack = (CallBack<T1, T2>)info.CallBack;
            if (callBack != null)
            {
                callBack(arg1, arg2);
                count++;
                if (info.IsOnce)
                {
                    Off(info);
                }
            }
        }
        return count;
    }
    public int Notify<T1, T2, T3>(BroadcastCode code, T1 arg1, T2 arg2, T3 arg3)
    {
        int count = 0;
        if (!_infoDic.ContainsKey(code))
        {
            return count;
        }
        var list = _infoDic[code];
        foreach (var info in list)
        {
            var callBack = (CallBack<T1, T2, T3>)info.CallBack;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3);
                count++;
                if (info.IsOnce)
                {
                    Off(info);
                }
            }
        }
        return count;
    }
    public int Notify<T1, T2, T3, T4>(BroadcastCode code, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        int count = 0;
        if (!_infoDic.ContainsKey(code))
        {
            return count;
        }
        var list = _infoDic[code];
        foreach (var info in list)
        {
            var callBack = (CallBack<T1, T2, T3, T4>)info.CallBack;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4);
                count++;
                if (info.IsOnce)
                {
                    Off(info);
                }
            }
        }
        return count;
    }

    #endregion
    #region 清空监听事件
    public void ClearListener(BroadcastCode code)
    {
        if (_infoDic.ContainsKey(code))
        {
            _infoDic.Remove(code);
        }
    }
    #endregion
}
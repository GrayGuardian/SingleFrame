using System;
using System.Collections.Generic;

public class ActionList
{
    private List<Action> _actionArr = new List<Action>();
    private int _order = 0;

    public void Into(int order)
    {
        _order = order;
        Next();
    }
    public void Next()
    {
        try
        {
            Action e = _actionArr[_order];
            _order++;
            e();
        }
        catch { }
    }
    public ActionList On(Action<ActionList> cb)
    {
        _actionArr.Add(() =>
        {
            cb(this);
        });
        return this;
    }


}
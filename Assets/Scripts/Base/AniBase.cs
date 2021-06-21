
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AniBase : CompBase
{
    protected Animator _curAnimator;
    // ------------------------- 重写记得base. -------------------------
    protected void Awake()
    {
        _curAnimator = GetComponent<Animator>();
        if (_curAnimator == null)
        {
            Debug.LogError("没找到动画状态机,请检查" + this.gameObject.name + "是否添加动画状态机");
        }
    }
    protected void OnEnable()
    {
        base.OnEnable();
    }
    protected void OnDisable()
    {
        base.OnDisable();
    }
    protected void OnDestroy()
    {
        base.OnDestroy();
    }
    // ------------------------- 重写记得base. -------------------------
    protected void AddAnimationEvent(string clipName, string eventFunctionName, Func<AnimationClip, float> getTimeEvent)
    {
        foreach (var clip in _curAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                int cnt = clip.events.Count<AnimationEvent>(e => e.functionName == eventFunctionName);
                if (cnt == 0)
                {
                    var e = new AnimationEvent();
                    e.functionName = eventFunctionName;
                    e.time = getTimeEvent(clip);
                    clip.AddEvent(e);
                }
            }
        }
    }
    protected void RemoveAnimationEvent(string clipName, string eventFunctionName)
    {
        foreach (var clip in _curAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                var events = clip.events.Where<AnimationEvent>(e => e.functionName == eventFunctionName).ToArray();
                if (events.Length > 0)
                {
                    var list = clip.events.ToList();
                    print(list.Count);

                    foreach (var item in list)
                    {
                        if (events[0].functionName == item.functionName)
                        {
                            list.Remove(item);
                            break;
                        }
                    }
                    clip.events = list.ToArray();
                }
            }
        }
    }
}
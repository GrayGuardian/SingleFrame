
using System;
using System.Collections.Generic;
using UnityEngine;
public class AniTestComp : AniBase
{
    private void Awake()
    {
        base.Awake();

        AddAnimationEvent("idle", "aaa", (clip) => { return clip.length; });
        // RemoveAnimationEvent("ani1", "aaa");
    }
    public void aaa()
    {

        print(":::::::::::::::::::::::::::::::" + gameObject.name);
    }

}
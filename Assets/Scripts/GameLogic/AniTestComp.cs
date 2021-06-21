
using System;
using System.Collections.Generic;
using UnityEngine;
public class AniTestComp : AniBase
{
    private void Awake()
    {
        base.Awake();

        AddAnimationEvent("ani1", "aaa", (clip) => { return clip.length - 0.5f; });
        // RemoveAnimationEvent("ani1", "aaa");
    }
    public void aaa()
    {

        print(":::::::::::::::::::::::::::::::" + gameObject.name);
    }

}
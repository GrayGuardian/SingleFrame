
using UnityEngine;

public class TestComp : CompBase
{
    private void Start()
    {
        // var info = base.Broadcast_On(BroadcastCode.MONO_UPDATE, () =>
        // {
        //     transform.Translate(Vector3.one);
        // });

        // base.Timer_On(1f, true, (timer, e) => { print("我是一秒循环的计时器"); });

        // PoolUtil.Instance.CreateObject("fdff", new GameObject("123123"), PoolBox.DefaultKey);

        // var go = PoolUtil.Instance.Load("fdff");
        // UnityEngine.Debug.Log(go);
        // PoolUtil.Instance.UnLoad(go, true);

        // var audio = AudioUtil.Instance;

        // float time = Time.time;

        // base.Delay(1f).Delay(2f).Event(() =>
        // {
        //     print("延时3秒>>>>>>" + (Time.time - time));
        // }).Delay(1f).Event(() =>
        // {
        //     print("延时4秒>>>>>>" + (Time.time - time));
        // }).Delay(2f).Event(() =>
        // {
        //     print("延时6秒>>>>>>" + (Time.time - time));
        // });

        // gameObject.AddComponent<TouchComp>()
        // .AddPressed((o, e) => { print("按下"); })
        // .AddReleased((o, e) => { print("释放"); })
        // .AddLongPressed((o, e) => { print("长按"); })
        // .AddTransformed((o, e) => { print("拖动过程"); })
        // .AddTransformStarted((o, e) => { print("拖动开始"); })
        // .AddTransformCompleted((o, e) => { print("拖动结束"); });

        // base.Audio_Play(Resources.Load<AudioClip>("1"));
    }
}
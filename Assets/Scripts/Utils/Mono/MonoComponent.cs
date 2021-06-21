using UnityEngine;
public class MonoComponent : MonoBehaviour
{
    protected void Awake()
    {
        BroadcastUtil.Instance.Notify(BroadcastCode.MONO_AWAKE);
    }
    protected void Start()
    {
        BroadcastUtil.Instance.Notify(BroadcastCode.MONO_START);
    }
    protected void OnEnable()
    {
        BroadcastUtil.Instance.Notify(BroadcastCode.MONO_ONENABLE);
    }
    protected void OnDisable()
    {
        BroadcastUtil.Instance.Notify(BroadcastCode.MONO_ONDISABLE);
    }
    protected void OnDestroy()
    {
        BroadcastUtil.Instance.Notify(BroadcastCode.MONO_ONDESTROY);
    }
    protected void Update()
    {
        BroadcastUtil.Instance.Notify(BroadcastCode.MONO_UPDATE);
    }
    protected void FixedUpdate()
    {
        BroadcastUtil.Instance.Notify(BroadcastCode.MONO_FIXEDUPDATE);
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        BroadcastUtil.Instance.Notify(BroadcastCode.MONO_ONAPPLICATIONPAUSE);
    }
    private void OnApplicationFocus(bool focusStatus)
    {
        BroadcastUtil.Instance.Notify(BroadcastCode.MONO_ONAPPLICATIONFOCUS);
    }
}
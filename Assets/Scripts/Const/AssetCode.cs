using System;
using UnityEngine;

public enum ASSET_SOURCE
{
    // Resources.Load
    RESOURCES = 0,
}
public class AssetInfo
{
    public ASSET_SOURCE source;
    public Type type;
    public string path;
}
public class AssetCode
{
    public static AssetInfo ClickClip = new AssetInfo() { source = ASSET_SOURCE.RESOURCES, type = typeof(AudioClip), path = "2" };     // 点击音效
}
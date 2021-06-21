using UnityEngine;
public class AssetUtil : Singleton<AssetUtil>
{
    public UnityEngine.Object Load(AssetInfo info)
    {
        if (info.source == ASSET_SOURCE.RESOURCES)
        {
            return Resources.Load(info.path, info.type);
        }

        return null;
    }
    public T Load<T>(AssetInfo info) where T : UnityEngine.Object
    {
        return (T)Load(info);
    }
}
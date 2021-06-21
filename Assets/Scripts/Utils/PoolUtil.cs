using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

/// <summary>
/// 对象池容器
/// </summary>
public class PoolBox
{
    public string Key;
    public Transform Node;
    public PoolBox ParentBox;
    public PoolBox(string key)
    {
        Key = key;
        Node = new GameObject(key).transform;
    }

    public PoolBox BindParentBox(PoolBox poolBox)
    {
        ParentBox = poolBox;
        Node.parent = ParentBox.Node;
        return this;
    }
    public const string DefaultKey = "PoolBox";
}
/// <summary>
/// 对象实例
/// </summary>
public class PoolObject
{
    /// <summary>
    /// 对象Key
    /// </summary>
    public string Key;
    /// <summary>
    /// 对象实例
    /// </summary>
    public GameObject GameObject;
    /// <summary>
    /// 信息
    /// </summary>
    public PoolObjectInfo Info;
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled;
}
/// <summary>
/// 对象信息
/// </summary>
public class PoolObjectInfo
{
    /// <summary>
    /// 预制体
    /// </summary>
    /// <returns></returns>
    public GameObject Prefab;
    /// <summary>
    /// 对象池容器
    /// </summary>
    public PoolBox PoolBox;
    /// <summary>
    /// 最大数量
    /// </summary>
    public int MaxSize = 10;
    /// <summary>
    /// 是否自动销毁
    /// </summary>
    public bool IsAutomationDel = true;
    /// <summary>
    /// 定时检测删除的时间间隔
    /// </summary>
    public float DelDelayTime = 5f;
    /// <summary>
    /// 单次删除数量
    /// </summary>
    public int SingleDelSum = 5;

    public static PoolObjectInfo Default
    {
        get
        {
            return new PoolObjectInfo();
        }
    }
}

public class PoolUtil : Singleton<PoolUtil>
{
    private Transform _rootBox;
    private Transform _prefabBox;
    private Dictionary<string, PoolBox> _poolBoxDic;
    private Dictionary<string, PoolObjectInfo> _poolObjectInfoDic;
    private Dictionary<string, List<PoolObject>> _poolObjectDic;
    private Dictionary<string, UnityTimer> _delayTimerDic;

    public PoolUtil()
    {
        _poolBoxDic = new Dictionary<string, PoolBox>();
        _poolObjectInfoDic = new Dictionary<string, PoolObjectInfo>();
        _poolObjectDic = new Dictionary<string, List<PoolObject>>();
        _delayTimerDic = new Dictionary<string, UnityTimer>();

        //对象池根容器初始化
        _rootBox = new GameObject("PoolRoot").transform;
        UnityEngine.Object.DontDestroyOnLoad(_rootBox.gameObject);
        _prefabBox = new GameObject("PrefabBox").transform;
        _prefabBox.parent = _rootBox;
        AddBox(PoolBox.DefaultKey).Node.parent = _rootBox;
    }

    #region 对象池
    /// <summary>
    /// 实例化对象池容器
    /// </summary>
    /// <param name="key"></param>
    /// <param name="parentKey">父级Key 不存在则新建存放到根容器下</param>
    /// <returns></returns>
    public PoolBox CreateBox(string key, string parentKey = PoolBox.DefaultKey)
    {
        PoolBox parentBox = GetBox(parentKey);
        PoolBox poolBox = AddBox(key).BindParentBox(parentBox);
        return poolBox;
    }
    /// <summary>
    /// 获取对象池容器 没有则实例化
    /// </summary>
    public PoolBox GetBox(string key)
    {
        PoolBox poolBox;
        if (!_poolBoxDic.ContainsKey(key))
        {
            poolBox = CreateBox(key);
        }
        else
        {
            poolBox = _poolBoxDic[key];
        }

        return poolBox;
    }
    /// <summary>
    /// 添加对象池容器
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private PoolBox AddBox(string key)
    {
        PoolBox poolBox = new PoolBox(key);
        _poolBoxDic.Add(key, poolBox);
        return poolBox;
    }
    #endregion

    #region 对象实例
    /// <summary>
    /// 添加对象信息
    /// </summary>
    private PoolObjectInfo AddObjectInfo(string key, PoolObjectInfo info)
    {
        GameObject go = GameObject.Instantiate(info.Prefab);
        go.name = key;
        go.transform.parent = _prefabBox;
        go.SetActive(false);
        info.Prefab = go;
        info.PoolBox = GetBox(info.PoolBox.Key);
        _poolObjectInfoDic.Add(key, info);
        return info;
    }
    /// <summary>
    /// 添加PoolObject实例
    /// </summary>
    /// <param name="key"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    private PoolObject AddPoolObject(string key, PoolObjectInfo info)
    {
        GameObject go = GameObject.Instantiate(info.Prefab);
        go.name = key;
        go.transform.parent = info.PoolBox.Node;
        PoolObject poolObject = new PoolObject()
        {
            Key = key,
            GameObject = go,
            Info = info
        };
        _poolObjectDic[key].Add(poolObject);
        return poolObject;
    }
    /// <summary>
    /// 添加检测间隔时钟
    /// </summary>
    /// <param name="key"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private UnityTimer AddDelayTimer(string key, float time)
    {
        PoolObjectInfo info = _poolObjectInfoDic[key];
        UnityTimer timer = TimerUtil.Instance.On(time, true, delegate (object o, TimerEventArgs args)
        {
            if (GetPoolObjects(key).Length <= info.MaxSize) return;
            int delSum = GetPoolObjects(key).Length - info.MaxSize;
            delSum = delSum > info.SingleDelSum ? info.SingleDelSum : delSum;
            PoolObject[] disablePoolObjects = GetDisablePoolObjects(key);
            delSum = delSum > disablePoolObjects.Length ? disablePoolObjects.Length : delSum;
            for (int i = 0; i < delSum; i++)
            {
                PoolObject poolObject = disablePoolObjects[i];
                Destory(poolObject);
            }
        });
        return timer;
    }

    /// <summary>
    /// 通过游戏物体实例获取PoolObject
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public PoolObject GetPoolObjectInGameObject(GameObject gameObject)
    {
        PoolObject poolObject = null;
        foreach (var poolObjectList in _poolObjectDic.Values)
        {
            foreach (var VARIABLE in poolObjectList)
            {
                if (VARIABLE.GameObject == gameObject)
                {
                    poolObject = VARIABLE;
                    return poolObject;
                }
            }
        }
        return poolObject;
    }
    /// <summary>
    /// 获取PoolObject信息
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public PoolObjectInfo GetPoolObjectInfo(string key)
    {
        return _poolObjectInfoDic.ContainsKey(key) ? _poolObjectInfoDic[key] : null;
    }
    /// <summary>
    /// 获取PoolObject列表
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public PoolObject[] GetPoolObjects(string key)
    {
        return _poolObjectDic.ContainsKey(key) ? _poolObjectDic[key].ToArray() : null;
    }
    /// <summary>
    /// 获取已启用的PoolObject列表
    /// </summary>
    public PoolObject[] GetEnblePoolObjects(string key)
    {
        if (GetPoolObjects(key) == null) return null;
        List<PoolObject> poolObjectList = new List<PoolObject>();
        foreach (var poolObject in GetPoolObjects(key))
        {
            if (poolObject.IsEnabled)
            {
                poolObjectList.Add(poolObject);
            }
        }
        return poolObjectList.ToArray();
    }
    /// <summary>
    /// 获取未启用的PoolObject列表
    /// </summary>
    public PoolObject[] GetDisablePoolObjects(string key)
    {
        if (GetPoolObjects(key) == null) return null;
        List<PoolObject> poolObjectList = new List<PoolObject>();
        foreach (var poolObject in GetPoolObjects(key))
        {
            if (!poolObject.IsEnabled)
            {
                poolObjectList.Add(poolObject);
            }
        }
        return poolObjectList.ToArray();
    }

    /// <summary>
    /// 实例化对象池
    /// </summary>
    public bool CreateObject(string key, AssetInfo assetInfo, string poolBoxKey = PoolBox.DefaultKey)
    {
        PoolObjectInfo info = new PoolObjectInfo();
        info.PoolBox = GetBox(poolBoxKey);
        return CreateObject(key, assetInfo, info);
    }
    public bool CreateObject(string key, AssetInfo assetInfo, PoolObjectInfo info = null)
    {
        return CreateObject(key, AssetUtil.Instance.Load<GameObject>(assetInfo), info);
    }
    public bool CreateObject(string key, GameObject gameObject, string poolBoxKey = PoolBox.DefaultKey)
    {
        PoolObjectInfo info = new PoolObjectInfo();
        info.PoolBox = GetBox(poolBoxKey);
        return CreateObject(key, gameObject, info);
    }
    public bool CreateObject(string key, GameObject gameObject, PoolObjectInfo info = null)
    {
        if (gameObject == null) return false;
        info = info ?? new PoolObjectInfo();
        info.Prefab = gameObject;
        info = AddObjectInfo(key, info);
        _poolObjectDic.Add(key, new List<PoolObject>());
        for (int i = 0; i < info.MaxSize; i++)
        {
            PoolObject poolObject = AddPoolObject(key, info);
        }
        _delayTimerDic.Add(key, AddDelayTimer(key, info.DelDelayTime));
        return true;
    }

    /// <summary>
    /// 获取对象实例 没有则添加
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public PoolObject GetPoolObject(string key)
    {
        if (!_poolObjectInfoDic.ContainsKey(key)) return null;
        PoolObjectInfo info = _poolObjectInfoDic[key];
        PoolObject[] poolObjects = GetDisablePoolObjects(key);
        PoolObject poolObject = poolObjects.Length <= 0 ? AddPoolObject(key, info) : poolObjects[0];
        poolObject.IsEnabled = true;
        poolObject.GameObject.SetActive(true);
        return poolObject;
    }
    public GameObject Load(string key)
    {
        return GetPoolObject(key)?.GameObject;
    }

    /// <summary>
    /// 回收对象实例
    /// </summary>
    /// <param name="gameObject">实例</param>
    /// <param name="IsDestory">是否直接删除实例</param>
    public void UnLoad(GameObject gameObject, bool IsDestory = false)
    {
        PoolObject poolObject = GetPoolObjectInGameObject(gameObject);
        if (poolObject == null) return;
        UnLoad(poolObject, IsDestory);
    }
    public void UnLoad(PoolObject poolObject, bool IsDestory = false)
    {
        if (IsDestory)
        {
            Destory(poolObject);
        }
        else
        {
            Disabled(poolObject);
        }
    }

    /// <summary>
    /// 禁用对象
    /// </summary>
    /// <returns></returns>
    public void Disabled(GameObject gameObject)
    {
        PoolObject poolObject = GetPoolObjectInGameObject(gameObject);
        if (poolObject == null) return;
        Disabled(poolObject);
    }
    public void Disabled(PoolObject poolObject)
    {
        poolObject.IsEnabled = false;
        poolObject.GameObject.SetActive(false);
        poolObject.GameObject.transform.parent = _poolObjectInfoDic[poolObject.Key].PoolBox.Node;
    }

    /// <summary>
    /// 删除对象实例
    /// </summary>
    public void Destory(GameObject gameObject)
    {
        PoolObject poolObject = GetPoolObjectInGameObject(gameObject);
        if (poolObject == null) return;
        Destory(poolObject);
    }
    public void Destory(PoolObject poolObject)
    {
        _poolObjectDic[poolObject.Key].Remove(poolObject);
        GameObject.Destroy(poolObject.GameObject);
    }
    #endregion
}
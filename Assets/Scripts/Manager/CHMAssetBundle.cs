using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System.IO;
using UnityEditor;

[Serializable]
public class AssetBundleInfo
{
    public string name;

    public AssetBundleInfo(string _name)
    {
        name = _name;
    }
}

[Serializable]
public class AssetBundleData
{
    public List<AssetBundleInfo> listAssetBundleInfo = new List<AssetBundleInfo>();
}

public partial class AssetBundlePool
{
    [Serializable]
    public class Task
    {
        public string key;
        public Subject<AssetBundle> onDownloaded;
    }

    [Serializable]
    class Item
    {
        public string key;
        public AssetBundle obj;
    }

    private Dictionary<string, Item> dicItem = new Dictionary<string, Item>();
    private Dictionary<string, Task> dicTask = new Dictionary<string, Task>();

    public void LoadAssetBundle(string _bundleName, AssetBundle _assetBundle)
    {
        if (dicItem.TryGetValue(_bundleName, out Item item) == false && _assetBundle != null)
        {
            dicItem.Add(_bundleName, new Item
            {
                key = _bundleName,
                obj = _assetBundle,
            });
        }
    }

    public AssetBundle GetItem(string _bundleName)
    {
        if (dicItem.TryGetValue(_bundleName.ToLower(), out Item ret))
        {
            return ret.obj;
        }
        else
        {
            return null;
        }
    }
}

public class CHMAssetBundle
{
    private AssetBundlePool assetBundlePool = new AssetBundlePool();
    //private static ObjectPool objectPool = new ObjectPool();

    public void LoadAssetBundle(string _bundleName, AssetBundle _assetBundle)
    {
        assetBundlePool.LoadAssetBundle(_bundleName, _assetBundle);
    }

    public void LoadAsset<T>(string _bundleName, string _assetName, Action<T> _callback) where T : UnityEngine.Object
    {
        AssetBundle assetBundle = assetBundlePool.GetItem(_bundleName);

        if (assetBundle != null)
        {
            _callback(assetBundle.LoadAsset<T>(_assetName));
        }
        else
        {
            _callback(null);
        }
    }

#if UNITY_EDITOR
    public void LoadAssetOnEditor<T>(string _bundleName, string _assetName, Action<T> _callback) where T : UnityEngine.Object
    {
        string path = $"Assets/AssetBundleResources/{_bundleName.ToLower()}/{_assetName}";

        if (typeof(T) == typeof(GameObject))
        {
            path += ".prefab";
        }
        else if (typeof(T) == typeof(TextAsset))
        {
            path += ".json";
        }

        T original = AssetDatabase.LoadAssetAtPath<T>(path);

        _callback(original);
    }
#endif
}

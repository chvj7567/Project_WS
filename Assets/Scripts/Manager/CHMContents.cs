using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public enum EJsonType
{
    Korea,
    AssetBundleURL,
}

public class CHMContents
{
    [Serializable]
    class StringInfo
    {
        public int stringID = -1;
        public string value = "";
    }

    [Serializable]
    public class AssetBundleURL
    {
        public string name;
        public string url;
    }

    [Serializable]
    class SHContentsData
    {
        public StringInfo[] arrStringInfo;
        public AssetBundleURL[] arrAssetBundleURL;
    }

    int loadCompleteFileCount = 0;
    int loadingFileCount = 0;
    List<Action<TextAsset>> listAction = new List<Action<TextAsset>>();
    Dictionary<int, string> dicStringInfo = new Dictionary<int, string>();

    public void Init()
    {
        LoadContentsData();
    }

    void LoadContentsData()
    {
        loadCompleteFileCount = 0;
        listAction.Clear();

        listAction.Add(LoadStringInfo());

        loadingFileCount = listAction.Count;
    }

    float GetLoadingPercent()
    {
        if (loadingFileCount == 0 || loadCompleteFileCount == 0)
        {
            return 0;
        }

        return ((float)loadCompleteFileCount) / loadingFileCount;
    }

    Action<TextAsset> LoadStringInfo()
    {
        Action<TextAsset> callback;

        CHMMain.Resource.LoadString(EJsonType.Korea, callback = (TextAsset textAsset) =>
        {
            var contentsData = JsonUtility.FromJson<SHContentsData>(("{\"arrStringInfo\":" + textAsset.text + "}"));
            foreach (var data in contentsData.arrStringInfo)
            {
                dicStringInfo.Add(data.stringID, data.value);
            }

            ++loadCompleteFileCount;
        });

        return callback;
    }

    public string TryGetString(int _stringID)
    {
        if (dicStringInfo.TryGetValue(_stringID, out string result))
        {
            return result;
        }

        return "";
    }
}

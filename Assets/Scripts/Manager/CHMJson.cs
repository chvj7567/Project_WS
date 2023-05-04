using System;
using System.Collections.Generic;
using UnityEngine;
using static Infomation;

public class CHMJson
{
    [Serializable]
    public class JsonData
    {
        public StringInfo[] arrStringInfo;
        public UnitInfo[] arrUnitInfo;
    }

    int loadCompleteFileCount = 0;
    int loadingFileCount = 0;

    List<Action<TextAsset>> liAction = new List<Action<TextAsset>>();
    Dictionary<int, string> dicStringInfo = new Dictionary<int, string>();
    List<UnitInfo> liUnitInfo = new List<UnitInfo>();

    public void Init()
    {
        LoadJsonData();
    }

    public void Clear()
    {
        liAction.Clear();
        dicStringInfo.Clear();
        liUnitInfo.Clear();
    }

    void LoadJsonData()
    {
        loadCompleteFileCount = 0;
        liAction.Clear();

        liAction.Add(LoadStringInfo());
        liAction.Add(LoadUnitInfo());

        loadingFileCount = liAction.Count;
    }

    public float GetJsonLoadingPercent()
    {
        if (loadingFileCount == 0 || loadCompleteFileCount == 0)
        {
            return 0;
        }

        return ((float)loadCompleteFileCount) / loadingFileCount * 100f;
    }

    Action<TextAsset> LoadStringInfo()
    {
        Action<TextAsset> callback;

        dicStringInfo.Clear();

        CHMMain.Resource.LoadJson(Defines.EJsonType.String, callback = (TextAsset textAsset) =>
        {
            var jsonData = JsonUtility.FromJson<JsonData>(("{\"arrStringInfo\":" + textAsset.text + "}"));
            foreach (var data in jsonData.arrStringInfo)
            {
                dicStringInfo.Add(data.stringID, data.value);
            }

            ++loadCompleteFileCount;
        });

        return callback;
    }

    Action<TextAsset> LoadUnitInfo()
    {
        Action<TextAsset> callback;

        liUnitInfo.Clear();

        CHMMain.Resource.LoadJson(Defines.EJsonType.Unit, callback = (TextAsset textAsset) =>
        {
            var jsonData = JsonUtility.FromJson<JsonData>(("{\"arrUnitInfo\":" + textAsset.text + "}"));
            foreach (var data in jsonData.arrUnitInfo)
            {
                liUnitInfo.Add(data);
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

    public UnitInfo GetUnitInfo(Defines.EUnit _unitID)
    {
        return liUnitInfo.Find(_ => _.eUnit == _unitID);
    }
}

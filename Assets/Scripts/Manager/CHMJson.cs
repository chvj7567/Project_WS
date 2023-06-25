using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Infomation;

public class CHMJson
{
    [Serializable]
    public class JsonData
    {
        public StringInfo[] stringInfoArray;
        public PositionInfo[] positionInfoArray;
    }

    int loadCompleteFileCount = 0;
    int loadingFileCount = 0;

    List<Action<TextAsset>> actionList = new List<Action<TextAsset>>();
    Dictionary<int, string> stringInfoDic = new Dictionary<int, string>();
    List<PositionInfo> positionInfoList = new List<PositionInfo>();

    public void Init()
    {
        LoadJsonData();
    }

    public void Clear()
    {
        actionList.Clear();
        stringInfoDic.Clear();
    }

    void LoadJsonData()
    {
        loadCompleteFileCount = 0;
        actionList.Clear();

        actionList.Add(LoadStringInfo());
        actionList.Add(LoadPositionInfo());

        loadingFileCount = actionList.Count;
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

        stringInfoDic.Clear();

        CHMMain.Resource.LoadJson(Defines.EJsonType.String, callback = (TextAsset textAsset) =>
        {
            var jsonData = JsonUtility.FromJson<JsonData>(("{\"stringInfoArray\":" + textAsset.text + "}"));
            foreach (var data in jsonData.stringInfoArray)
            {
                stringInfoDic.Add(data.stringID, data.value);
            }

            ++loadCompleteFileCount;
        });

        return callback;
    }

    Action<TextAsset> LoadPositionInfo()
    {
        Action<TextAsset> callback;

        positionInfoList.Clear();

        CHMMain.Resource.LoadJson(Defines.EJsonType.Position, callback = (TextAsset textAsset) =>
        {
            var jsonData = JsonUtility.FromJson<JsonData>(("{\"positionInfoArray\":" + textAsset.text + "}"));
            foreach (var data in jsonData.positionInfoArray)
            {
                positionInfoList.Add(data);
            }

            ++loadCompleteFileCount;
        });

        return callback;
    }

    public string TryGetString(int _stringID)
    {
        if (stringInfoDic.TryGetValue(_stringID, out string result))
        {
            return result;
        }

        return "";
    }

    public List<PositionInfo> GetTeamPositionInfoList(int _stage, int _team)
    {
        return positionInfoList.FindAll(_ => _.stage == _stage && _.team == _team);
    }

    public Vector3 GetPositionFromPositionInfo(PositionInfo _positionInfo)
    {
        return new Vector3
        {
            x = _positionInfo.posX,
            y = _positionInfo.posY,
            z = _positionInfo.posZ
        };
    }

    public List<Vector3> GetTeamPositionList(int _stage, int _team)
    {
        List<Vector3> posList = new List<Vector3>();
        var tempPositionInfoList = GetTeamPositionInfoList(_stage, _team);
        foreach (var positionInfo in tempPositionInfoList)
        {
            posList.Add(new Vector3
            {
                x = positionInfo.posX,
                y = positionInfo.posY,
                z = positionInfo.posZ
            });
        }

        return posList;
    }
}

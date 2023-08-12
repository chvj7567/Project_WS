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
        public StringInfo[] stringInfoArr;
        public StageInfo[] stageInfoArr;
    }

    int loadCompleteFileCount = 0;
    int loadingFileCount = 0;

    List<Action<TextAsset>> actionList = new List<Action<TextAsset>>();
    Dictionary<int, string> stringInfoDic = new Dictionary<int, string>();
    List<StageInfo> stageInfoList = new List<StageInfo>();

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
        actionList.Add(LoadStageInfo());

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
            var jsonData = JsonUtility.FromJson<JsonData>(("{\"stringInfoArr\":" + textAsset.text + "}"));
            foreach (var data in jsonData.stringInfoArr)
            {
                stringInfoDic.Add(data.stringID, data.value);
            }

            ++loadCompleteFileCount;
        });

        return callback;
    }

    Action<TextAsset> LoadStageInfo()
    {
        Action<TextAsset> callback;

        stageInfoList.Clear();

        CHMMain.Resource.LoadJson(Defines.EJsonType.Stage, callback = (TextAsset textAsset) =>
        {
            var jsonData = JsonUtility.FromJson<JsonData>(("{\"stageInfoArr\":" + textAsset.text + "}"));
            foreach (var data in jsonData.stageInfoArr)
            {
                stageInfoList.Add(data);
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

    public List<StageInfo> GetStageInfoList(int _stage, int _team)
    {
        return stageInfoList.FindAll(_ => _.stage == _stage && _.team == _team);
    }

    public Vector3 GetPositionFromStageInfo(StageInfo _positionInfo)
    {
        return new Vector3
        {
            x = _positionInfo.posX,
            y = _positionInfo.posY,
            z = _positionInfo.posZ
        };
    }

    public List<Vector3> GetPositionListFromStageInfo(int _stage, int _team)
    {
        List<Vector3> posList = new List<Vector3>();
        var tempPositionInfoList = GetStageInfoList(_stage, _team);
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

    public List<Defines.EUnit> GetUnitListFromStageInfo(int _stage, int _team)
    {
        List<Defines.EUnit> unitList = new List<Defines.EUnit>();
        var stageInfoList = GetStageInfoList(_stage, _team);
        foreach (var stageInfo in stageInfoList)
        {
            unitList.Add(stageInfo.eUnit);
        }

        return unitList;
    }
}

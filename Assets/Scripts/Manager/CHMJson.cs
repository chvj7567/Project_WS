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
        public SkillInfo[] arrSkillInfo;
        public UnitInfo[] arrUnitInfo;
    }

    int loadCompleteFileCount = 0;
    int loadingFileCount = 0;

    List<Action<TextAsset>> liAction = new List<Action<TextAsset>>();
    Dictionary<int, string> dicStringInfo = new Dictionary<int, string>();
    List<SkillInfo> liSkillInfo = new List<SkillInfo>();
    List<UnitInfo> liUnitInfo = new List<UnitInfo>();

    public void Init()
    {
        LoadJsonData();
    }

    void LoadJsonData()
    {
        loadCompleteFileCount = 0;
        liAction.Clear();

        liAction.Add(LoadStringInfo());
        liAction.Add(LoadSkillInfo());
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

    Action<TextAsset> LoadSkillInfo()
    {
        Action<TextAsset> callback;

        liSkillInfo.Clear();

        CHMMain.Resource.LoadJson(Defines.EJsonType.Skill, callback = (TextAsset textAsset) =>
        {
            var jsonData = JsonUtility.FromJson<JsonData>(("{\"arrSkillInfo\":" + textAsset.text + "}"));
            foreach (var data in jsonData.arrSkillInfo)
            {
                liSkillInfo.Add(data);
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

    public SkillInfo GetSkillInfo(Defines.ESkillID _skillID)
    {
        return liSkillInfo.Find(_ => _.eSkillID == _skillID);
    }

    public UnitInfo GetUnitInfo(Defines.EUnitID _unitID)
    {
        return liUnitInfo.Find(_ => _.eUnitID == _unitID);
    }
}

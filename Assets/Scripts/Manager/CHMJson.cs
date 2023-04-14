using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Defines;
using static Infomation;

public class CHMJson
{
    [Serializable]
    public class StringJson
    {
        public int stringID = -1;
        public string value = "";
    }

    [Serializable]
    public class SkillJson
    {
        public ESkillID skillID = ESkillID.None;
        public int skillDesc = -1;
        public bool isTargeting = false;
        public float coolTime = -1f;
        public ESkillCost eSkillCost = ESkillCost.None;
        public float cost = -1f;
        public List<EffectInfo> liEffectInfo = new List<EffectInfo>(); // 스킬 그 자체
    }

    [Serializable]
    public class JsonData
    {
        public StringJson[] arrStringInfo;
        public SkillJson[] arrSkillInfo;
    }

    int loadCompleteFileCount = 0;
    int loadingFileCount = 0;

    List<Action<TextAsset>> liAction = new List<Action<TextAsset>>();
    Dictionary<int, string> dicStringInfo = new Dictionary<int, string>();
    List<SkillJson> liSkillInfo = new List<SkillJson>();

    public void Init()
    {
        LoadJsonData();
        LoadSkillInfo();
    }

    void LoadJsonData()
    {
        loadCompleteFileCount = 0;
        liAction.Clear();

        liAction.Add(LoadStringInfo());
        liAction.Add(LoadSkillInfo());

        loadingFileCount = liAction.Count;
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

        dicStringInfo.Clear();

        CHMMain.Resource.LoadJson(Defines.EJsonType.Korea, callback = (TextAsset textAsset) =>
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

    public string TryGetString(int _stringID)
    {
        if (dicStringInfo.TryGetValue(_stringID, out string result))
        {
            return result;
        }

        return "";
    }

    public SkillJson GetSkillInfo(Defines.ESkillID _skillID)
    {
        return liSkillInfo.Find(_ => _.skillID == _skillID);
    }
}

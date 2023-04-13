using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHMJson
{
    [Serializable]
    class StringJson
    {
        public int stringID = -1;
        public string value = "";
    }

    [Serializable]
    class SkillJson
    {
        public int skillID = -1;
        public int skillType = -1;
        public int skillTarget = -1;
        public int collision = -1;
        public int standardPos = -1;
        public int damageEffect = -1;
        public int damageType = -1;
        public float damage = 0f;
    }

    [Serializable]
    class JsonData
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
}

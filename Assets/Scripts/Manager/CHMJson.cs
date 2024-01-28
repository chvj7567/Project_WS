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
        public StageMonsterInfo[] stageMonsterInfoArr;
        public ShopInfo[] shopInfoArr;
        public ConstValueInfo[] constValueInfoArr;
    }

    int loadCompleteFileCount = 0;
    int loadingFileCount = 0;

    List<Action<TextAsset>> actionList = new List<Action<TextAsset>>();
    Dictionary<int, string> stringInfoDic = new Dictionary<int, string>();
    List<StageInfo> stageInfoList = new List<StageInfo>();
    List<StageMonsterInfo> stageMonsterInfoList = new List<StageMonsterInfo>();
    List<ShopInfo> shopInfoList = new List<ShopInfo>();
    List<ConstValueInfo> constValueInfoList = new List<ConstValueInfo>();

    public void Init()
    {
        LoadJsonData();
    }

    public void Clear()
    {
        actionList.Clear();
        stringInfoDic.Clear();
        stageInfoList.Clear();
        stageMonsterInfoList.Clear();
        shopInfoList.Clear();
        constValueInfoList.Clear();
    }

    void LoadJsonData()
    {
        loadCompleteFileCount = 0;
        actionList.Clear();

        actionList.Add(LoadStringInfo());
        actionList.Add(LoadStageInfo());
        actionList.Add(LoadStageMonsterInfo());
        actionList.Add(LoadShopInfo());
        actionList.Add(LoadConstValueInfo());

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

    Action<TextAsset> LoadStageMonsterInfo()
    {
        Action<TextAsset> callback;

        stageMonsterInfoList.Clear();

        CHMMain.Resource.LoadJson(Defines.EJsonType.StageMonster, callback = (TextAsset textAsset) =>
        {
            var jsonData = JsonUtility.FromJson<JsonData>(("{\"stageMonsterInfoArr\":" + textAsset.text + "}"));
            foreach (var data in jsonData.stageMonsterInfoArr)
            {
                stageMonsterInfoList.Add(data);
            }

            ++loadCompleteFileCount;
        });

        return callback;
    }

    Action<TextAsset> LoadShopInfo()
    {
        Action<TextAsset> callback;

        shopInfoList.Clear();

        CHMMain.Resource.LoadJson(Defines.EJsonType.Shop, callback = (TextAsset textAsset) =>
        {
            var jsonData = JsonUtility.FromJson<JsonData>(("{\"shopInfoArr\":" + textAsset.text + "}"));
            foreach (var data in jsonData.shopInfoArr)
            {
                shopInfoList.Add(data);
            }

            ++loadCompleteFileCount;
        });

        return callback;
    }

    Action<TextAsset> LoadConstValueInfo()
    {
        Action<TextAsset> callback;

        constValueInfoList.Clear();

        CHMMain.Resource.LoadJson(Defines.EJsonType.ConstValue, callback = (TextAsset textAsset) =>
        {
            var jsonData = JsonUtility.FromJson<JsonData>(("{\"constValueInfoArr\":" + textAsset.text + "}"));
            foreach (var data in jsonData.constValueInfoArr)
            {
                constValueInfoList.Add(data);
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

    public List<StageInfo> GetStageInfo(int stage)
    {
        return stageInfoList?.FindAll(_ => _.stage == stage);
    }

    public StageInfo GetStageInfo(int stage, int wave)
    {
        return stageInfoList?.Find(_ => _.stage == stage && _.wave == wave);
    }

    public ShopInfo GetShopInfo(Defines.EShop shopID, int step)
    {
        return shopInfoList.Find(_ => _.shopID == shopID && _.step == step);
    }

    public List<StageMonsterInfo> GetMonsterInfo(int stage, int wave)
    {
        return stageMonsterInfoList?.FindAll(_ => _.stage == stage && _.wave == wave);
    }

    public int GetMonsterCount(int stage, int wave)
    {
        var liMonsterInfo = GetMonsterInfo(stage, wave);
        if (liMonsterInfo == null)
            return 0;

        int count = 0;
        for (int i = 0;  i < liMonsterInfo.Count; i++)
        {
            count += liMonsterInfo[i].monsterCount;
        }

        return count;
    }

    public float GetConstValue(Defines.EConstValue eConst)
    {
        if (constValueInfoList == null)
            return -1f;

        var find = constValueInfoList.Find(_ => _.eConst == eConst);
        if (find == null)
            return -1f;

        return find.value;
    }
}

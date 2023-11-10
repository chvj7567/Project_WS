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
        public StageGoldInfo[] stageGoldInfoArr;
        public StageMonsterInfo[] stageMonsterInfoArr;
        public ShopInfo[] shopInfoArr;
    }

    int loadCompleteFileCount = 0;
    int loadingFileCount = 0;

    List<Action<TextAsset>> actionList = new List<Action<TextAsset>>();
    Dictionary<int, string> stringInfoDic = new Dictionary<int, string>();
    List<StageGoldInfo> stageGoldInfoList = new List<StageGoldInfo>();
    List<StageMonsterInfo> stageMonsterInfoList = new List<StageMonsterInfo>();
    List<ShopInfo> shopInfoList = new List<ShopInfo>();

    public void Init()
    {
        LoadJsonData();
    }

    public void Clear()
    {
        actionList.Clear();
        stringInfoDic.Clear();
        stageGoldInfoList.Clear();
        stageMonsterInfoList.Clear();
        shopInfoList.Clear();

    }

    void LoadJsonData()
    {
        loadCompleteFileCount = 0;
        actionList.Clear();

        actionList.Add(LoadStringInfo());
        actionList.Add(LoadStageGoldInfo());
        actionList.Add(LoadStageMonsterInfo());
        actionList.Add(LoadShopInfo());

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

    Action<TextAsset> LoadStageGoldInfo()
    {
        Action<TextAsset> callback;

        stageGoldInfoList.Clear();

        CHMMain.Resource.LoadJson(Defines.EJsonType.StageGold, callback = (TextAsset textAsset) =>
        {
            var jsonData = JsonUtility.FromJson<JsonData>(("{\"stageGoldInfoArr\":" + textAsset.text + "}"));
            foreach (var data in jsonData.stageGoldInfoArr)
            {
                stageGoldInfoList.Add(data);
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

    public string TryGetString(int _stringID)
    {
        if (stringInfoDic.TryGetValue(_stringID, out string result))
        {
            return result;
        }

        return "";
    }

    public StageGoldInfo GetStageInfo(int stage)
    {
        return stageGoldInfoList.Find(_ => _.stage == stage);
    }

    public ShopInfo GetShopInfo(Defines.EShop shopID, int step)
    {
        return shopInfoList.Find(_ => _.shopID == shopID && _.step == step);
    }

    public StageMonsterInfo GetMonsterInfo(int stage)
    {
        if (stageMonsterInfoList == null)
            return null;

        var monsterInfo = stageMonsterInfoList.Find(_ => _.stage == stage);
        if (monsterInfo == null)
            return null;

        return monsterInfo;
    }
}

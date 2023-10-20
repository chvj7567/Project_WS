using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
    List<Value> MakeList(Dictionary<Key, Value> dict);
}

public class CHMData : CHSingleton<CHMData>
{
    public Dictionary<string, Data.Player> playerDataDic = null;
    public Dictionary<string, Data.Shop> shopDataDic = null;

    public async Task LoadLocalData(string _path)
    {
        Debug.Log("Local Data Load");

        if (playerDataDic == null)
        {
            Debug.Log("Player Local Data Load");
            var data = await LoadJsonToLocal<Data.ExtractData<Data.Player>, string, Data.Player>(_path, Defines.EData.Player.ToString());
            playerDataDic = data.MakeDict();
        }

        if (shopDataDic == null)
        {
            Debug.Log("Shop Local Data Load");
            var data = await LoadJsonToLocal<Data.ExtractData<Data.Shop>, string, Data.Shop>(_path, Defines.EData.Shop.ToString());
            shopDataDic = data.MakeDict();
        }
    }

    async Task<Loader> LoadJsonToLocal<Loader, Key, Value>(string _path, string _name) where Loader : ILoader<Key, Value>
    {
        string path = $"{Application.persistentDataPath}/{_path}.json";

        Debug.Log($"Local Path : {path}");
        if (File.Exists(path) == false)
        {
            Debug.Log("Path is Null");
            return await LoadDefaultData<Loader>(_name);
        }
        else
        {
            var data = File.ReadAllText(path);

            // 데이터가 없을 경우 디폴트 데이터 저장
            if (data.Contains($"{_name.ToLower()}List") == false || data.Contains($"\"{_name.ToLower()}List\":[]"))
            {
                return await LoadDefaultData<Loader>(_name);
            }
            else
            {
                return JsonUtility.FromJson<Loader>(File.ReadAllText(path));
            }
        }
    }

    async Task<Loader> LoadDefaultData<Loader>(string _name)
    {
        TaskCompletionSource<TextAsset> taskCompletionSource = new TaskCompletionSource<TextAsset>();

        CHMMain.Resource.LoadData(_name, (data) =>
        {
            Debug.Log($"Load Default {_name} Data is {data}");
            taskCompletionSource.SetResult(data);
        });

        var task = await taskCompletionSource.Task;

        return JsonUtility.FromJson<Loader>($"{{\"{_name.ToLower()}List\":{task.text}}}");
    }

    public void SaveData(string _path)
    {
        string json = "";

        Data.ExtractData<Object> saveData = new Data.ExtractData<Object>();

        Data.ExtractData<Data.Player> playerData = new Data.ExtractData<Data.Player>();
        saveData.playerList = playerData.MakeList(playerDataDic);

        Data.ExtractData<Data.Shop> shopData = new Data.ExtractData<Data.Shop>();
        saveData.shopList = shopData.MakeList(shopDataDic);

#if UNITY_EDITOR == false
        json = JsonUtility.ToJson(saveData);
#else
        json = JsonUtility.ToJson(saveData, true);
#endif

        Debug.Log($"Save Local Data is {json}");

        File.WriteAllText($"{Application.persistentDataPath}/{_path}.json", json);
    }

#if UNITY_EDITOR == false
public async Task LoadCloudData(string _path)
    {
        Debug.Log("Cloud Data Load");

        if (playerDataDic == null)
        {
            Debug.Log("Player Cloud Data Load");
            var data = await LoadJsonToGPGSCloud<Data.ExtractData<Data.Player>, string, Data.Player>(_path, Defines.EData.Player.ToString());
            playerDataDic = data.MakeDict();
        }

        if (shopDataDic == null)
        {
            Debug.Log("Shop Local Data Load");
            var data = await LoadJsonToGPGSCloud<Data.ExtractData<Data.Shop>, string, Data.Shop>(_path, Defines.EData.Shop.ToString());
            shopDataDic = data.MakeDict();
        }
    }
public async Task<Loader> LoadJsonToGPGSCloud<Loader, Key, Value>(string _path, string _name) where Loader : ILoader<Key, Value>
    {
        TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();

        CHMGPGS.Instance.LoadCloud(_path, (success, data) =>
        {
            Debug.Log($"Load Cloud {_name} Data is {success} : {data}");
            taskCompletionSource.SetResult(data);
        });

        var stringTask = await taskCompletionSource.Task;

        // 데이터가 없을 경우 디폴트 데이터 저장
        if (stringTask.Contains($"{_name.ToLower()}List") == false || stringTask.Contains($"\"{_name.ToLower()}List\":[]"))
        {
            return await LoadDefaultData<Loader>(_name);
        }

        return JsonUtility.FromJson<Loader>(stringTask);
    }
#endif
    Data.Player CreatePlayerData(string key)
    {
        Debug.Log($"Create Player {key}");

        Data.Player data = new Data.Player
        {
            key = key
        };

        playerDataDic.Add(key, data);

        return data;
    }

    public Data.Player GetPlayerData(string key)
    {
        if (CHMData.Instance.playerDataDic.TryGetValue(key, out var data) == false)
        {
            data = null;
        }

        return data;
    }

    Data.Shop CreateShopData(Defines.EShop key)
    {
        Debug.Log($"Create Shop {key}");

        Data.Shop data = new Data.Shop
        {
            shopID = key
        };

        shopDataDic.Add(key.ToString(), data);

        return data;
    }

    public Data.Shop GetShopData(Defines.EShop key)
    {
        if (CHMData.Instance.shopDataDic.TryGetValue(key.ToString(), out var data) == false)
        {
            data = CreateShopData(key);
        }

        return data;
    }
}

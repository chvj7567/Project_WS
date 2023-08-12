using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
    List<Value> MakeList(Dictionary<Key, Value> dict);
}

public class CHMData
{
    public Dictionary<string, Data.Player> playerDataDic = new Dictionary<string, Data.Player>();

    string _playerPath;

    public async void Init()
    {
        _playerPath = $"{Application.persistentDataPath}/{Defines.EData.Player.ToString()}.json";

        Debug.Log(_playerPath);

        var playerData = await LoadJson<Data.ExtractData<Data.Player>, string, Data.Player>(Defines.EData.Player.ToString());

        playerDataDic = playerData.MakeDict();
    }

    async Task<Loader> LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        if (path == Defines.EData.Player.ToString())
        {
            if (File.Exists(_playerPath) == false)
            {
                TaskCompletionSource<TextAsset> taskCompletionSource = new TaskCompletionSource<TextAsset>();

                CHMMain.Resource.LoadPlayerData((data) =>
                {
                    taskCompletionSource.SetResult(data);
                });

                var textAsset = await taskCompletionSource.Task;

                return JsonUtility.FromJson<Loader>(textAsset.text);
            }
            else
            {
                return JsonUtility.FromJson<Loader>(File.ReadAllText(_playerPath));
            }
        }

        return default(Loader);
    }

    public void SaveJson()
    {
        Data.ExtractData<Data.Player> gamePlayerData = new Data.ExtractData<Data.Player>();

        gamePlayerData.playerList = gamePlayerData.MakeList(playerDataDic);

        string json = JsonUtility.ToJson(gamePlayerData);
        File.WriteAllText(_playerPath, json);
    }
}

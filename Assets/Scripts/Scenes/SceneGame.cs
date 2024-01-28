using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Infomation;

public class SceneGame : MonoBehaviour
{
    [SerializeField] GameObject targetObj;
    [SerializeField] List<Transform> liDest = new List<Transform>();
    [SerializeField] List<CHSpawner> liSpawner = new List<CHSpawner>();

    [SerializeField, ReadOnly] int totSpawnCount;

    void Start()
    {
        StartSpawn(1);
    }

    async void StartSpawn(int stage)
    {
        List<StageInfo> liStageInfo = CHMMain.Json.GetStageInfo(stage);

        for (int i = 0; i < liStageInfo.Count; i++)
        {
            Debug.Log($"{liStageInfo[i].stage} / {liStageInfo[i].wave}");
            StartSpawn(liStageInfo[i].stage, liStageInfo[i].wave);

            await Task.Delay((int)(1000 * liStageInfo[i].waveTime));;
        }
    }

    void StartSpawn(int stage, int wave)
    {
        StageInfo stageInfo = CHMMain.Json.GetStageInfo(stage, wave);
        int monsterCount = CHMMain.Json.GetMonsterCount(stage, wave);
        totSpawnCount = monsterCount;

        int oneSpawnerSpawnCount = monsterCount / liSpawner.Count;
        int remainSpawnCount = monsterCount % liSpawner.Count;
        foreach (CHSpawner spawner in liSpawner)
        {
            spawner.SetDest(liDest);
            spawner.Target = targetObj;
            spawner.SpawnDelay = stageInfo.spawnDelay;
            spawner.StartSpawn(oneSpawnerSpawnCount);
        }
    }
}

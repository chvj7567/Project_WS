using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using static Defines;

public class CHSpawner : MonoBehaviour
{
    [SerializeField] Transform createPosition;
    [SerializeField] List<Transform> destList = new List<Transform>();
    [SerializeField] Defines.EUnit unit;
    [SerializeField] bool onTargetTracker = true;

    [SerializeField, ReadOnly] int totSpawnCount = 0; // 총 스폰 카운트
    [SerializeField, ReadOnly] int oneTimeSpawnCount = 0; // 한 번 스폰할 때 스폰 카운트(maxSpawnCount 까지)
    [SerializeField, ReadOnly] int maxSpawnCount = 0; // 한 번 스폰할 때 지정한 스폰 카운트

    bool isSpawn;
    CancellationTokenSource cts;

    public Action end;

    public GameObject Target { get; set; }

    public float SpawnDelay { get; set; }

    public void SetUnit(Defines.EUnit unit)
    {
        this.unit = unit;
    }

    public void SetDest(List<Transform> destList)
    {
        if (destList == null)
            return;

        this.destList = destList;
    }

    public int GetMaxSpawnCount()
    {
        return maxSpawnCount;
    }

    private async Task SpawnLoopAsync()
    {
        while (!cts.Token.IsCancellationRequested)
        {
            GameObject obj = Target == null ? CHMMain.Resource.Instantiate(CHMMain.Unit.GetOriginBall()) : CHMMain.Resource.Instantiate(Target);
            if (obj != null)
            {
                CHMMain.Unit.SetUnit(obj, unit);
                CHMMain.Unit.SetTargetMask(obj, Defines.ELayer.None);
                obj.transform.position = createPosition.position;
                obj.layer = (int)Defines.ELayer.Red;

                var unitBase = obj.GetComponent<CHUnitBase>();
                if (unitBase != null)
                {
                    unitBase.onHpBar = true;
                    unitBase.onMpBar = false;
                    unitBase.onCoolTimeBar = false;

                    unitBase.Active();
                }

                var targetTracker = obj.GetOrAddComponent<CHTargetMover>();
                if (targetTracker != null)
                {
                    if (onTargetTracker == false)
                    {
                        Destroy(targetTracker);
                    }
                    else
                    {
                        targetTracker.SetDest(createPosition.position, destList);
                        targetTracker.StartRun();
                    }
                }

                ++totSpawnCount;

                if (maxSpawnCount > 0)
                {
                    ++oneTimeSpawnCount;
                    if (oneTimeSpawnCount >= maxSpawnCount)
                    {
                        StopSpawn();
                    }
                }
            }

            await Task.Delay((int)(SpawnDelay * 1000f), cts.Token);
        }
    }

    public void StartSpawn(int maxSpawnCount = 0)
    {
        if (!isSpawn)
        {
            this.maxSpawnCount = maxSpawnCount;
            isSpawn = true;
            cts = new CancellationTokenSource();
            cts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
            _ = SpawnLoopAsync();
        }
    }

    public void StopSpawn()
    {
        if (isSpawn)
        {
            oneTimeSpawnCount = 0;
            isSpawn = false;
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
            }
        }
    }

    private void OnDestroy()
    {
        StopSpawn();
    }
}
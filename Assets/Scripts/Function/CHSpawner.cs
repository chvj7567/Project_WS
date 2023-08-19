using DG.Tweening;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class CHSpawner : MonoBehaviour
{
    [SerializeField] Transform trDestination;
    [SerializeField] float spawnDelay = 1f;
    [SerializeField] Defines.EUnit unit;
    [SerializeField] bool onTargetTracker = true;

    [SerializeField, ReadOnly] int totSpawnCount = 0; // 총 스폰 카운트

    [SerializeField, ReadOnly] int oneTimeSpawnCount = 0; // 한 번 스폰할 때 스폰 카운트(maxSpawnCount 까지)
    [SerializeField, ReadOnly] int maxSpawnCount = 0; // 한 번 스폰할 때 지정한 스폰 카운트

    bool isSpawn;
    CancellationTokenSource cts;

    public void SetSpawnDelay(float _value)
    {
        spawnDelay = _value;
    }

    private async Task SpawnLoopAsync()
    {
        while (!cts.Token.IsCancellationRequested)
        {
            var obj = CHMMain.Resource.Instantiate(CHMMain.Unit.GetOriginBall());
            if (obj != null)
            {
                CHMMain.Unit.SetUnit(obj, unit);
                CHMMain.Unit.SetColor(obj, unit);
                CHMMain.Unit.SetTargetMask(obj, Defines.ELayer.Blue);
                obj.transform.localPosition = transform.position;
                obj.layer = (int)Defines.ELayer.Red;

                var targetTracker = obj.GetComponent<CHTargetTracker>();
                if (targetTracker != null)
                {
                    if (onTargetTracker == false)
                    {
                        Destroy(targetTracker);
                    }
                    else
                    {
                        targetTracker.trDestination = trDestination;
                    }
                }

                var unitBase = obj.GetComponent<CHUnitBase>();
                if (unitBase != null)
                {
                    unitBase.onHpBar = true;
                    unitBase.onMpBar = false;
                    unitBase.onCoolTimeBar = false;
                }

                obj.SetActive(true);
                ++totSpawnCount;

                if (maxSpawnCount > 0)
                {
                    if (++oneTimeSpawnCount >= maxSpawnCount)
                    {
                        StopSpawn();
                    }
                }
            }

            await Task.Delay((int)(spawnDelay * 1000f), cts.Token);
        }
    }

    public void StartSpawn()
    {
        if (!isSpawn)
        {
            maxSpawnCount = 0;
            isSpawn = true;
            cts = new CancellationTokenSource();
            cts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
            _ = SpawnLoopAsync();
        }
    }

    public void StartSpawn(int _maxSpawnCount)
    {
        if (!isSpawn)
        {
            maxSpawnCount = _maxSpawnCount;
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
            maxSpawnCount = 0;
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
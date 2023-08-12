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
                obj.transform.localPosition = transform.position;
                obj.layer = (int)Defines.ELayer.Red;

                var targetTracker = obj.GetComponent<CHTargetTracker>();
                if (targetTracker != null)
                {
                    Destroy(targetTracker);
                }

                var agent = obj.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.SetDestination(trDestination.position);
                }

                obj.SetActive(true);
            }

            await Task.Delay((int)(spawnDelay * 1000f), cts.Token);
        }
    }

    public void StartSpawn()
    {
        if (!isSpawn)
        {
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
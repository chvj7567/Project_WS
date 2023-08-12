using System.Threading;
using System.Threading.Tasks;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class CHSpawner : MonoBehaviour
{
    [SerializeField] Transform trDestination;
    [SerializeField] float spawnDelay = 1f;

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
            var obj = CHMMain.Resource.Instantiate(CHMMain.Unit.GetOriginBall(), transform);
            if (obj != null)
            {
                obj.transform.localPosition = Vector3.zero;
                obj.layer = (int)Defines.ELayer.Red;
                if (trDestination)
                {
                    var targetTracker = obj.GetComponent<CHTargetTracker>();
                    targetTracker.trDestination = trDestination;
                }

                var unitBase = obj.GetComponent<CHUnitBase>();
                if (unitBase != null)
                {
                    unitBase.gameObject.SetActive(true);
                }
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
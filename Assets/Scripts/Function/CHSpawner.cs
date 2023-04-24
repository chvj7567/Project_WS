using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class CHSpawner : MonoBehaviour
{
    [SerializeField] GameObject objSpawn;
    [SerializeField] Transform trDestination;
    [SerializeField] float spawnDelay;

    public bool isSpawn;

    CancellationTokenSource cts;

    private void Start()
    {
        cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        StartSpawn(token);
    }

    public void SetSpawnDelay(float _value)
    {
        spawnDelay = _value;
    }

    public async void StartSpawn(CancellationToken cancellationToken)
    {
        isSpawn = true;
        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        while (!cancellationToken.IsCancellationRequested && isSpawn)
        {
            try
            {
                var obj = CHMMain.Resource.Instantiate(objSpawn, transform);
                obj.transform.localPosition = Vector3.zero;

                var targetTracker = obj.GetComponent<CHTargetTracker>();
                targetTracker.trDestination = trDestination;

                var unitBase = obj.GetComponent<CHUnitBase>();
                if (unitBase != null && unitBase.GetCurrentHp() < 1f)
                {
                    unitBase.Reset();
                }

                await Task.Delay((int)(spawnDelay * 1000f), cts.Token);
            }
            catch (TaskCanceledException)
            {
                
            }
        }
    }

    public void StopSpawn()
    {
        isSpawn = false;
        if (cts != null && !cts.IsCancellationRequested)
        {
            cts.Cancel();
        }
    }

    private void OnDestroy()
    {
        StopSpawn();
    }
}
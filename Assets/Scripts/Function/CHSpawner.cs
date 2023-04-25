using System;
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

        StartSpawn(cts.Token);
    }

    public void SetSpawnDelay(float _value)
    {
        spawnDelay = _value;
    }

    public async void StartSpawn(CancellationToken _token)
    {
        isSpawn = true;
        cts = CancellationTokenSource.CreateLinkedTokenSource(_token);

        while (!_token.IsCancellationRequested && isSpawn)
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
                Debug.Log("스폰 도중 종료");
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

    private void OnApplicationQuit()
    {
        StopSpawn();
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class CHSpawner : MonoBehaviour
{
    [SerializeField] GameObject objSpawn;
    [SerializeField] Transform trDestination;
    [SerializeField] float spawnDelay;

    public bool isSpawn;

    private void Start()
    {
        StartSpawn();
    }

    public void SetSpawnDelay(float _value)
    {
        spawnDelay = _value;
    }

    public async void StartSpawn()
    {
        isSpawn = true;

        while (isSpawn)
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

            await Task.Delay((int)(spawnDelay * 1000f));
        }
    }

    public void StopSpawn()
    {
        isSpawn = false;
    }
}

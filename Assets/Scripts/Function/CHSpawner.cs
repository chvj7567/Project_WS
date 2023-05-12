using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CHSpawner : MonoBehaviour
{
    [SerializeField] GameObject objSpawn;
    [SerializeField] Transform trDestination;
    [SerializeField] float spawnDelay = 1f;

    public bool isSpawn;

    CancellationTokenSource cts;
    CancellationToken token;

    private void Start()
    {
        cts = new CancellationTokenSource();
        token = cts.Token;

        StartSpawn();
    }

    public void SetSpawnDelay(float _value)
    {
        spawnDelay = _value;
    }

    public async void StartSpawn()
    {
        isSpawn = true;
        cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        while (!token.IsCancellationRequested && isSpawn)
        {
            try
            {
                var obj = CHMMain.Resource.Instantiate(objSpawn, transform);
                obj.transform.localPosition = Vector3.zero;

                if (trDestination)
                {
                    var targetTracker = obj.GetComponent<CHTargetTracker>();
                    targetTracker.trDestination = trDestination;
                }

                var unitBase = obj.GetComponent<CHUnitBase>();
                if (unitBase != null)
                {
                    unitBase.ResetUnit();
                    unitBase.gameObject.SetActive(true);
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

    private void OnDestroy()
    {
        StopSpawn();
    }
}
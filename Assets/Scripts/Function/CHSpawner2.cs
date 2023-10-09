using DG.Tweening;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class CHSpawner2 : MonoBehaviour
{
    [SerializeField] GameObject spawnObj;
    [SerializeField] PathTD pathTD;
    [SerializeField] float moveSpeed;
    [SerializeField] float spawnDelay = 1f;
    [SerializeField, ReadOnly] int totSpawnCount = 0; // 총 스폰 카운트
    [SerializeField, ReadOnly] int oneTimeSpawnCount = 0; // 한 번 스폰할 때 스폰 카운트(maxSpawnCount 까지)
    [SerializeField, ReadOnly] int maxSpawnCount = 0; // 한 번 스폰할 때 지정한 스폰 카운트

    [SerializeField] bool isSpawn;
    CancellationTokenSource cts;

    private void Start()
    {
        StartSpawn();
    }
    public void SetSpawnDelay(float _value)
    {
        spawnDelay = _value;
    }

    private async Task SpawnLoopAsync()
    {
        while (!cts.Token.IsCancellationRequested)
        {
            var obj = CHMMain.Resource.Instantiate(spawnObj);
            if (obj != null)
            {
                StartCoroutine(MoveToTransformList(obj));

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

    IEnumerator MoveToTransformList(GameObject _moveObj)
    {
        var unit = _moveObj.GetOrAddComponent<CHUnitBase>();
        for (int i = 0; i < pathTD.wpList.Count; ++i)
        {
            if (unit.GetIsDeath())
                break;

            if (i == 0)
            {
                _moveObj.transform.position = pathTD.wpList[i].position;
                continue;
            }

            float distance = Vector3.Distance(pathTD.wpList[i].position, pathTD.wpList[i - 1].position);
            // DOTween을 사용하여 현재 Transform을 움직임
            _moveObj.transform.DOMove(pathTD.wpList[i].position, distance / moveSpeed)
                .SetEase(Ease.Linear);

            // 현재 Transform의 이동이 완료될 때까지 기다림
            yield return new WaitForSeconds(distance / moveSpeed);
        }

        CHMMain.Resource.Destroy(_moveObj);
    }
}
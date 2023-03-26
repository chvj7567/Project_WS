using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;

    public Transform bulletSpawnPoint;

    public float bulletForce = 10f;

    async void Update()
    {
        // 플레이어가 발사 버튼을 눌렀는지 확인
        if (Input.GetButtonDown("Fire1"))
        {
            // 스폰 지점에서 새 총알 생성
            GameObject bullet = CHMMain.Resource.Instantiate(bulletPrefab, bulletSpawnPoint);
            bullet.transform.localPosition = Vector3.zero;
            bullet.transform.up = bulletSpawnPoint.transform.up;

            // 총알을 스폰 지점의 방향으로 힘을 가해 발사
            bullet.GetComponent<Rigidbody>().AddForce(bulletSpawnPoint.up * bulletForce, ForceMode.Impulse);

            await Task.Delay(2000);

            CHMMain.Resource.Destroy(bullet);
        }
    }
}

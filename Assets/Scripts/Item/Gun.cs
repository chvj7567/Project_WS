using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float bulletForce = 10f;
    [SerializeField] float fireDelay = .1f;

    [SerializeField, ReadOnly] float timeSinceLastFire = 0f;
    [SerializeField, ReadOnly] bool isFire = false;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            isFire = true;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            isFire = false;
        }

        if (timeSinceLastFire < fireDelay)
        {
            timeSinceLastFire += Time.deltaTime;
        }
        else if(isFire)
        {
            Fire();
        }
    }

    void Fire()
    {
        // 스폰 지점에서 새 총알 생성
        GameObject bullet = CHMMain.Resource.Instantiate(bulletPrefab, bulletSpawnPoint);
        bullet.transform.localPosition = Vector3.zero;
        bullet.transform.up = bulletSpawnPoint.transform.up;

        // 총알을 스폰 지점의 방향으로 힘을 가해 발사
        bullet.GetOrAddComponent<Bullet>();

        // 총알을 스폰 지점의 방향으로 힘을 가해 발사
        bullet.GetOrAddComponent<Rigidbody>().AddForce(bulletSpawnPoint.up * bulletForce, ForceMode.Impulse);

        // 딜레이 초기화
        timeSinceLastFire = 0f;
    }
}

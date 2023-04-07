using UnityEngine;

public class ContGun : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float bulletForce = 10f;
    [SerializeField] float fireDelay = .1f;
    [SerializeField] bool isFire = false;
    [SerializeField, ReadOnly] float timeSinceLastFire = 0f;
    
    public void IsFire(bool _isFire)
    {
        isFire = _isFire;
    }

    void Update()
    {
        if (isFire)
        {
            if (timeSinceLastFire < fireDelay)
            {
                timeSinceLastFire += Time.deltaTime;
            }
            else
            {
                Fire();
            }
        }
    }

    void Fire()
    {
        // 스폰 지점에서 새 총알 생성
        GameObject bullet = CHMMain.Resource.Instantiate(bulletPrefab, bulletSpawnPoint);
        bullet.transform.localPosition = Vector3.zero;
        bullet.transform.up = bulletSpawnPoint.transform.up;

        // 총알을 스폰 지점의 방향으로 힘을 가해 발사
        bullet.GetOrAddComponent<ContBullet>();
        // 총알을 스폰 지점의 방향으로 힘을 가해 발사
        bullet.GetOrAddComponent<Rigidbody>().AddForce(bulletSpawnPoint.up * bulletForce, ForceMode.Impulse);

        // 딜레이 초기화
        timeSinceLastFire = 0f;
    }
}

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
        // ���� �������� �� �Ѿ� ����
        GameObject bullet = CHMMain.Resource.Instantiate(bulletPrefab, bulletSpawnPoint);
        bullet.transform.localPosition = Vector3.zero;
        bullet.transform.up = bulletSpawnPoint.transform.up;

        // �Ѿ��� ���� ������ �������� ���� ���� �߻�
        bullet.GetOrAddComponent<Bullet>();

        // �Ѿ��� ���� ������ �������� ���� ���� �߻�
        bullet.GetOrAddComponent<Rigidbody>().AddForce(bulletSpawnPoint.up * bulletForce, ForceMode.Impulse);

        // ������ �ʱ�ȭ
        timeSinceLastFire = 0f;
    }
}

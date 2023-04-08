using UniRx.Triggers;
using UnityEngine;
using UniRx;

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

    private void Start()
    {
        gameObject.UpdateAsObservable().Subscribe(_ =>
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
        });
    }

    void Fire()
    {
        // ���� �������� �� �Ѿ� ����
        GameObject bullet = CHMMain.Resource.Instantiate(bulletPrefab, bulletSpawnPoint);
        bullet.transform.localPosition = Vector3.zero;
        bullet.transform.up = bulletSpawnPoint.transform.up;

        // �Ѿ��� ���� ������ �������� ���� ���� �߻�
        bullet.GetOrAddComponent<ContBullet>().Init(transform.forward, bulletForce);

        // ������ �ʱ�ȭ
        timeSinceLastFire = 0f;
    }
}

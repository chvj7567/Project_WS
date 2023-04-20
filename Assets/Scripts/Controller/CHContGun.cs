using UniRx.Triggers;
using UnityEngine;
using UniRx;

public class CHContGun : MonoBehaviour
{
    [SerializeField] GameObject objBullet;
    [SerializeField] Transform trBulletSpawnPoint;
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
        GameObject bullet = CHMMain.Resource.Instantiate(objBullet, trBulletSpawnPoint);
        bullet.transform.localPosition = Vector3.zero;
        bullet.transform.up = trBulletSpawnPoint.transform.up;

        // �Ѿ��� ���� ������ �������� ���� ���� �߻�
        bullet.GetOrAddComponent<CHContBullet>().Init(transform.forward, bulletForce);

        // ������ �ʱ�ȭ
        timeSinceLastFire = 0f;
    }
}
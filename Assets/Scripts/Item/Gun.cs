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
        // �÷��̾ �߻� ��ư�� �������� Ȯ��
        if (Input.GetButtonDown("Fire1"))
        {
            // ���� �������� �� �Ѿ� ����
            GameObject bullet = CHMMain.Resource.Instantiate(bulletPrefab, bulletSpawnPoint);
            bullet.transform.localPosition = Vector3.zero;
            bullet.transform.up = bulletSpawnPoint.transform.up;

            // �Ѿ��� ���� ������ �������� ���� ���� �߻�
            bullet.GetComponent<Rigidbody>().AddForce(bulletSpawnPoint.up * bulletForce, ForceMode.Impulse);

            await Task.Delay(2000);

            CHMMain.Resource.Destroy(bullet);
        }
    }
}

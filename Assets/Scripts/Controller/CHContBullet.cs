using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CHContBullet : MonoBehaviour
{
    Rigidbody rbHit;
    Vector3 v3Direct;
    float bulletSpeed;

    public void Init(Vector3 _direction, float _bulletSpeed)
    {
        v3Direct = _direction;
        bulletSpeed = _bulletSpeed;
        rbHit = gameObject.GetOrAddComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        rbHit.MovePosition(rbHit.position + v3Direct * bulletSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}

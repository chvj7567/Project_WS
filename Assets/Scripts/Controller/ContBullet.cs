using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ContBullet : MonoBehaviour
{
    Rigidbody hitRb;
    Vector3 direction;
    float bulletSpeed;

    public void Init(Vector3 _direction, float _bulletSpeed)
    {
        direction = _direction;
        bulletSpeed = _bulletSpeed;
        hitRb = gameObject.GetOrAddComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        hitRb.MovePosition(hitRb.position + direction * bulletSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var particle = CHMMain.Particle.GetRandomParticleObject();
        particle.transform.position = collision.contacts.First().point;
        CHMMain.Resource.Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContBullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        CHMMain.Resource.InstantiateEffect(Defines.EEffect.FX_Fire, (effect) =>
        {
            effect.transform.position = collision.transform.position;
        });

        CHMMain.Resource.Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContBullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        CHMMain.Resource.Destroy(gameObject);
    }
}

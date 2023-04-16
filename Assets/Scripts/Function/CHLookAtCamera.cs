using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHLookAtCamera : MonoBehaviour
{
    [SerializeField] Camera camera;

    private void Start()
    {
        if (camera != null)
        {
            transform.forward = -(camera.transform.position - transform.position);
        }
    }
}

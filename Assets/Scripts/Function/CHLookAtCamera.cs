using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHLookAtCamera : MonoBehaviour
{
    [SerializeField] Camera uiCamera;

    private void Start()
    {
        if (uiCamera != null)
        {
            transform.forward = -(uiCamera.transform.position - transform.position);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    BoxCollider weaponColider;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        weaponColider = GetComponent<BoxCollider>();

        if (weaponColider == null)
        {
            weaponColider = this.AddComponent<BoxCollider>();
        }

        weaponColider.isTrigger = true;
    }
}

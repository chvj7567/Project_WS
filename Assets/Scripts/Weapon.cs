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
        weaponColider.isTrigger = true;
    }

    void Shot()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag == Defines.ETag.Player.ToString())
        {
            var controller = other.GetComponent<PlayerController>();

            if (controller)
            {
                var leftHand = controller.GetLeftHand();
                if (leftHand.handState == UnitHand.EHandState.None)
                {
                    transform.SetParent(leftHand.transform);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    leftHand.UpdateState();
                    return;
                }
                var rightHand = controller.GetRightHand();
                if (rightHand.handState == UnitHand.EHandState.None)
                {
                    transform.SetParent(rightHand.transform);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    rightHand.UpdateState();
                    return;
                }
            }
        }
    }
}

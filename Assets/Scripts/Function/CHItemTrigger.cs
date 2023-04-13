using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CHItemTrigger : MonoBehaviour
{
    BoxCollider colItem;

    void Start()
    {
        Init();
    }

    void Init()
    {
        colItem = GetComponent<BoxCollider>();
        colItem.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag == Defines.ETag.Player.ToString())
        {
            var controller = other.GetComponent<ContPlayer>();

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

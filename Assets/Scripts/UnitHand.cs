using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHand : MonoBehaviour
{
    public enum EHandState
    {
        None,
        Weapon
    }

    public EHandState handState { get; private set; }

    public void Init()
    {
        int weaponCount = transform.childCount;

        // ��� �ִ� ���� ���� Ȯ���Ͽ� 2�� �̻��� �� 1���� ������ ��
        if (weaponCount == 0)
        {
            handState = EHandState.None;
        }
        else if (weaponCount == 1)
        {
            handState = EHandState.Weapon;
        }
        else
        {
            handState = EHandState.Weapon;

            for (int i = weaponCount - 1; i > 0; --i)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}

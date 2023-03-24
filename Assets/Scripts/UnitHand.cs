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

        // 들고 있는 무기 수를 확인하여 2개 이상일 시 1개만 남도록 함
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

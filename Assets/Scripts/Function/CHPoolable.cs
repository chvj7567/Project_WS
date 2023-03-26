using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHPoolable : MonoBehaviour
{
    [SerializeField] bool isUse = true;

    public bool GetIsUse()
    {
        return isUse;
    }

    public void IsUse()
    {
        isUse = true;
    }

    public void IsNotUse()
    {
        isUse = false;
    }
}

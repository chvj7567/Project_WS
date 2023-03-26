using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
}

public static class CHExtention
{
    public static bool IsNullOrEmpty<T>(this List<T> list)
    {
        if (list == null)
        {
            return true;
        }

        if (list.Count <= 0)
        {
            return true;
        }

        return false;
    }
}

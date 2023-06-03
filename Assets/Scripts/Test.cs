using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);
        CHMMain.UI.ShowUI(Defines.EUI.UIMenu, new CHUIArg());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseScene : SceneBase
{
    private void Start()
    {
        CHMMain.UI.CreateEventSystemObject();
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);
    }
}

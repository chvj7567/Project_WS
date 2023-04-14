using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitA : UnitBase
{
    protected override void Init()
    {
        unitInfoCurrent = unitInfoOrigin = CHMMain.Json.GetUnitInfo(Defines.EUnitID.A);


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlayer : UnitBase
{
    protected override void Init()
    {
        unitInfoCurrent = unitInfoOrigin = CHMMain.Json.GetUnitInfo(Defines.EUnitID.B);
    }
}

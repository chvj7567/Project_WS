using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlayer : UnitBase
{
    protected override void Init()
    {
        unitInfo = unitInfo= CHMMain.Json.GetUnitInfo(Defines.EUnitID.B);

        skill1Info = CHMMain.Json.GetSkillInfo(unitInfo.eSkill1ID);
        skill2Info = CHMMain.Json.GetSkillInfo(unitInfo.eSkill2ID);
        skill3Info = CHMMain.Json.GetSkillInfo(unitInfo.eSkill3ID);
        skill4Info = CHMMain.Json.GetSkillInfo(unitInfo.eSkill4ID);
    }
}

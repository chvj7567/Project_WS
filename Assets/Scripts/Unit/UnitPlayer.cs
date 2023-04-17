using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlayer : UnitBase
{
    protected override void Init()
    {
        orgUnitInfo = curUnitInfo = CHMMain.Json.GetUnitInfo(Defines.EUnitID.B);

        orgSkill1Info = curSkill1Info = CHMMain.Json.GetSkillInfo(orgUnitInfo.eSkill1ID);
        orgSkill2Info = curSkill2Info = CHMMain.Json.GetSkillInfo(orgUnitInfo.eSkill2ID);
        orgSkill3Info = curSkill3Info = CHMMain.Json.GetSkillInfo(orgUnitInfo.eSkill3ID);
        orgSkill4Info = curSkill4Info = CHMMain.Json.GetSkillInfo(orgUnitInfo.eSkill4ID);
    }
}

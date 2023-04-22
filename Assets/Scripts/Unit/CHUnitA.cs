using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHUnitA : CHUnitBase
{
    protected override void Init()
    {
        orgUnitInfo = CHMMain.Json.GetUnitInfo(Defines.EUnitID.A);

        curUnitInfo = orgUnitInfo.Copy();

        orgSkill1Info = CHMMain.Json.GetSkillInfo(orgUnitInfo.eSkill1ID);
        orgSkill2Info = CHMMain.Json.GetSkillInfo(orgUnitInfo.eSkill2ID);
        orgSkill3Info = CHMMain.Json.GetSkillInfo(orgUnitInfo.eSkill3ID);
        orgSkill4Info = CHMMain.Json.GetSkillInfo(orgUnitInfo.eSkill4ID);

        curSkill1Info = orgSkill1Info.Clone();
        curSkill1Info = orgSkill2Info.Clone();
        curSkill1Info = orgSkill3Info.Clone();
        curSkill1Info = orgSkill4Info.Clone();
    }
}

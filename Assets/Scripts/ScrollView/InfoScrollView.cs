using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoScrollView : CHPoolingScrollView<InfoScrollViewItem, SkillData>
{
    [SerializeField] UIInfo uiInfo;

    public override void InitItem(InfoScrollViewItem obj, SkillData info, int index)
    {
        obj.Init(index, info, uiInfo.GetUnitSprite(info.eSkill));
    }
}

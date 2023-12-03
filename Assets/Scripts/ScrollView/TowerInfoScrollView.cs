using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerInfoScrollView : CHPoolingScrollView<TowerInfoScrollViewItem, SkillData>
{
    [SerializeField] UITowerInfo uiInfo;

    public override void InitItem(TowerInfoScrollViewItem obj, SkillData info, int index)
    {
        obj.Init(index, info, uiInfo.GetUnitSprite(info.eSkill));
    }
}

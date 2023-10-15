using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UIInfoArg : CHUIArg
{
    public float closeTime = .5f;
    public int stringID;
}

public class UIInfo : UIBase
{
    UIInfoArg arg;

    [SerializeField] InfoScrollView scrollView;
    [SerializeField] List<Sprite> spriteList = new List<Sprite>();
    [SerializeField, ReadOnly] Dictionary<Defines.ESkill, Defines.EUnit> skillUnitDic = new Dictionary<Defines.ESkill, Defines.EUnit>();

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UIInfoArg;
    }

    private async void Start()
    {
        var unitDataList = CHMMain.Unit.GetUnitDataAll();
        if (unitDataList != null)
        {
            List<SkillData> skillDataList = new List<SkillData>();

            for (int i = 0; i < spriteList.Count; ++i)
            {
                int index = i;
                var skillData = CHMMain.Skill.GetSkillData(unitDataList[index].eSkill1);
                if (skillData != null)
                {
                    skillUnitDic.Add(skillData.eSkill, unitDataList[index].eUnit);
                    skillDataList.Add(skillData);
                }
            }

            scrollView.SetItemList(skillDataList);
        }
    }

    public Sprite GetUnitSprite(Defines.ESkill skill)
    {
        if (skillUnitDic.TryGetValue(skill, out Defines.EUnit result) == false)
        {
            return null;
        }

        return spriteList[(int)result];
    }
}

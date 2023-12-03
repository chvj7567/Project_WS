using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UITowerInfoArg : CHUIArg
{
    public float closeTime = .5f;
    public int stringID;
}

public class UITowerInfo : UIBase
{
    UITowerInfoArg arg;

    [SerializeField] TowerInfoScrollView scrollView;
    [SerializeField] List<Sprite> spriteList = new List<Sprite>();
    [SerializeField, ReadOnly] Dictionary<Defines.ESkill, Defines.EUnit> skillUnitDic = new Dictionary<Defines.ESkill, Defines.EUnit>();

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UITowerInfoArg;
    }

    private async void Start()
    {
        backAction += () =>
        {
            SetUnitData(Defines.EUnit.White, Defines.EItem.White);
            SetUnitData(Defines.EUnit.Brown, Defines.EItem.Brown);
            SetUnitData(Defines.EUnit.Orange, Defines.EItem.Orange);
            SetUnitData(Defines.EUnit.Yellow, Defines.EItem.Yellow);
            SetUnitData(Defines.EUnit.Green, Defines.EItem.Green);
            SetUnitData(Defines.EUnit.Blue, Defines.EItem.Blue);
            SetUnitData(Defines.EUnit.Pink, Defines.EItem.Pink);
            SetUnitData(Defines.EUnit.Red, Defines.EItem.Red);
        };

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

    void SetUnitData(Defines.EUnit eUnit, Defines.EItem eItem)
    {
        var unitData = CHMMain.Unit.GetUnitData(eUnit);
        var itemData = CHMMain.Item.GetItemData(eItem);
        var addData = CHMData.Instance.GetUnitData(eUnit);

        itemData.damage = addData.plusDamage;
        itemData.coolTime = addData.minusCoolTime;

        unitData.eItem1 = eItem;
    }
}

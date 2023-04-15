using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField, ReadOnly] protected Infomation.UnitInfo unitInfo;

    [SerializeField, ReadOnly] protected Infomation.SkillInfo skill1Info;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo skill2Info;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo skill3Info;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo skill4Info;

    private void Awake()
    {
        Init();
    }

    protected abstract void Init();

    #region UnitInfoGetter
    public Defines.EUnitID GetOriginUnitID() { return unitInfo.eUnitID; }
    public int GetOriginNameStringID() { return unitInfo.nameStringID; }
    public float GetOriginHp() { return unitInfo.hp; }
    public float GetOriginMp() { return unitInfo.mp; }
    public float GetOriginAttackDelay() { return unitInfo.attackDelay; }
    public float GetOriginAttackDistance() { return unitInfo.attackDistance; }
    public float GetOriginMoveSpeed() { return unitInfo.moveSpeed; }
    public Defines.ESkillID GetOriginSkill1() { return skill1Info.eSkillID; }
    public Defines.ESkillID GetOriginSkill2() { return skill2Info.eSkillID; }
    public Defines.ESkillID GetOriginSkill3() { return skill3Info.eSkillID; }
    public Defines.ESkillID GetOriginSkill4() { return skill4Info.eSkillID; }
    public float GetOriginSkill1CoolTime() { return skill1Info.coolTime; }
    public float GetOriginSkill2CoolTime() { return skill2Info.coolTime; }
    public float GetOriginSkill3CoolTime() { return skill3Info.coolTime; }
    public float GetOriginSkill4CoolTime() { return skill4Info.coolTime; }
    public float GetOriginSkill1Distance() { return skill1Info.distance; }
    public float GetOriginSkill2Distance() { return skill2Info.distance; }
    public float GetOriginSkill3Distance() { return skill3Info.distance; }
    public float GetOriginSkill4Distance() { return skill4Info.distance; }
    #endregion

    public void PlusHp(float _value)
    {
        if (unitInfo == null) Init();

        Debug.Log($"Hp : {unitInfo.hp} -> Hp : {unitInfo.hp + _value}");
        unitInfo.hp += _value;
    }

    public void MinusHp(float _value)
    {
        if (unitInfo == null) Init();

        Debug.Log($"Hp : {unitInfo.hp} -> Hp : {unitInfo.hp - _value}");
        unitInfo.hp -= _value;
    }

    public void PlusMp(float _value)
    {
        if (unitInfo == null) Init();

        Debug.Log($"Mp : {unitInfo.mp} -> Mp : {unitInfo.mp + _value}");
        unitInfo.mp += _value;
    }

    public void MinusMp(float _value)
    {
        if (unitInfo == null) Init();

        Debug.Log($"Mp : {unitInfo.mp} -> Mp : {unitInfo.mp - _value}");
        unitInfo.mp -= _value;
    }
}

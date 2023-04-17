using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    // 기본 유닛 정보
    [SerializeField, ReadOnly] protected Infomation.UnitInfo orgUnitInfo;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo orgSkill1Info;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo orgSkill2Info;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo orgSkill3Info;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo orgSkill4Info;

    // 현재 유닛 정보
    [SerializeField, ReadOnly] protected Infomation.UnitInfo curUnitInfo;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo curSkill1Info;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo curSkill2Info;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo curSkill3Info;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo curSkill4Info;

    private void Awake()
    {
        Init();
    }

    protected abstract void Init();

    #region OriginUnitInfoGetter
    public Defines.EUnitID GetOriginUnitID() { return orgUnitInfo.eUnitID; }
    public int GetOriginNameStringID() { return orgUnitInfo.nameStringID; }
    public float GetOriginMaxHp() { return orgUnitInfo.maxHp; }
    public float GetOriginHp() { return orgUnitInfo.hp; }
    public float GetOriginMaxMp() { return orgUnitInfo.maxMp; }
    public float GetOriginMp() { return orgUnitInfo.mp; }
    public float GetOriginAttackDelay() { return orgUnitInfo.attackDelay; }
    public float GetOriginAttackDistance() { return orgUnitInfo.attackDistance; }
    public float GetOriginMoveSpeed() { return orgUnitInfo.moveSpeed; }
    public Defines.ESkillID GetOriginSkill1() { return orgSkill1Info.eSkillID; }
    public Defines.ESkillID GetOriginSkill2() { return orgSkill2Info.eSkillID; }
    public Defines.ESkillID GetOriginSkill3() { return orgSkill3Info.eSkillID; }
    public Defines.ESkillID GetOriginSkill4() { return orgSkill4Info.eSkillID; }
    public float GetOriginSkill1CoolTime() { return orgSkill1Info.coolTime; }
    public float GetOriginSkill2CoolTime() { return orgSkill2Info.coolTime; }
    public float GetOriginSkill3CoolTime() { return orgSkill3Info.coolTime; }
    public float GetOriginSkill4CoolTime() { return orgSkill4Info.coolTime; }
    public float GetOriginSkill1Distance() { return orgSkill1Info.distance; }
    public float GetOriginSkill2Distance() { return orgSkill2Info.distance; }
    public float GetOriginSkill3Distance() { return orgSkill3Info.distance; }
    public float GetOriginSkill4Distance() { return orgSkill4Info.distance; }
    #endregion

    #region CurrentUnitInfoGetter
    public Defines.EUnitID GetCurrentUnitID() { return curUnitInfo.eUnitID; }
    public int GetCurrentNameStringID() { return curUnitInfo.nameStringID; }
    public float GetCurrentMaxHp() { return curUnitInfo.maxHp; }
    public float GetCurrentHp() { return curUnitInfo.hp; }
    public float GetCurrentMaxMp() { return curUnitInfo.maxMp; }
    public float GetCurrentMp() { return curUnitInfo.mp; }
    public float GetCurrentAttackDelay() { return curUnitInfo.attackDelay; }
    public float GetCurrentAttackDistance() { return curUnitInfo.attackDistance; }
    public float GetCurrentMoveSpeed() { return curUnitInfo.moveSpeed; }
    public Defines.ESkillID GetCurrentSkill1() { return curSkill1Info.eSkillID; }
    public Defines.ESkillID GetCurrentSkill2() { return curSkill2Info.eSkillID; }
    public Defines.ESkillID GetCurrentSkill3() { return curSkill3Info.eSkillID; }
    public Defines.ESkillID GetCurrentSkill4() { return curSkill4Info.eSkillID; }
    public float GetCurrentSkill1CoolTime() { return curSkill1Info.coolTime; }
    public float GetCurrentSkill2CoolTime() { return curSkill2Info.coolTime; }
    public float GetCurrentSkill3CoolTime() { return curSkill3Info.coolTime; }
    public float GetCurrentSkill4CoolTime() { return curSkill4Info.coolTime; }
    public float GetCurrentSkill1Distance() { return curSkill1Info.distance; }
    public float GetCurrentSkill2Distance() { return curSkill2Info.distance; }
    public float GetCurrentSkill3Distance() { return curSkill3Info.distance; }
    public float GetCurrentSkill4Distance() { return curSkill4Info.distance; }
    #endregion

    public void PlusHp(float _value)
    {
        if (orgUnitInfo == null) Init();

        Debug.Log($"Hp : {curUnitInfo.hp} -> Hp : {curUnitInfo.hp + _value}");
        curUnitInfo.hp += _value;
    }

    public void MinusHp(float _value)
    {
        if (orgUnitInfo == null) Init();

        Debug.Log($"Hp : {curUnitInfo.hp} -> Hp : {curUnitInfo.hp - _value}");
        curUnitInfo.hp -= _value;
    }

    public void PlusMp(float _value)
    {
        if (orgUnitInfo == null) Init();

        Debug.Log($"Mp : {curUnitInfo.mp} -> Mp : {curUnitInfo.mp + _value}");
        curUnitInfo.mp += _value;
    }

    public void MinusMp(float _value)
    {
        if (orgUnitInfo == null) Init();

        Debug.Log($"Mp : {curUnitInfo.mp} -> Mp : {curUnitInfo.mp - _value}");
        curUnitInfo.mp -= _value;
    }
}

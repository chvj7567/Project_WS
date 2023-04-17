using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using static Defines;

public abstract class UnitBase : MonoBehaviour
{
    // �⺻ ���� ����
    [SerializeField, ReadOnly] protected Infomation.UnitInfo orgUnitInfo;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo orgSkill1Info;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo orgSkill2Info;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo orgSkill3Info;
    [SerializeField, ReadOnly] protected Infomation.SkillInfo orgSkill4Info;

    // ���� ���� ����
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
    public float GetOriginAttackPower() { return orgUnitInfo.attackPower; }
    public float GetOriginDefensePower() { return orgUnitInfo.defensePower; }
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
    public float GetCurrentAttackPower() { return curUnitInfo.attackPower; }
    public float GetCurrentDefensePower() { return curUnitInfo.defensePower; }
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

    public void ChangeHp(float _value, EDamageState eDamageState)
    {
        switch (eDamageState)
        {
            case EDamageState.AtOnce:
                AtOnceChangeHp(_value);
                break;
            case EDamageState.Continuous_1Sec_3Count:
                ContinuousChangeHp(1f, 3f, _value);
                break;
            default:
                AtOnceChangeHp(_value);
                break;
        }
    }

    public void ChangeMp(float _value, EDamageState eDamageState)
    {
        switch (eDamageState)
        {
            case EDamageState.AtOnce:
                AtOnceChangeMp(_value);
                break;
            case EDamageState.Continuous_1Sec_3Count:
                ContinuousChangeMp(1f, 3f, _value);
                break;
            default:
                AtOnceChangeMp(_value);
                break;
        }
    }

    public void ChangeAttackPower(float _value, EDamageState eDamageState)
    {
        if (orgUnitInfo == null) Init();

        switch (eDamageState)
        {
            case EDamageState.AtOnce:
                AtOnceChangeAttackPower(_value);
                break;
            case EDamageState.Continuous_1Sec_3Count:
                ContinuousChangeAttackPower(1f, 3f, _value);
                break;
            default:
                break;
        }
    }

    public void ChangeDefensePower(float _value, EDamageState eDamageState)
    {
        if (orgUnitInfo == null) Init();

        switch (eDamageState)
        {
            case EDamageState.AtOnce:
                AtOnceChangeDefensePower(_value);
                break;
            case EDamageState.Continuous_1Sec_3Count:
                ContinuousChangeDefensePower(1f, 3f, _value);
                break;
            default:
                break;
        }
    }

    void AtOnceChangeHp(float _value)
    {
        float hpResult = curUnitInfo.hp + _value;
        if (hpResult >= GetCurrentMaxHp())
        {
            hpResult = GetCurrentMaxHp();
        }

        Debug.Log($"{curUnitInfo.nameStringID} => Hp : {curUnitInfo.hp} -> Hp : {hpResult}");
        curUnitInfo.hp += _value;
    }

    void AtOnceChangeMp(float _value)
    {
        float mpResult = curUnitInfo.mp + _value;
        if (mpResult >= GetCurrentMaxMp())
        {
            mpResult = GetCurrentMaxMp();
        }

        Debug.Log($"{curUnitInfo.nameStringID} => Mp : {curUnitInfo.mp} -> Mp : {mpResult}");
        curUnitInfo.mp += _value;
    }

    void AtOnceChangeAttackPower(float _value)
    {
        float attackPowerResult = curUnitInfo.attackPower + _value;
        CheckMaxStatValue(EStat.AttackPower, ref attackPowerResult);

        Debug.Log($"{curUnitInfo.nameStringID} => AttackPower : {curUnitInfo.attackPower} -> AttackPower : {attackPowerResult}");
        curUnitInfo.attackPower += _value;
    }

    void AtOnceChangeDefensePower(float _value)
    {
        float defensePowerResult = curUnitInfo.defensePower + _value;
        CheckMaxStatValue(EStat.DefensePower, ref defensePowerResult);

        Debug.Log($"{curUnitInfo.nameStringID} => DefensePower : {curUnitInfo.defensePower} -> DefensePower : {defensePowerResult}");
        curUnitInfo.defensePower += _value;
    }

    async void ContinuousChangeHp(float _time, float _count, float _value)
    {
        float tickTime = _time / _count;

        for (int i = 0; i < _count; ++i)
        {
            AtOnceChangeHp(_value);

            if (i == _count - 1)
            {
                break;
            }

            await Task.Delay((int)(tickTime * 1000f));
        }
    }

    async void ContinuousChangeMp(float _time, float _count, float _value)
    {
        float tickTime = _time / _count;

        for (int i = 0; i < _count; ++i)
        {
            AtOnceChangeMp(_value);

            if (i == _count - 1)
            {
                break;
            }

            await Task.Delay((int)(tickTime * 1000f));
        }
    }

    async void ContinuousChangeAttackPower(float _time, float _count, float _value)
    {
        float tickTime = _time / _count;

        for (int i = 0; i < _count; ++i)
        {
            AtOnceChangeAttackPower(_value);

            if (i == _count - 1)
            {
                break;
            }

            await Task.Delay((int)(tickTime * 1000f));
        }
    }

    async void ContinuousChangeDefensePower(float _time, float _count, float _value)
    {
        float tickTime = _time / _count;

        for (int i = 0; i < _count; ++i)
        {
            AtOnceChangeDefensePower(_value);

            if (i == _count - 1)
            {
                break;
            }

            await Task.Delay((int)(tickTime * 1000f));
        }
    }

    public void CheckMaxStatValue(EStat _stat, ref float _value)
    {
        switch (_stat)
        {
            case EStat.Hp:
                if (_value < 0f)
                {
                    _value = 0f;
                }
                else if (_value > 10000f)
                {
                    _value = 10000f;
                }
                break;
            case EStat.Mp:
                if (_value < 0f)
                {
                    _value = 0f;
                }
                else if (_value > 10000f)
                {
                    _value = 10000f;
                }
                break;
            case EStat.AttackPower:
                if (_value < 0f)
                {
                    _value = 0f;
                }
                else if (_value > 10000f)
                {
                    _value = 10000f;
                }
                break;
            case EStat.DefensePower:
                if (_value < 0f)
                {
                    _value = 0f;
                }
                else if (_value > 10000f)
                {
                    _value = 10000f;
                }
                break;
            default:
                break;
        }
    }
}

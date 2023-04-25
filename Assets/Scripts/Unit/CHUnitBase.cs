using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using static Defines;
using static Infomation;

public abstract class CHUnitBase : MonoBehaviour
{
    [SerializeField, ReadOnly] protected Collider unitCollider;

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

    [SerializeField, ReadOnly] protected Defines.EUnitState unitState = Defines.EUnitState.None;

    // 떨어지는 상태인지 확인(에어본과 별도 확인)
    public bool IsFalling { get; set; }

    protected CHGaugeBar hpGaugeBar;

    public void Reset()
    {
        unitState = 0;

        curUnitInfo = orgUnitInfo.Clone();
        curSkill1Info = orgSkill1Info.Clone();
        curSkill1Info = orgSkill2Info.Clone();
        curSkill1Info = orgSkill3Info.Clone();
        curSkill1Info = orgSkill4Info.Clone();

        unitCollider.enabled = true;

        hpGaugeBar.ResetGaugeBar();
    }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        gameObject.UpdateAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
        {
            if (GetIsDeath() == false)
            {
                /*ChangeHp(curUnitInfo.hpRegenPerSecond, Defines.EDamageState.None);
                ChangeMp(curUnitInfo.mpRegenPerSecond, Defines.EDamageState.None);*/
            }
        });

        CHMMain.Resource.InstantiateMajor(EMajor.GaugeBar, (gaugeBar) =>
        {
            if (gaugeBar)
            {
                gaugeBar.transform.SetParent(transform);
                gaugeBar.transform.localPosition = Vector3.zero;

                hpGaugeBar = gaugeBar.GetComponent<CHGaugeBar>();
                if (hpGaugeBar)
                {
                    if (unitCollider == null)
                    {
                        unitCollider = gameObject.GetOrAddComponent<Collider>();
                    }

                    hpGaugeBar.Init(unitCollider.bounds.size.y);
                }
            }
        });
    }

    protected abstract void Init();

    #region OriginUnitInfoGetter
    public Defines.EUnitID GetOriginUnitID() { return orgUnitInfo.eUnitID; }
    public int GetOriginNameStringID() { return orgUnitInfo.nameStringID; }
    public float GetOriginMaxHp() { return orgUnitInfo.maxHp; }
    public float GetOriginHp() { return orgUnitInfo.hp; }
    public float GetOriginHpRegenPerSecond() { return orgUnitInfo.hpRegenPerSecond; }
    public float GetOriginMaxMp() { return orgUnitInfo.maxMp; }
    public float GetOriginMp() { return orgUnitInfo.mp; }
    public float GetOriginMpRegenPerSecond() { return orgUnitInfo.mpRegenPerSecond; }
    public float GetOriginAttackPower() { return orgUnitInfo.attackPower; }
    public float GetOriginDefensePower() { return orgUnitInfo.defensePower; }
    public float GetOriginAttackDelay() { return orgUnitInfo.attackDelay; }
    public float GetOriginAttackDistance() { return orgUnitInfo.attackDistance; }
    public float GetOriginMoveSpeed() { return orgUnitInfo.moveSpeed; }
    public float GetOriginRotateSpeed() { return orgUnitInfo.rotateSpeed; }
    public float GetOriginRange() { return orgUnitInfo.range; }
    public float GetOriginRangeMulti() { return orgUnitInfo.rangeMulti; }
    public float GetOriginViewAngle() { return orgUnitInfo.viewAngle; }
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
    public float GetCurrentHpRegenPerSecond() { return curUnitInfo.hpRegenPerSecond; }
    public float GetCurrentMaxMp() { return curUnitInfo.maxMp; }
    public float GetCurrentMp() { return curUnitInfo.mp; }
    public float GetCurrnetHpRegenPerSecond() { return curUnitInfo.hpRegenPerSecond; }
    public float GetCurrentAttackPower() { return curUnitInfo.attackPower; }
    public float GetCurrentDefensePower() { return curUnitInfo.defensePower; }
    public float GetCurrentAttackDelay() { return curUnitInfo.attackDelay; }
    public float GetCurrentAttackDistance() { return curUnitInfo.attackDistance; }
    public float GetCurrentMoveSpeed() { return curUnitInfo.moveSpeed; }
    public float GetCurrentRotateSpeed() { return curUnitInfo.rotateSpeed; }
    public float GetCurrentRange() { return curUnitInfo.range; }
    public float GetCurrentRangeMulti() { return curUnitInfo.rangeMulti; }
    public float GetCurrentViewAngle() { return curUnitInfo.viewAngle; }
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

    public bool IsNormalState()
    {
        return unitState == 0;
    }

    public bool GetIsDeath()
    {
        return (unitState & Defines.EUnitState.IsDead) != 0;
    }

    public bool GetIsAirborne()
    {
        return (unitState & Defines.EUnitState.IsAirborne) != 0;
    }

    public void SetIsAirborne(bool _airborne)
    {
        if (_airborne)
        {
            unitState |= Defines.EUnitState.IsAirborne;
        }
        else
        {
            unitState &= ~Defines.EUnitState.IsAirborne;
        }
    }

    public void ChangeHp(CHUnitBase _attackUnit, float _value, Defines.EDamageState eDamageState)
    {
        if (GetIsDeath() == false)
        {
            var targetTracker = GetComponent<CHTargetTracker>();
            if (targetTracker != null)
            {
                targetTracker.ExpensionRange();
            }

            switch (eDamageState)
            {
                case Defines.EDamageState.AtOnce:
                    AtOnceChangeHp(_value);
                    break;
                case Defines.EDamageState.Continuous_1Sec_3Count:
                    ContinuousChangeHp(1f, 3, _value);
                    break;
                default:
                    AtOnceChangeHp(_value);
                    break;
            }
        }
    }

    public void ChangeMp(CHUnitBase _attackUnit, float _value, Defines.EDamageState eDamageState)
    {
        if (GetIsDeath() == false)
        {
            switch (eDamageState)
            {
                case Defines.EDamageState.AtOnce:
                    AtOnceChangeMp(_value);
                    break;
                case Defines.EDamageState.Continuous_1Sec_3Count:
                    ContinuousChangeMp(1f, 3, _value);
                    break;
                default:
                    AtOnceChangeMp(_value);
                    break;
            }
        }
    }

    public void ChangeAttackPower(CHUnitBase _attackUnit, float _value, Defines.EDamageState eDamageState)
    {
        if (GetIsDeath() == false)
        {
            switch (eDamageState)
            {
                case Defines.EDamageState.AtOnce:
                    AtOnceChangeAttackPower(_value);
                    break;
                case Defines.EDamageState.Continuous_1Sec_3Count:
                    ContinuousChangeAttackPower(1f, 3, _value);
                    break;
                default:
                    break;
            }
        }
    }

    public void ChangeDefensePower(CHUnitBase _attackUnit, float _value, Defines.EDamageState eDamageState)
    {
        if (GetIsDeath() == false)
        {
            switch (eDamageState)
            {
                case Defines.EDamageState.AtOnce:
                    AtOnceChangeDefensePower(_value);
                    break;
                case Defines.EDamageState.Continuous_1Sec_3Count:
                    ContinuousChangeDefensePower(1f, 3, _value);
                    break;
                default:
                    break;
            }
        }
    }

    void AtOnceChangeHp(float _value)
    {
        float hpOrigin = curUnitInfo.hp;
        float hpResult = curUnitInfo.hp + _value;
        if (hpResult >= GetCurrentMaxHp())
        {
            hpResult = GetCurrentMaxHp();
        }

        curUnitInfo.hp = hpResult;
        if (hpGaugeBar) hpGaugeBar.SetGaugeBar(GetCurrentMaxHp(), hpResult);
        Debug.Log($"{curUnitInfo.nameStringID}<{gameObject.name}> => Hp : {hpOrigin} -> Hp : {hpResult}");

        // 죽음 Die
        if (hpResult <= 0.00001f)
        {
            hpResult = 0f;

            var unitBase = GetComponent<CHContBase>();
            if (unitBase != null)
            {
                var animator = unitBase.GetAnimator();
                if (animator != null)
                {
                    animator.SetBool(unitBase.attackRange, false);
                    animator.SetBool(unitBase.sightRange, false);
                    animator.SetBool(unitBase.isDeath, true);
                }
            }

            unitState |= Defines.EUnitState.IsDead;

            unitCollider.enabled = false;

            CHMMain.Resource.Destroy(gameObject, 3f);
        }
    }

    void AtOnceChangeMp(float _value)
    {
        float mpOrigin = curUnitInfo.mp;
        float mpResult = curUnitInfo.mp + _value;
        if (mpResult >= GetCurrentMaxMp())
        {
            mpResult = GetCurrentMaxMp();
        }
        else if (mpResult < 0)
        {
            mpResult = 0f;
        }

        curUnitInfo.mp = mpResult;
        Debug.Log($"{curUnitInfo.nameStringID} => Mp : {mpOrigin} -> Mp : {mpResult}");
    }

    void AtOnceChangeAttackPower(float _value)
    {
        float attackPowerOrigin = curUnitInfo.attackPower;
        float attackPowerResult = curUnitInfo.attackPower + _value;
        CheckMaxStatValue(Defines.EStat.AttackPower, ref attackPowerResult);

        curUnitInfo.attackPower = attackPowerResult;
        Debug.Log($"{curUnitInfo.nameStringID} => AttackPower : {attackPowerOrigin} -> AttackPower : {attackPowerResult}");
    }

    void AtOnceChangeDefensePower(float _value)
    {
        float defensePowerOrigin = curUnitInfo.defensePower;
        float defensePowerResult = curUnitInfo.defensePower + _value;
        CheckMaxStatValue(Defines.EStat.DefensePower, ref defensePowerResult);

        curUnitInfo.attackPower = defensePowerResult;
        Debug.Log($"{curUnitInfo.nameStringID} => DefensePower : {defensePowerOrigin} -> DefensePower : {defensePowerResult}");
    }

    async void ContinuousChangeHp(float _time, int _count, float _value)
    {
        float tickTime = _time / (_count - 1);

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

    async void ContinuousChangeMp(float _time, int _count, float _value)
    {
        float tickTime = _time / (_count - 1);

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

    async void ContinuousChangeAttackPower(float _time, int _count, float _value)
    {
        float tickTime = _time / (_count - 1);

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

    async void ContinuousChangeDefensePower(float _time, int _count, float _value)
    {
        float tickTime = _time / (_count - 1);

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

    public void CheckMaxStatValue(Defines.EStat _stat, ref float _value)
    {
        switch (_stat)
        {
            case Defines.EStat.Hp:
                if (_value < 0f)
                {
                    _value = 0f;
                }
                else if (_value > 10000f)
                {
                    _value = 10000f;
                }
                break;
            case Defines.EStat.Mp:
                if (_value < 0f)
                {
                    _value = 0f;
                }
                else if (_value > 10000f)
                {
                    _value = 10000f;
                }
                break;
            case Defines.EStat.AttackPower:
                if (_value < 0f)
                {
                    _value = 0f;
                }
                else if (_value > 10000f)
                {
                    _value = 10000f;
                }
                break;
            case Defines.EStat.DefensePower:
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

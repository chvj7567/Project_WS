using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using static Defines;
using static Infomation;

public class CHUnitBase : MonoBehaviour
{
    [SerializeField] EUnit unit;
    [SerializeField] public Collider unitCollider;
    [SerializeField] public MeshRenderer meshRenderer;

    [SerializeField, ReadOnly] protected float hp;
    [SerializeField, ReadOnly] protected float mp;

    [SerializeField, ReadOnly] protected UnitData unitData;
    [SerializeField, ReadOnly] protected SkillData skill1Data;
    [SerializeField, ReadOnly] protected SkillData skill2Data;
    [SerializeField, ReadOnly] protected SkillData skill3Data;
    [SerializeField, ReadOnly] protected SkillData skill4Data;

    [SerializeField, ReadOnly] protected EUnitState unitState = EUnitState.None;

    protected CHGaugeBar hpGaugeBar;

    CancellationTokenSource cts;
    CancellationToken token;

    private void Awake()
    {
        cts = new CancellationTokenSource();
        token = cts.Token;

        if (unitCollider == null) unitCollider = gameObject.GetOrAddComponent<Collider>();
        if (meshRenderer == null) meshRenderer = gameObject.GetOrAddComponent<MeshRenderer>();

        unitData = CHMMain.Unit.GetUnitData(unit);
        if (unitData != null)
        {
            hp = unitData.maxHp;
            mp = unitData.maxMp;

            skill1Data = CHMMain.Skill.GetSkillData(unitData.eSkill1);
            skill2Data = CHMMain.Skill.GetSkillData(unitData.eSkill2);
            skill3Data = CHMMain.Skill.GetSkillData(unitData.eSkill3);
            skill4Data = CHMMain.Skill.GetSkillData(unitData.eSkill4);
        }
    }

    private void Start()
    {
        gameObject.UpdateAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
        {
            if (GetIsDeath() == false)
            {
                /*ChangeHp(this, unitData.hpRegenPerSecond, Defines.EDamageType1.None);
                ChangeMp(this, unitData.mpRegenPerSecond, Defines.EDamageType1.None);*/
            }
        });

        CHMMain.Resource.InstantiateMajor(EMajor.GaugeBar, (Action<GameObject>)((gaugeBar) =>
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

                    // HP 게이지가 스케일에 영향받지 않도록 
                    hpGaugeBar.Init(unitCollider.bounds.size.y / 2f / transform.localScale.x);
                    hpGaugeBar.SetGaugeBar(GetOriginMaxHp(), this.GetCurrentHp(), 0f);
                }
            }
        }));
    }

    private void OnDestroy()
    {
        if (cts != null && !cts.IsCancellationRequested)
        {
            cts.Cancel();
        }
    }

    #region OriginUnitInfoGetter
    public EUnit GetOriginUnitID() { return unitData.eUnit; }
    public int GetOriginNameStringID() { return unitData.nameStringID; }
    public float GetOriginMaxHp() { return unitData.maxHp; }
    public float GetOriginHpRegenPerSecond() { return unitData.hpRegenPerSecond; }
    public float GetOriginMaxMp() { return unitData.maxMp; }
    public float GetOriginMpRegenPerSecond() { return unitData.hpRegenPerSecond; }
    public float GetOriginAttackPower() { return unitData.attackPower; }
    public float GetOriginDefensePower() { return unitData.defensePower; }
    public float GetOriginAttackDelay() { return unitData.attackDelay; }
    public float GetOriginAttackDistance() { return unitData.attackDistance; }
    public float GetOriginMoveSpeed() { return unitData.moveSpeed; }
    public float GetOriginRotateSpeed() { return unitData.rotateSpeed; }
    public float GetOriginRange() { return unitData.range; }
    public float GetOriginRangeMulti() { return unitData.rangeMulti; }
    public float GetOriginViewAngle() { return unitData.viewAngle; }
    public SkillData GetSkill1Data() { return skill1Data; }
    public SkillData GetSkill2Data() { return skill2Data; }
    public SkillData GetSkill3Data() { return skill3Data; }
    public SkillData GetSkill4Data() { return skill4Data; }
    public Defines.ESkill GetOriginSkill1() { return skill1Data.eSkill; }
    public Defines.ESkill GetOriginSkill2() { return skill2Data.eSkill; }
    public Defines.ESkill GetOriginSkill3() { return skill3Data.eSkill; }
    public Defines.ESkill GetOriginSkill4() { return skill4Data.eSkill; }
    public float GetOriginSkill1CoolTime() { return skill1Data.coolTime; }
    public float GetOriginSkill2CoolTime() { return skill2Data.coolTime; }
    public float GetOriginSkill3CoolTime() { return skill3Data.coolTime; }
    public float GetOriginSkill4CoolTime() { return skill4Data.coolTime; }
    public float GetOriginSkill1Distance() { return skill1Data.distance; }
    public float GetOriginSkill2Distance() { return skill2Data.distance; }
    public float GetOriginSkill3Distance() { return skill3Data.distance; }
    public float GetOriginSkill4Distance() { return skill4Data.distance; }
    #endregion

    public float GetCurrentHp()
    {
        return hp;
    }

    public float GetCurrentMp()
    {
        return mp;
    }

    public void ResetUnit()
    {
        unitState = 0;

        hp = unitData.maxHp;
        mp = unitData.maxMp;

        unitCollider.enabled = true;

        hpGaugeBar.ResetGaugeBar();
    }

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

    public void ChangeHp(CHUnitBase _attackUnit, float _value, Defines.EDamageType1 eDamageType1)
    {
        if (GetIsDeath() == false)
        {
            var targetTracker = GetComponent<CHTargetTracker>();
            if (targetTracker != null)
            {
                targetTracker.ExpensionRange();
            }

            switch (eDamageType1)
            {
                case Defines.EDamageType1.AtOnce:
                    AtOnceChangeHp(_value);
                    break;
                case Defines.EDamageType1.Continuous_1Sec_3Count:
                    ContinuousChangeHp(1f, 3, _value);
                    break;
                case Defines.EDamageType1.Continuous_Dot1Sec_10Count:
                    ContinuousChangeHp(.1f, 10, _value);
                    break;
                default:
                    AtOnceChangeHp(_value);
                    break;
            }
        }
    }

    public void ChangeMp(CHUnitBase _attackUnit, float _value, Defines.EDamageType1 eDamageType1)
    {
        if (GetIsDeath() == false)
        {
            switch (eDamageType1)
            {
                case Defines.EDamageType1.AtOnce:
                    AtOnceChangeMp(_value);
                    break;
                case Defines.EDamageType1.Continuous_1Sec_3Count:
                    ContinuousChangeMp(1f, 3, _value);
                    break;
                default:
                    AtOnceChangeMp(_value);
                    break;
            }
        }
    }

    public void ChangeAttackPower(CHUnitBase _attackUnit, float _value, Defines.EDamageType1 eDamageType1)
    {
        if (GetIsDeath() == false)
        {
            switch (eDamageType1)
            {
                case Defines.EDamageType1.AtOnce:
                    AtOnceChangeAttackPower(_value);
                    break;
                case Defines.EDamageType1.Continuous_1Sec_3Count:
                    ContinuousChangeAttackPower(1f, 3, _value);
                    break;
                default:
                    break;
            }
        }
    }

    public void ChangeDefensePower(CHUnitBase _attackUnit, float _value, Defines.EDamageType1 eDamageType1)
    {
        if (GetIsDeath() == false)
        {
            switch (eDamageType1)
            {
                case Defines.EDamageType1.AtOnce:
                    AtOnceChangeDefensePower(_value);
                    break;
                case Defines.EDamageType1.Continuous_1Sec_3Count:
                    ContinuousChangeDefensePower(1f, 3, _value);
                    break;
                default:
                    break;
            }
        }
    }

    void AtOnceChangeHp(float _value)
    {
        float hpOrigin = hp;
        float hpResult = hp + _value;
        if (hpResult >= GetOriginMaxHp())
        {
            hpResult = GetOriginMaxHp();
        }

        hp = hpResult;
        if (hpGaugeBar) hpGaugeBar.SetGaugeBar(GetOriginMaxHp(), hpResult, hpResult - hpOrigin);
        Debug.Log($"{unitData.nameStringID}<{gameObject.name}> => Hp : {hpOrigin} -> Hp : {hpResult}");

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
        float mpOrigin = mp;
        float mpResult = mp + _value;
        if (mpResult >= GetOriginMaxMp())
        {
            mpResult = GetOriginMaxMp();
        }
        else if (mpResult < 0)
        {
            mpResult = 0f;
        }

        mp = mpResult;
        if (unitData) Debug.Log($"{unitData.nameStringID} => Mp : {mpOrigin} -> Mp : {mpResult}");
    }

    void AtOnceChangeAttackPower(float _value)
    {
        float attackPowerOrigin = unitData.attackPower;
        float attackPowerResult = unitData.attackPower + _value;
        CheckMaxStatValue(Defines.EStat.AttackPower, ref attackPowerResult);

        unitData.attackPower = attackPowerResult;
        Debug.Log($"{unitData.nameStringID} => AttackPower : {attackPowerOrigin} -> AttackPower : {attackPowerResult}");
    }

    void AtOnceChangeDefensePower(float _value)
    {
        float defensePowerOrigin = unitData.defensePower;
        float defensePowerResult = unitData.defensePower + _value;
        CheckMaxStatValue(Defines.EStat.DefensePower, ref defensePowerResult);

        unitData.attackPower = defensePowerResult;
        Debug.Log($"{unitData.nameStringID} => DefensePower : {defensePowerOrigin} -> DefensePower : {defensePowerResult}");
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

            if (cts.IsCancellationRequested) return;
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

            if (cts.IsCancellationRequested) return;
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

            if (cts.IsCancellationRequested) return;
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

            if (cts.IsCancellationRequested) return;
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

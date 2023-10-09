using DG.Tweening;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using static Defines;

public class CHUnitBase : MonoBehaviour
{
    [SerializeField] public EUnit unit;
    [SerializeField] public Collider unitCollider;
    [SerializeField] public MeshRenderer meshRenderer;

    [SerializeField] public bool onHpBar = true;
    [SerializeField] public bool onMpBar = false;
    [SerializeField] public bool onCoolTimeBar = false;

    [SerializeField] public UnitData unitData;

    [SerializeField] public LevelData levelData;

    [SerializeField] public SkillData skill1Data;
    [SerializeField] public SkillData skill2Data;
    [SerializeField] public SkillData skill3Data;
    [SerializeField] public SkillData skill4Data;

    [SerializeField] public ItemData item1Data;

    [SerializeField] public EUnitState unitState = EUnitState.None;

    [SerializeField, ReadOnly] public float maxHp;
    [SerializeField, ReadOnly] public float maxMp;
    [SerializeField, ReadOnly] public float curHp;
    [SerializeField, ReadOnly] public float curMp;

    [SerializeField, ReadOnly] public float tempHp;
    [SerializeField, ReadOnly] public float tempMp;
    [SerializeField, ReadOnly] public float tempAttackPower;
    [SerializeField, ReadOnly] public float tempDefensePower;
    [SerializeField, ReadOnly] public float tempMoveSpeed;
    [SerializeField, ReadOnly] public float tempRotateSpeed;

    public CHGaugeBar hpGaugeBar;
    public CHGaugeBar mpGaugeBar;
    public CHGaugeBar coolTimeGaugeBar;

    CancellationTokenSource cts;
    CancellationToken token;

    private void Awake()
    {
        cts = new CancellationTokenSource();
        token = cts.Token;

        if (unitCollider == null) unitCollider = gameObject.GetOrAddComponent<Collider>();
        if (meshRenderer == null) meshRenderer = gameObject.GetOrAddComponent<MeshRenderer>();
    }

    private void Start()
    {
        gameObject.UpdateAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
        {
            if (GetIsDeath() == false)
            {
                if (item1Data == null)
                {
                    ChangeHp(Defines.ESkill.None, this, unitData.hpRegenPerSecond, Defines.EDamageType1.None);
                    ChangeMp(Defines.ESkill.None, this, unitData.mpRegenPerSecond, Defines.EDamageType1.None);
                }
                else
                {
                    ChangeHp(Defines.ESkill.None, this, unitData.hpRegenPerSecond + item1Data.hpRegenPerSecond, Defines.EDamageType1.None);
                    ChangeMp(Defines.ESkill.None, this, unitData.mpRegenPerSecond + item1Data.mpRegenPerSecond, Defines.EDamageType1.None);
                }
            }
        });

        InitUnitData();
        InitGaugeBar(onHpBar, onMpBar, onCoolTimeBar);
    }

    private void OnDestroy()
    {
        if (cts != null && !cts.IsCancellationRequested)
        {
            cts.Cancel();
        }
    }

    #region OriginUnitInfoGetter
    UnitData GetOriginUnitData() { if (unitData == null) InitUnitData(); return unitData; }
    public SkillData GetOriginSkill1Data() { if (unitData == null) InitUnitData(); return skill1Data; }
    public SkillData GetOriginSkill2Data() { if (unitData == null) InitUnitData(); return skill2Data; }
    public SkillData GetOriginSkill3Data() { if (unitData == null) InitUnitData(); return skill3Data; }
    public SkillData GetOriginSkill4Data() { if (unitData == null) InitUnitData(); return skill4Data; }
    public ItemData GetOriginItem1Data() { if (unitData == null) InitUnitData(); return item1Data; }
    #endregion

    #region CurrentUnitInfoGetter
    public float GetCurrentMaxHp()
    {
        float maxHp = 0f;

        maxHp += GetOriginUnitData().maxHp;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            maxHp += item1.maxHp;
        }

        return maxHp;
    }
    public float GetCurrentHpRegenPerSecond()
    {
        float hpRegenPerSecond = 0f;

        hpRegenPerSecond += GetOriginUnitData().hpRegenPerSecond;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            hpRegenPerSecond += item1.hpRegenPerSecond;
        }

        return hpRegenPerSecond;
    }
    public float GetCurrentMaxMp()
    {
        float maxMp = 0f;

        maxMp += GetOriginUnitData().maxMp;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            maxMp += item1.maxMp;
        }

        return maxMp;
    }
    public float GetCurrentMpRegenPerSecond()
    {
        float mpRegenPerSecond = 0f;

        mpRegenPerSecond += GetOriginUnitData().mpRegenPerSecond;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            mpRegenPerSecond += item1.mpRegenPerSecond;
        }

        return mpRegenPerSecond;
    }
    public float GetCurrentAttackPower()
    {
        float attackPower = 0f;

        attackPower += GetOriginUnitData().attackPower;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            attackPower += item1.attackPower;
        }

        return attackPower;
    }
    public float GetCurrentDefensePower()
    {
        float defensePower = 0f;

        defensePower += GetOriginUnitData().defensePower;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            defensePower += item1.defensePower;
        }

        return defensePower;
    }
    public float GetCurrentMoveSpeed()
    {
        float moveSpeed = 0f;

        moveSpeed += GetOriginUnitData().moveSpeed;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            moveSpeed += item1.moveSpeed;
        }

        return moveSpeed;
    }
    public float GetCurrentRotateSpeed()
    {
        float rotateSpeed = 0f;

        rotateSpeed += GetOriginUnitData().rotateSpeed;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            rotateSpeed += item1.rotateSpeed;
        }

        return rotateSpeed;
    }
    public float GetCurrentRange()
    {
        float range = 0f;

        range += GetOriginUnitData().range;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            range += item1.range;
        }

        return range;
    }
    public float GetCurrentRangeMulti()
    {
        float rangeMulti = 0f;

        rangeMulti += GetOriginUnitData().rangeMulti;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            rangeMulti += item1.rangeMulti;
        }

        return rangeMulti;
    }
    public float GetCurrentViewAngle()
    {
        float viewAngle = 0f;

        viewAngle += GetOriginUnitData().viewAngle;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            viewAngle += item1.viewAngle;
        }

        return viewAngle;
    }
    public float GetCurrentSkill1Distance()
    {
        float distance = 0f;

        distance += GetOriginSkill1Data().distance;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            distance += item1.distance;
        }

        return distance;
    }
    public float GetCurrentSkill2Distance()
    {
        float distance = 0f;

        distance += GetOriginSkill2Data().distance;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            distance += item1.distance;
        }

        return distance;
    }
    public float GetCurrentSkill3Distance()
    {
        float distance = 0f;

        distance += GetOriginSkill3Data().distance;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            distance += item1.distance;
        }

        return distance;
    }
    public float GetCurrentSkill4Distance()
    {
        float distance = 0f;

        distance += GetOriginSkill4Data().distance;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            distance += item1.distance;
        }

        return distance;
    }
    public float GetCurrentSkill1CoolTime()
    {
        float coolTime = 0f;

        coolTime += GetOriginSkill1Data().coolTime;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            coolTime += item1.coolTime;
        }

        return coolTime;
    }
    public float GetCurrentSkill2CoolTime()
    {
        float coolTime = 0f;

        coolTime += GetOriginSkill2Data().coolTime;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            coolTime += item1.coolTime;
        }

        return coolTime;
    }
    public float GetCurrentSkill3CoolTime()
    {
        float coolTime = 0f;

        coolTime += GetOriginSkill3Data().coolTime;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            coolTime += item1.coolTime;
        }

        return coolTime;
    }
    public float GetCurrentSkill4CoolTime()
    {
        float coolTime = 0f;

        coolTime += GetOriginSkill4Data().coolTime;

        ItemData item1 = GetOriginItem1Data();
        if (item1 != null)
        {
            coolTime += item1.coolTime;
        }

        return coolTime;
    }
    #endregion



    public float GetCurrentHp()
    {
        return curHp;
    }

    public float GetCurrentMp()
    {
        return curMp;
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

    public void ChangeHp(Defines.ESkill _eSkill, CHUnitBase _attackUnit, float _value, Defines.EDamageType1 eDamageType1)
    {
        if (GetIsDeath() == false)
        {
            var targetTracker = GetComponent<CHTargetTracker>();
            if (targetTracker != null && targetTracker.GetClosestTargetInfo() != null && targetTracker.GetClosestTargetInfo().objTarget != null)
            {
                targetTracker.ExpensionRange();
            }

            switch (eDamageType1)
            {
                case Defines.EDamageType1.AtOnce:
                    AtOnceChangeHp(_eSkill, _attackUnit, _value);
                    break;
                case Defines.EDamageType1.Continuous_1Sec_3Count:
                    ContinuousChangeHp(_eSkill, _attackUnit, 1f, 3, _value);
                    break;
                case Defines.EDamageType1.Continuous_Dot1Sec_10Count:
                    ContinuousChangeHp(_eSkill, _attackUnit, .1f, 10, _value);
                    break;
                default:
                    AtOnceChangeHp(_eSkill, _attackUnit, _value);
                    break;
            }
        }
    }

    public void ChangeMp(Defines.ESkill _eSkill, CHUnitBase _attackUnit, float _value, Defines.EDamageType1 eDamageType1)
    {
        if (GetIsDeath() == false)
        {
            switch (eDamageType1)
            {
                case Defines.EDamageType1.AtOnce:
                    AtOnceChangeMp(_eSkill, _attackUnit, _value);
                    break;
                case Defines.EDamageType1.Continuous_1Sec_3Count:
                    ContinuousChangeMp(_eSkill, _attackUnit, 1f, 3, _value);
                    break;
                default:
                    AtOnceChangeMp(_eSkill, _attackUnit, _value);
                    break;
            }
        }
    }

    public void ChangeAttackPower(Defines.ESkill _eSkill, CHUnitBase _attackUnit, float _value, Defines.EDamageType1 eDamageType1)
    {
        if (GetIsDeath() == false)
        {
            switch (eDamageType1)
            {
                case Defines.EDamageType1.AtOnce:
                    AtOnceChangeAttackPower(_eSkill, _attackUnit, _value);
                    break;
                case Defines.EDamageType1.Continuous_1Sec_3Count:
                    ContinuousChangeAttackPower(_eSkill, _attackUnit, 1f, 3, _value);
                    break;
                default:
                    AtOnceChangeAttackPower(_eSkill, _attackUnit, _value);
                    break;
            }
        }
    }

    public void ChangeDefensePower(Defines.ESkill _eSkill, CHUnitBase _attackUnit, float _value, Defines.EDamageType1 eDamageType1)
    {
        if (GetIsDeath() == false)
        {
            switch (eDamageType1)
            {
                case Defines.EDamageType1.AtOnce:
                    AtOnceChangeDefensePower(_eSkill, _attackUnit, _value);
                    break;
                case Defines.EDamageType1.Continuous_1Sec_3Count:
                    ContinuousChangeDefensePower(_eSkill, _attackUnit, 1f, 3, _value);
                    break;
                default:
                    AtOnceChangeDefensePower(_eSkill, _attackUnit, _value);
                    break;
            }
        }
    }

    public void ChangeSkill1(Defines.ESkill _skill)
    {
        skill1Data = CHMMain.Skill.GetSkillData(_skill);
    }
    public void ChangeSkill2(Defines.ESkill _skill)
    {
        skill2Data = CHMMain.Skill.GetSkillData(_skill);
    }
    public void ChangeSkill3(Defines.ESkill _skill)
    {
        skill3Data = CHMMain.Skill.GetSkillData(_skill);
    }
    public void ChangeSkill4(Defines.ESkill _skill)
    {
        skill4Data = CHMMain.Skill.GetSkillData(_skill);
    }

    public void ChangeItem1(Defines.EItem _item)
    {
        item1Data = CHMMain.Item.GetItemData(_item);
        if (item1Data != null)
        {
            maxHp = unitData.maxHp + item1Data.maxHp;
            maxMp = unitData.maxMp + item1Data.maxMp;

            curHp += item1Data.maxHp;
            curMp += item1Data.maxMp;
        }
    }

    public void ChangeLevel(Defines.ELevel _eLevel)
    {
        var changeLevelData = CHMMain.Level.GetLevelData(unit, _eLevel);
        if (changeLevelData == null)
            return;

        if (levelData != null)
        {
            maxHp += changeLevelData.maxHp - levelData.maxHp;
            maxMp += changeLevelData.maxMp - levelData.maxMp;
            curHp += changeLevelData.maxHp - levelData.maxHp;
            curMp += changeLevelData.maxMp - levelData.maxMp;
        }
        else
        {
            maxHp += changeLevelData.maxHp;
            maxMp += changeLevelData.maxMp;
            curHp += changeLevelData.maxHp;
            curMp += changeLevelData.maxMp;
        }

        levelData = changeLevelData;
    }

    public void InitUnitData()
    {
        unitState = 0;
        maxHp = 0;
        maxMp = 0;
        curHp = 0;
        curMp = 0;
        unitCollider.enabled = true;

        unitData = CHMMain.Unit.GetUnitData(unit);
        if (unitData != null)
        {
            maxHp += unitData.maxHp;
            maxMp += unitData.maxMp;
            curHp += unitData.maxHp;
            curMp += unitData.maxMp;

            CHMMain.Unit.SetColor(gameObject, unit);

            levelData = CHMMain.Level.GetLevelData(unit, unitData.eLevel);

            if (levelData != null)
            {
                maxHp += levelData.maxHp;
                maxMp += levelData.maxMp;
                curHp += levelData.maxHp;
                curMp += levelData.maxMp;
            }

            skill1Data = CHMMain.Skill.GetSkillData(unitData.eSkill1);
            skill2Data = CHMMain.Skill.GetSkillData(unitData.eSkill2);
            skill3Data = CHMMain.Skill.GetSkillData(unitData.eSkill3);
            skill4Data = CHMMain.Skill.GetSkillData(unitData.eSkill4);

            item1Data = CHMMain.Item.GetItemData(unitData.eItem1);

            if (item1Data != null)
            {
                maxHp += item1Data.maxHp;
                maxMp += item1Data.maxMp;
                curHp += item1Data.maxHp;
                curMp += item1Data.maxMp;
            }
        }
    }

    void InitGaugeBar(bool onHpBar, bool onMpBar, bool onCoolTimeBar)
    {
        if (onHpBar && hpGaugeBar == null)
        {
            CHMMain.Resource.InstantiateMajor(EMajor.GaugeBar, ((gaugeBar) =>
            {
                if (gaugeBar)
                {
                    gaugeBar.name = "HpBar";
                    gaugeBar.transform.SetParent(transform);

                    hpGaugeBar = gaugeBar.GetComponent<CHGaugeBar>();
                    if (hpGaugeBar)
                    {
                        if (unitCollider == null)
                        {
                            unitCollider = gameObject.GetOrAddComponent<Collider>();
                        }

                        // HP 게이지가 스케일에 영향받지 않도록 
                        hpGaugeBar.Init(this, unitCollider.bounds.size.y / 2f / transform.localScale.x, 2.3f);
                        hpGaugeBar.SetGaugeBar(1, 1, 0, 1.5f, 1f);
                    }
                }
            }));
        }
        else
        {
            if (hpGaugeBar != null)
                hpGaugeBar.ResetGaugeBar();
        }

        if (onMpBar && mpGaugeBar == null)
        {
            CHMMain.Resource.InstantiateMajor(EMajor.GaugeBar, ((gaugeBar) =>
            {
                if (gaugeBar)
                {
                    gaugeBar.name = "MpBar";
                    gaugeBar.transform.SetParent(transform);

                    mpGaugeBar = gaugeBar.GetComponent<CHGaugeBar>();
                    if (mpGaugeBar)
                    {
                        if (unitCollider == null)
                        {
                            unitCollider = gameObject.GetOrAddComponent<Collider>();
                        }

                        // HP 게이지가 스케일에 영향받지 않도록 
                        mpGaugeBar.Init(this, unitCollider.bounds.size.y / 2f / transform.localScale.x, 1.7f);
                        mpGaugeBar.SetGaugeBar(1, 1, 0f, 1.5f, 1f);
                    }
                }
            }));
        }
        else
        {
            if (mpGaugeBar != null)
                mpGaugeBar.ResetGaugeBar();
        }

        if (onCoolTimeBar && coolTimeGaugeBar == null)
        {
            CHMMain.Resource.InstantiateMajor(EMajor.GaugeBar, ((gaugeBar) =>
            {
                if (gaugeBar)
                {
                    gaugeBar.name = "CoolTimeBar";
                    gaugeBar.transform.SetParent(transform);

                    coolTimeGaugeBar = gaugeBar.GetComponent<CHGaugeBar>();
                    if (coolTimeGaugeBar)
                    {
                        if (unitCollider == null)
                        {
                            unitCollider = gameObject.GetOrAddComponent<Collider>();
                        }

                        // HP 게이지가 스케일에 영향받지 않도록 
                        coolTimeGaugeBar.Init(this, unitCollider.bounds.size.y / 2f / transform.localScale.x, -1.3f);
                        coolTimeGaugeBar.SetGaugeBar(1, 1, 0f, 1.5f, 1f);
                    }
                }
            }));
        }
        else
        {
            if (coolTimeGaugeBar != null)
                coolTimeGaugeBar.ResetGaugeBar();
        }
    }

    void AtOnceChangeHp(Defines.ESkill _eSkill, CHUnitBase _attackUnit, float _value)
    {
        float hpOrigin = curHp;
        float hpResult = curHp + _value;
        if (hpResult >= maxHp)
        {
            hpResult = maxHp;
        }

        curHp = hpResult;

        if (hpGaugeBar)
            hpGaugeBar.SetGaugeBar(maxHp, this.GetCurrentHp(), _value, 1.5f, 1f);

        if (_eSkill != Defines.ESkill.None)
        {
            Debug.Log($"attacker : {_attackUnit.name}, skill : {_eSkill.ToString()}, " +
            $"{unitData.unitName}<{gameObject.name}> => Hp : {hpOrigin} -> {hpResult}");
        }

        // 죽음 Die
        if (hpResult <= 0.00001f)
        {
            hpResult = 0f;

            var contBase = GetComponent<CHContBase>();
            if (contBase != null)
            {
                var animator = contBase.GetAnimator();
                if (animator != null)
                {
                    animator.SetBool(contBase.attackRange, false);
                    animator.SetBool(contBase.sightRange, false);
                    animator.SetTrigger(contBase.isDeath);
                }
            }

            unitState |= Defines.EUnitState.IsDead;

            unitCollider.enabled = false;

            if (hpGaugeBar)
                hpGaugeBar.gameObject.SetActive(false);
            transform.DOMoveY(-10f, 5f);
            CHMMain.Resource.Destroy(gameObject, 5f);
        }
    }

    void AtOnceChangeMp(Defines.ESkill _eSkill, CHUnitBase _attackUnit, float _value)
    {
        float mpOrigin = curMp;
        float mpResult = curMp + _value;
        if (mpResult >= maxMp)
        {
            mpResult = maxMp;
        }
        else if (mpResult < 0)
        {
            mpResult = 0f;
        }

        curMp = mpResult;

        if (mpGaugeBar)
            mpGaugeBar.SetGaugeBar(maxMp, this.GetCurrentMp(), _value, 1.5f, 1f, false);

        if (_eSkill != Defines.ESkill.None)
        {
            Debug.Log($"attacker : {_attackUnit.name}, skill : {_eSkill.ToString()}, " +
            $"{unitData.unitName}<{gameObject.name}> => Mp : {mpOrigin} -> {mpResult}");
        }
    }

    void AtOnceChangeAttackPower(Defines.ESkill _eSkill, CHUnitBase _attackUnit, float _value)
    {
        float attackPowerOrigin = unitData.attackPower;
        float attackPowerResult = unitData.attackPower + _value;
        CheckMaxStatValue(Defines.EStat.AttackPower, ref attackPowerResult);

        unitData.attackPower = attackPowerResult;
        Debug.Log($"attacker : {_attackUnit.name}, skill : {_eSkill.ToString()}, " +
            $"{unitData.unitName}<{gameObject.name}> => AttackPower : {attackPowerOrigin} -> {attackPowerResult}");
    }

    void AtOnceChangeDefensePower(Defines.ESkill _eSkill, CHUnitBase _attackUnit, float _value)
    {
        float defensePowerOrigin = unitData.defensePower;
        float defensePowerResult = unitData.defensePower + _value;
        CheckMaxStatValue(Defines.EStat.DefensePower, ref defensePowerResult);

        unitData.attackPower = defensePowerResult;
        Debug.Log($"attacker : {_attackUnit.name}, skill : {_eSkill.ToString()}, " +
            $"{unitData.unitName}<{gameObject.name}> => DefensePower : {defensePowerOrigin} -> {defensePowerResult}");
    }

    async void ContinuousChangeHp(Defines.ESkill _eSkill, CHUnitBase _attackUnit, float _time, int _count, float _value)
    {
        float tickTime = _time / (_count - 1);

        for (int i = 0; i < _count; ++i)
        {
            AtOnceChangeHp(_eSkill, _attackUnit, _value);

            if (i == _count - 1)
            {
                break;
            }

            await Task.Delay((int)(tickTime * 1000f));

            if (cts.IsCancellationRequested) return;
        }
    }

    async void ContinuousChangeMp(Defines.ESkill _eSkill, CHUnitBase _attackUnit, float _time, int _count, float _value)
    {
        float tickTime = _time / (_count - 1);

        for (int i = 0; i < _count; ++i)
        {
            AtOnceChangeMp(_eSkill, _attackUnit, _value);

            if (i == _count - 1)
            {
                break;
            }

            await Task.Delay((int)(tickTime * 1000f));

            if (cts.IsCancellationRequested) return;
        }
    }

    async void ContinuousChangeAttackPower(Defines.ESkill _eSkill, CHUnitBase _attackUnit, float _time, int _count, float _value)
    {
        float tickTime = _time / (_count - 1);

        for (int i = 0; i < _count; ++i)
        {
            AtOnceChangeAttackPower(_eSkill, _attackUnit, _value);

            if (i == _count - 1)
            {
                break;
            }

            await Task.Delay((int)(tickTime * 1000f));

            if (cts.IsCancellationRequested) return;
        }
    }

    async void ContinuousChangeDefensePower(Defines.ESkill _eSkill, CHUnitBase _attackUnit, float _time, int _count, float _value)
    {
        float tickTime = _time / (_count - 1);

        for (int i = 0; i < _count; ++i)
        {
            AtOnceChangeDefensePower(_eSkill, _attackUnit, _value);

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

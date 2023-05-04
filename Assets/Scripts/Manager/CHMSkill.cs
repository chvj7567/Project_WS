using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Infomation;

public class CHMSkill
{
    public class SkillLocationInfo
    {
        public Transform trCaster;
        public Vector3 posCaster;
        public Vector3 dirCaster;
        public Transform trTarget;
        public Vector3 posTarget;
        public Vector3 dirTarget;
        public Vector3 posSkill;
        public Vector3 dirSkill;

        public SkillLocationInfo Copy()
        {
            SkillLocationInfo copy = new SkillLocationInfo();
            copy.trCaster = this.trCaster;
            copy.posCaster = this.posCaster;
            copy.dirCaster = this.dirCaster;
            copy.trTarget = this.trTarget;
            copy.posTarget = this.posTarget;
            copy.dirTarget = this.dirTarget;
            copy.posSkill = this.posSkill;
            copy.dirSkill = this.dirSkill;
            return copy;
        }
    }

    GameObject roundAreaDecal = null;
    GameObject roundTimingDecal = null;

    Dictionary<Defines.ESkill, SkillData> dicSkillData = new Dictionary<Defines.ESkill, SkillData>();

    public void Init()
    {
        for (int i = 0; i < (int)Defines.ESkill.Max; ++i)
        {
            var skill = (Defines.ESkill)i;

            CHMMain.Resource.LoadSkillData(skill, (_) =>
            {
                if (_ == null) return;

                dicSkillData.Add(skill, _);
            });
        }
    }

    public void Clear()
    {
        roundAreaDecal = null;
        roundTimingDecal = null;

        dicSkillData.Clear();
    }

    public SkillData GetSkillData(Defines.ESkill _skill)
    {
        if (dicSkillData.ContainsKey(_skill) == false) return null;

        return dicSkillData[_skill];
    }

    public async void CreateSkill(SkillLocationInfo _skillLocationInfo, Defines.ESkill _skill)
    {
        // 스킬 시전자가 죽었으면 스킬 발동 X
        var isDeath = _skillLocationInfo.trCaster.GetComponent<CHUnitBase>().GetIsDeath();
        if (isDeath) return;

        var skillData = GetSkillData(_skill);

        if (skillData != null)
        {
            var casterUnit = _skillLocationInfo.trCaster.GetComponent<CHUnitBase>();
            if (casterUnit != null)
            {
                if (CanUseSkill(casterUnit, skillData) == false) return;
            }

            foreach (var effectInfo in skillData.liEffectData)
            {
                // 해당 위치로 움직일지 여부
                if (effectInfo.moveToPos)
                {
                    float distance = Vector3.Distance(_skillLocationInfo.trCaster.position, _skillLocationInfo.posSkill);
                    effectInfo.startDelay = (distance + effectInfo.offsetToTarget) / effectInfo.moveSpeed;
                }
                
                // 스킬 시전 딜레이 시간 전에 데칼로 스킬 시전 구역 알려줌
                if (effectInfo.onDecal && (Mathf.Approximately(0f, effectInfo.startDelay) == false))
                {
                    await CreateDecal(_skillLocationInfo, effectInfo, skillData.isTargeting);
                }
                else
                {
                    if (effectInfo.moveToPos)
                    {
                        float time = 0f;
                        while (time <= effectInfo.startDelay)
                        {
                            _skillLocationInfo.trCaster.position += _skillLocationInfo.dirSkill.normalized * effectInfo.moveSpeed * Time.deltaTime;

                            time += Time.deltaTime;
                            await Task.Delay((int)(Time.deltaTime * 1000f));
                        }
                    }
                    else
                    {
                        await Task.Delay((int)(effectInfo.startDelay * 1000f));
                    }
                }

                if (_skillLocationInfo.trCaster == null) return;

                // 스킬 충돌 범위 생성
                CreateSkillCollision(_skillLocationInfo, effectInfo, skillData.isTargeting);
            }
        }
    }

    public List<TargetInfo> GetTargetInfoListInRange(Vector3 _originPos, Vector3 _direction, LayerMask _lmTarget, float _range, float _viewAngle = 360f)
    {
        List<TargetInfo> targetInfoList = new List<TargetInfo>();

        // 범위내에 있는 타겟들 확인
        Collider[] targets = Physics.OverlapSphere(_originPos, _range, _lmTarget);

        float minDistance = float.MaxValue;

        foreach (Collider target in targets)
        {
            Transform targetTr = target.transform;
            Vector3 targetDir = (targetTr.position - _originPos).normalized;

            // 시야각에 걸리는지 확인
            if (Vector3.Angle(_direction, targetDir) < _viewAngle / 2)
            {
                float targetDis = Vector3.Distance(_originPos, targetTr.position);

                var unitBase = target.GetComponent<CHUnitBase>();
                // 타겟이 살아있으면 타겟으로 지정
                if (unitBase != null && unitBase.GetIsDeath() == false)
                {
                    // 제일 짧은 거리에 있는 타겟을 리스트의 첫번째로
                    if (minDistance > targetDis)
                    {
                        minDistance = targetDis;

                        targetInfoList.Insert(0, new TargetInfo
                        {
                            objTarget = target.gameObject,
                            distance = targetDis,
                        });
                    }
                    else
                    {
                        targetInfoList.Add(new TargetInfo
                        {
                            objTarget = target.gameObject,
                            distance = targetDis,
                        });
                    }
                }
            }
        }

        return targetInfoList;
    }

    public List<Transform> GetTargetTransformList(List<TargetInfo> _liTargetInfo)
    {
        if (_liTargetInfo == null) return null;

        List<Transform> targetTransformList = new List<Transform>();
        foreach (TargetInfo targetInfo in _liTargetInfo)
        {
            targetTransformList.Add(targetInfo.objTarget.transform);
        }

        return targetTransformList;
    }

    public List<Transform> GetTargetTransformList(TargetInfo _targetInfo)
    {
        if (_targetInfo == null) return null;

        List<Transform> targetTransformList = new List<Transform>();
        targetTransformList.Add(_targetInfo.objTarget.transform);

        return targetTransformList;
    }

    public void ApplySkillValue(Transform _trCaster, List<Transform> _liTarget, SkillData.EffectData _effectData)
    {
        // 스킬 효과(데미지 등) 적용

        var casterUnit = _trCaster.GetComponent<CHUnitBase>();

        foreach (var target in _liTarget)
        {
            if (target == null) continue;

            var targetUnit = target.GetComponent<CHUnitBase>();
            if (targetUnit != null)
            {
                ApplyEffectType(casterUnit, targetUnit, _effectData);
            }
        }
    }

    public LayerMask GetTargetMask(int _myLayer, Defines.ETargetMask _targetMask)
    {
        LayerMask myLayerMask = 1 << _myLayer;
        LayerMask enemyLayerMask;
        if (LayerMask.LayerToName(_myLayer) == "Red")
        {
            enemyLayerMask = LayerMask.GetMask("Blue");
        }
        else
        {
            enemyLayerMask = LayerMask.GetMask("Red");
        }

        switch (_targetMask)
        {
            case Defines.ETargetMask.Me:
            case Defines.ETargetMask.MyTeam:
                return myLayerMask;
            case Defines.ETargetMask.Enemy:
                return enemyLayerMask;
            case Defines.ETargetMask.MyTeam_Enemy:
                return myLayerMask | enemyLayerMask;
            default:
                return -1;
        }
    }

    //-------------------------------------- private ------------------------------------------//

    void CreateSkillCollision(SkillLocationInfo _skillLocationInfo, SkillData.EffectData _effectData, bool _isTargeting)
    {
        SkillLocationInfo skillLocationInfo = _skillLocationInfo.Copy();
        LayerMask targetMask = GetTargetMask(skillLocationInfo.trCaster.gameObject.layer, _effectData.eTargetMask);

        List<TargetInfo> liTargetInfo = new List<TargetInfo>();
        List<Transform> liTarget = new List<Transform>();

        // 스킬 시작 시 해당 위치에 콜리젼 생성
        if (_isTargeting == false)
        {
            // 논타겟팅 스킬
            liTargetInfo = GetTargetInfoListInRange(skillLocationInfo.posSkill, skillLocationInfo.dirSkill, targetMask, _effectData.sphereRadius, _effectData.collisionAngle);
            liTarget = GetTargetTransformList(liTargetInfo);

            // 논타겟팅 스킬은 생성 시에 타겟이 없을 수도 있음
            if (liTargetInfo == null || liTargetInfo.Count <= 0)
            {
                if (_effectData.createCasterPosition)
                {
                    // 파티클을 시전자 위치에서 생성
                    skillLocationInfo.posSkill = skillLocationInfo.posCaster;
                    skillLocationInfo.dirSkill = skillLocationInfo.dirCaster;
                }

                if (_effectData.createOnEmpty)
                {
                    CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { skillLocationInfo.trTarget },
                        new List<Vector3> { skillLocationInfo.posSkill }, new List<Vector3> { skillLocationInfo.dirSkill }, _effectData);
                }

                return;
            }
        }
        else
        {
            // 타겟팅 스킬
            liTargetInfo = GetTargetInfoListInRange(skillLocationInfo.trTarget.position, skillLocationInfo.trTarget.forward, targetMask, _effectData.sphereRadius, _effectData.collisionAngle);
            liTarget = GetTargetTransformList(liTargetInfo);

            if (liTargetInfo == null || liTargetInfo.Count <= 0)
            {
                Debug.Log("Targeting Skill : No Target Error");
                return;
            }
        }

        if (_effectData.createCasterPosition)
        {
            // 파티클을 시전자 위치에서 생성
            skillLocationInfo.posSkill = skillLocationInfo.posCaster;
            skillLocationInfo.dirSkill = skillLocationInfo.dirCaster;
        }

        // 파티클 위치에 따라 파티클 생성
        switch (_effectData.eTarget)
        {
            case Defines.ETarget.Position:
                {
                    if (_effectData.duplication)
                    {
                        foreach (var target in liTarget)
                        {
                            CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { skillLocationInfo.trTarget },
                                new List<Vector3> { skillLocationInfo.posSkill }, new List<Vector3> { skillLocationInfo.dirSkill }, _effectData);
                        }
                    }
                    else
                    {
                        CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { skillLocationInfo.trTarget },
                            new List<Vector3> { skillLocationInfo.posSkill }, new List<Vector3> { skillLocationInfo.dirSkill }, _effectData);
                    }
                }
                break;
            case Defines.ETarget.Target_One:
                {
                    Transform targetOne = null;

                    foreach (var target in liTarget)
                    {
                        if (target == _skillLocationInfo.trTarget)
                        {
                            targetOne = target;
                            break;
                        }
                    }
                    
                    if (targetOne == null)
                    {
                        targetOne = liTarget.Last();
                    }

                    Vector3 direction = (targetOne.position - skillLocationInfo.trCaster.position).normalized;

                    // 맞은 타겟 수 만큼 파티클 중복 여부
                    if (_effectData.duplication)
                    {
                        if (_effectData.createCasterPosition == false)
                        {
                            foreach (var target in liTarget)
                            {
                                CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { targetOne },
                                    new List<Vector3> { targetOne.position }, new List<Vector3> { direction }, _effectData);
                            }
                        }
                        else
                        {
                            foreach (var target in liTarget)
                            {
                                CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { skillLocationInfo.trTarget },
                                    new List<Vector3> { skillLocationInfo.posSkill }, new List<Vector3> { direction }, _effectData);
                            }
                        }
                    }
                    else
                    {
                        if (_effectData.createCasterPosition == false)
                        {
                            CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { targetOne },
                                new List<Vector3> { targetOne.position }, new List<Vector3> { direction }, _effectData);
                        }
                        else
                        {
                            CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { skillLocationInfo.trTarget },
                                new List<Vector3> { skillLocationInfo.posSkill }, new List<Vector3> { direction }, _effectData);
                        }
                    }
                }
                break;
            case Defines.ETarget.Target_All:
                {
                    List<Vector3> liParticlePos = new List<Vector3>();
                    List<Vector3> liParticleDir = new List<Vector3>();

                    if (_effectData.createCasterPosition == false)
                    {
                        for (int i = 0; i < liTarget.Count; ++i)
                        {
                            liParticlePos.Add(liTarget[i].position);
                            liParticleDir.Add((liTarget[i].position - skillLocationInfo.trCaster.position).normalized);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < liTarget.Count; ++i)
                        {
                            liParticlePos.Add(skillLocationInfo.trCaster.position);
                            liParticleDir.Add((liTarget[i].position - skillLocationInfo.trCaster.position).normalized);
                        }
                    }

                    CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, liTarget, liParticlePos, liParticleDir, _effectData);
                }
                break;
            default:
                {
                    CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { skillLocationInfo.trTarget },
                        new List<Vector3> { skillLocationInfo.posSkill }, new List<Vector3> { skillLocationInfo.dirSkill }, _effectData);
                }
                break;
        }
    }

    void ApplyEffectType(CHUnitBase _casterUnit, CHUnitBase _targetUnit, SkillData.EffectData _effectData)
    {
        if (_casterUnit == null || _targetUnit == null || _effectData == null) return;

        float skillValue = CalculateSkillDamage(_casterUnit, _targetUnit, _effectData);

        // 스킬 시전자 스탯
        float casterAttackPower = _casterUnit.GetOriginAttackPower();
        float casterDefensePower = _casterUnit.GetOriginDefensePower();
        // 타겟 스탯
        float targetAttackPower = _targetUnit.GetOriginAttackPower();
        float targetDefensePower = _targetUnit.GetOriginDefensePower();

        switch (_effectData.eStatModifyType)
        {
            case Defines.EStatModifyType.Hp_Up:
                Debug.Log($"HpUp : {skillValue}");
                _targetUnit.ChangeHp(_casterUnit, skillValue, _effectData.eDamageType1);
                break;
            case Defines.EStatModifyType.Hp_Down:
                {
                    // 데미지 계산 : 스킬 데미지 + 스킬 시전자 공격력 - 타겟 방어력
                    float totalValue = skillValue + casterAttackPower - targetDefensePower;
                    // 데미지가 -이면 데미지를 줄 수 없다는 뜻
                    if (totalValue < 0)
                    {
                        totalValue = 0f;
                    }
                    Debug.Log($"HpDown : {totalValue}");
                    _targetUnit.ChangeHp(_casterUnit, CHUtil.ReverseValue(totalValue), _effectData.eDamageType1);
                }
                break;
                // 우선 HP만 사용 아래는 사용 시 추가 작업 필요
            /*case Defines.EStatModifyType.Mp_Up:
                _targetUnit.ChangeMp(_casterUnit, skillValue, _effectData.eDamageType1);
                break;
            case Defines.EStatModifyType.Mp_Down:
                _targetUnit.ChangeMp(_casterUnit, CHUtil.ReverseValue(skillValue), _effectData.eDamageType1);
                break;
            case Defines.EStatModifyType.AttackPower_Up:
                _targetUnit.ChangeAttackPower(_casterUnit, skillValue, _effectData.eDamageType1);
                break;
            case Defines.EStatModifyType.AttackPower_Down:
                _targetUnit.ChangeAttackPower(_casterUnit, CHUtil.ReverseValue(skillValue), _effectData.eDamageType1);
                break;
            case Defines.EStatModifyType.DefensePower_Up:
                _targetUnit.ChangeDefensePower(_casterUnit, skillValue, _effectData.eDamageType1);
                break;
            case Defines.EStatModifyType.DefensePower_Down:
                _targetUnit.ChangeDefensePower(_casterUnit, CHUtil.ReverseValue(skillValue), _effectData.eDamageType1);
                break;*/
            default:
                break;
        }
    }

    float CalculateSkillDamage(CHUnitBase _casterUnit, CHUnitBase _targetUnit, SkillData.EffectData _effectData)
    {
        if (_casterUnit == null || _targetUnit == null || _effectData == null) return 0f;

        // 데미지 타입에 따라 구분
        switch (_effectData.eDamageType2)
        {
            case Defines.EDamageType2.Fixed:
                return _effectData.damage;
            case Defines.EDamageType2.Percent_Me_MaxHp:
                return _casterUnit.GetOriginMaxHp() * _effectData.damage / 100f;
            case Defines.EDamageType2.Percent_Me_RemainHp:
                return _casterUnit.GetCurrentHp() * _effectData.damage / 100f;
            case Defines.EDamageType2.Percent_Target_MaxHp:
                return _targetUnit.GetOriginMaxHp() * _effectData.damage / 100f;
            case Defines.EDamageType2.Percent_Target_RemainHp:
                return _targetUnit.GetCurrentHp() * _effectData.damage / 100f;
            default:
                return 0f;
        }
    }

    bool CanUseSkill(CHUnitBase _casterUnit, SkillData skillInfo)
    {
        switch (skillInfo.eSkillCost)
        {
            case Defines.ESkillCost.Fixed_HP:
                {
                    if (_casterUnit.GetCurrentHp() >= skillInfo.cost)
                    {
                        _casterUnit.ChangeHp(_casterUnit, CHUtil.ReverseValue(skillInfo.cost), Defines.EDamageType1.None);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case Defines.ESkillCost.Percent_MaxHP:
                {
                    var costValue = _casterUnit.GetOriginMaxHp() * skillInfo.cost / 100f;

                    if (_casterUnit.GetCurrentHp() >= costValue)
                    {
                        _casterUnit.ChangeHp(_casterUnit, CHUtil.ReverseValue(costValue), Defines.EDamageType1.None);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case Defines.ESkillCost.Percent_RemainHP:
                {
                    var costValue = _casterUnit.GetCurrentHp() * skillInfo.cost / 100f;

                    if (_casterUnit.GetCurrentHp() >= costValue)
                    {
                        _casterUnit.ChangeHp(_casterUnit, CHUtil.ReverseValue(costValue), Defines.EDamageType1.None);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case Defines.ESkillCost.Fixed_MP:
                {
                    if (_casterUnit.GetCurrentMp() >= skillInfo.cost)
                    {
                        _casterUnit.ChangeMp(_casterUnit, CHUtil.ReverseValue(skillInfo.cost), Defines.EDamageType1.None);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case Defines.ESkillCost.Percent_MaxMP:
                {
                    var costValue = _casterUnit.GetOriginMaxMp() * skillInfo.cost / 100f;

                    if (_casterUnit.GetCurrentMp() >= costValue)
                    {
                        _casterUnit.ChangeMp(_casterUnit, CHUtil.ReverseValue(costValue), Defines.EDamageType1.None);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case Defines.ESkillCost.Percent_RemainMP:
                {
                    var costValue = _casterUnit.GetCurrentMp() * skillInfo.cost / 100f;

                    if (_casterUnit.GetCurrentMp() >= costValue)
                    {
                        _casterUnit.ChangeMp(_casterUnit, CHUtil.ReverseValue(costValue), Defines.EDamageType1.None);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            default:
                return false;
        }
    }

    async Task CreateDecal(SkillLocationInfo _skillLocationInfo, SkillData.EffectData _effectData, bool _isTargeting)
    {
        GameObject objDecal = null;

        switch (_effectData.eCollision)
        {
            case Defines.ECollision.Sphere:
                {
                    if (roundAreaDecal == null)
                    {
                        CHMMain.Resource.InstantiateDecal(Defines.EDecal.RoundArea, (decal) =>
                        {
                            roundAreaDecal = decal;
                            roundAreaDecal.SetActive(false);
                            roundAreaDecal.GetOrAddComponent<CHPoolable>();

                            objDecal = CHMMain.Resource.Instantiate(roundAreaDecal);
                        });
                    }
                    else
                    {
                        objDecal = CHMMain.Resource.Instantiate(roundAreaDecal);
                    }

                    if (_isTargeting)
                    {
                        objDecal.transform.SetParent(_skillLocationInfo.trTarget.transform);
                        objDecal.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        objDecal.transform.position = _skillLocationInfo.posSkill;
                        objDecal.transform.forward = _skillLocationInfo.dirSkill;
                    }

                    objDecal.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                    var decalProjector = objDecal.GetComponent<DecalProjector>();
                    if (decalProjector != null)
                    {
                        decalProjector.size = Vector3.one * _effectData.sphereRadius * 2;
                    }
                }
                break;
            case Defines.ECollision.Box:
                break;
        }

        await CreateTimeDecal(_skillLocationInfo, objDecal, _effectData, _isTargeting);
    }

    async Task CreateTimeDecal(SkillLocationInfo _skillLocationInfo, GameObject _areaDecal, SkillData.EffectData _effectData, bool _isTargeting)
    {
        GameObject objDecal = null;

        switch (_effectData.eCollision)
        {
            case Defines.ECollision.Sphere:
                {
                    if (roundTimingDecal == null)
                    {
                        CHMMain.Resource.InstantiateDecal(Defines.EDecal.RoundTiming, (decal) =>
                        {
                            roundTimingDecal = decal;
                            roundTimingDecal.SetActive(false);
                            roundTimingDecal.GetOrAddComponent<CHPoolable>();

                            objDecal = CHMMain.Resource.Instantiate(roundTimingDecal);
                        });
                    }
                    else
                    {
                        objDecal = CHMMain.Resource.Instantiate(roundTimingDecal);
                    }

                    if (_isTargeting)
                    {
                        objDecal.transform.SetParent(_skillLocationInfo.trTarget.transform);
                        objDecal.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        objDecal.transform.position = _skillLocationInfo.posSkill;
                        objDecal.transform.forward = _skillLocationInfo.dirSkill;
                    }

                    objDecal.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                    var decalProjector = objDecal.GetComponent<DecalProjector>();
                    if (decalProjector != null)
                    {
                        float time = 0;
                        while (time <= _effectData.startDelay)
                        {
                            var curValue = Mathf.Lerp(0, _effectData.sphereRadius * 2, time / _effectData.startDelay);

                            if (decalProjector == null) break;

                            decalProjector.size = Vector3.one * curValue;
                            time += Time.deltaTime;

                            if (_effectData.moveToPos)
                            {
                                _skillLocationInfo.trCaster.position += _skillLocationInfo.dirSkill.normalized * _effectData.moveSpeed * Time.deltaTime;
                            }

                            await Task.Delay((int)(Time.deltaTime * 1000f));
                        }

                        CHMMain.Resource.Destroy(objDecal);
                        CHMMain.Resource.Destroy(_areaDecal);
                    }
                }
                break;
            case Defines.ECollision.Box:
                break;
            default:
                break;
        }
    }
}

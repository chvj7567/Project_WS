using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Infomation;

public class CHMSkill
{
    GameObject roundAreaDecal = null;
    GameObject roundTimingDecal = null;

    public async void CreateSkill(Transform _trCaster, Transform _trTarget, Vector3 _posSkill, Vector3 _dirSkill, Defines.ESkillID _skill)
    {
        // 스킬 시전자가 죽었으면 스킬 발동 X
        var isDeath = _trCaster.GetComponent<CHUnitBase>().GetIsDeath();
        if (isDeath) return;

        var skillInfo = CHMMain.Json.GetSkillInfo(_skill);
        if (skillInfo != null)
        {
            // 논타겟 스킬은 타겟을 강제로 삭제
            if (skillInfo.isTargeting == false)
            {
                _trTarget = null;
            }

            var casterUnit = _trCaster.GetComponent<CHUnitBase>();
            if (casterUnit != null)
            {
                if (CanUseSkill(casterUnit, skillInfo) == false) return;
            }

            foreach (var effectInfo in skillInfo.liEffectInfo)
            {
                // 해당 위치로 움직일지 여부
                if (effectInfo.moveToPos)
                {
                    float distance = Vector3.Distance(_trCaster.position, _posSkill);
                    effectInfo.startDelay = (distance + effectInfo.offsetToTarget) / effectInfo.moveSpeed;
                }
                
                // 스킬 시전 딜레이 시간 전에 데칼로 스킬 시전 구역 알려줌
                if (effectInfo.onDecal && (Mathf.Approximately(0f, effectInfo.startDelay) == false))
                {
                    await CreateDecal(_trCaster, _trTarget, _posSkill, _dirSkill, effectInfo);
                }
                else
                {
                    if (effectInfo.moveToPos)
                    {
                        float time = 0f;
                        while (time <= effectInfo.startDelay)
                        {
                            _trCaster.position += _dirSkill.normalized * effectInfo.moveSpeed * Time.deltaTime;

                            time += Time.deltaTime;
                            await Task.Delay((int)(Time.deltaTime * 1000f));
                        }
                    }
                    else
                    {
                        await Task.Delay((int)(effectInfo.startDelay * 1000f));
                    }
                }

                if (_trCaster == null) return;

                // 스킬 충돌 범위 생성
                CreateSkillCollision(_trCaster, _trTarget, _posSkill, _dirSkill, effectInfo);
            }
        }
    }

    public List<TargetInfo> GetTargetInfoListInRange(Vector3 _originPos, Vector3 _direction, LayerMask _lmTarget, float _range, float _viewAngle = 360f)
    {
        List<TargetInfo> targetInfoList = new List<TargetInfo>();

        // 범위내에 있는 타겟들 확인
        Collider[] targets = Physics.OverlapSphere(_originPos, _range, _lmTarget);

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
                    targetInfoList.Add(new TargetInfo
                    {
                        objTarget = target.gameObject,
                        distance = targetDis,
                    });
                }
            }
        }

        return targetInfoList;
    }

    public List<TargetInfo> GetTargetInfoListInRange(Vector3 _originPos, LayerMask _lmTarget, Vector3 _size, Quaternion _quater)
    {
        List<TargetInfo> targetInfoList = new List<TargetInfo>();

        // 범위내에 있는 타겟들 확인
        Collider[] targets = Physics.OverlapBox(_originPos, _size / 2f, _quater, _lmTarget);

        foreach (Collider target in targets)
        {
            Transform targetTr = target.transform;
            Vector3 targetDir = (targetTr.position - _originPos).normalized;
            float targetDis = Vector3.Distance(_originPos, targetTr.position);

            // 장애물이 있는지 확인
            if (Physics.Raycast(_originPos, targetDir, targetDis, ~_lmTarget) == false)
            {
                targetInfoList.Add(new TargetInfo
                {
                    objTarget = target.gameObject,
                    distance = targetDis,
                });
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

    public void ApplySkillValue(Transform _trCaster, List<Transform> _liTarget, EffectInfo _effectInfo)
    {
        // 스킬 값들을(데미지나, 힐 등) _liTarget 적용

        var casterUnit = _trCaster.GetComponent<CHUnitBase>();

        foreach (var target in _liTarget)
        {
            if (target == null) continue;

            var targetUnit = target.GetComponent<CHUnitBase>();
            if (targetUnit != null)
            {
                ApplyEffectType(casterUnit, targetUnit, _effectInfo);
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

    void CreateSphereCollision(Transform _trCaster, Transform _trTarget, Vector3 _posSkill, Vector3 _dirSkill, EffectInfo _effectInfo)
    {
        LayerMask targetMask = GetTargetMask(_trCaster.gameObject.layer, _effectInfo.eTargetMask);

        List<TargetInfo> liTargetInfo = new List<TargetInfo>();
        List<Transform> liTarget = new List<Transform>();

        // 스킬 시작 시 해당 위치에 콜리젼 생성
        if (_effectInfo.eEffectPos != Defines.EEffectPos.Me_Only)
        {
            if (_trTarget == null)
            {
                // 논타겟팅 스킬
                liTargetInfo = GetTargetInfoListInRange(_posSkill, _dirSkill, targetMask, _effectInfo.sphereRadius, _effectInfo.collisionAngle);
                liTarget = GetTargetTransformList(liTargetInfo);

                // 논타겟팅 스킬은 생성 시에 타겟이 없을 수도 있음
                if (liTargetInfo == null || liTargetInfo.Count <= 0)
                {
                    // 파티클 기준이 자기 기준이 아닐 때만 파티클 생성
                    if ((_effectInfo.eEffectPos != Defines.EEffectPos.Me_Targeting) && (_effectInfo.eEffectPos != Defines.EEffectPos.Me_NoneTargeting))
                    {
                        CHMMain.Particle.CreateParticle(_trCaster, null, new List<Vector3> { _posSkill }, new List<Vector3> { _dirSkill }, _effectInfo);
                    }

                    return;
                }
            }
            else
            {
                // 타겟팅 스킬
                liTargetInfo = GetTargetInfoListInRange(_trTarget.position, _trTarget.forward, targetMask, _effectInfo.sphereRadius, _effectInfo.collisionAngle);
                liTarget = GetTargetTransformList(liTargetInfo);

                if (liTargetInfo == null || liTargetInfo.Count <= 0)
                {
                    Debug.Log("Targeting Skil : No Target Error");
                    return;
                }
            }
        }

        // 파티클 위치에 따라 파티클 생성
        switch (_effectInfo.eEffectPos)
        {
            case Defines.EEffectPos.Me_Only:
                {
                    if (_trTarget == null)
                    {
                        CHMMain.Particle.CreateParticle(_trCaster, null, new List<Vector3> { _trCaster.position }, new List<Vector3> { _trCaster.forward }, _effectInfo);
                    }
                    else
                    {
                        CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { _trCaster }, null, null, _effectInfo);
                    }
                }
                break;
            case Defines.EEffectPos.Me_Targeting:
                {
                    // 맞은 타겟 수 만큼 파티클 중복 여부
                    if (_effectInfo.duplication)
                    {
                        foreach (var target in liTarget)
                        {
                            CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { _trCaster }, null, null, _effectInfo);
                        }
                    }
                    else
                    {
                        CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { _trCaster }, null, null, _effectInfo);
                    }
                }
                break;
            case Defines.EEffectPos.Me_NoneTargeting:
                {
                    // 맞은 타겟 수 만큼 파티클 중복 여부
                    if (_effectInfo.duplication)
                    {
                        CHMMain.Particle.CreateParticle(_trCaster, null, new List<Vector3> { _trCaster.position }, new List<Vector3> { _trCaster.forward }, _effectInfo);
                    }
                    else
                    {
                        CHMMain.Particle.CreateParticle(_trCaster, null, new List<Vector3> { _trCaster.position }, new List<Vector3> { _trCaster.forward }, _effectInfo);
                    }
                }
                break;
            case Defines.EEffectPos.Target_One_Targeting:
                {
                    Transform targetOne = liTarget.First();

                    // 맞은 타겟 수 만큼 파티클 중복 여부
                    if (_effectInfo.duplication)
                    {
                        foreach (var target in liTarget)
                        {
                            CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { targetOne }, null, null, _effectInfo);
                        }
                    }
                    else
                    {
                        CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { targetOne }, null, null, _effectInfo);
                    }
                }
                break;
            case Defines.EEffectPos.Target_One_NoneTargeting:
                {
                    Transform targetOne = liTarget.First();

                    // 맞은 타겟 수 만큼 파티클 중복 여부
                    if (_effectInfo.duplication)
                    {
                        foreach (var target in liTarget)
                        {
                            CHMMain.Particle.CreateParticle(_trCaster, null, new List<Vector3> { _posSkill }, new List<Vector3> { _dirSkill }, _effectInfo);
                        }
                    }
                    else
                    {
                        CHMMain.Particle.CreateParticle(_trCaster, null, new List<Vector3> { _posSkill }, new List<Vector3> { _dirSkill }, _effectInfo);
                    }
                }
                break;
            case Defines.EEffectPos.Target_All_Targeting:
                {
                    CHMMain.Particle.CreateParticle(_trCaster, liTarget, null, null, _effectInfo);
                }
                break;
            case Defines.EEffectPos.Target_All_NoneTargeting:
                {
                    List<Vector3> liParticlePos = new List<Vector3>();
                    List<Vector3> liParticleDir = new List<Vector3>();

                    for (int i = 0; i < liTarget.Count; ++i)
                    {
                        liParticlePos.Add(liTarget[i].position);
                        liParticleDir.Add((liTarget[i].position - _trCaster.position).normalized);
                    }

                    CHMMain.Particle.CreateParticle(_trCaster, null, liParticlePos, liParticleDir, _effectInfo);
                }
                break;
            default:
                {
                    CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { _trTarget }, new List<Vector3> { _posSkill }, new List<Vector3> { _dirSkill }, _effectInfo);
                }
                break;
        }
    }

    void CreateSkillCollision(Transform _trCaster, Transform _trTarget, Vector3 _posSkill, Vector3 _dirSkill, EffectInfo _effectInfo)
    {
        // Collision 모양에 따라 구분
        switch (_effectInfo.eCollision)
        {
            case Defines.ECollision.Sphere:
                {
                    CreateSphereCollision(_trCaster, _trTarget, _posSkill, _dirSkill, _effectInfo);
                }
                break;
            case Defines.ECollision.Box:
                break;
            default:
                {
                    if (_effectInfo.eEffectPos == Defines.EEffectPos.Me_Only)
                    {
                        if (_trTarget == null)
                        {
                            CHMMain.Particle.CreateParticle(_trCaster, null, new List<Vector3> { _trCaster.position }, new List<Vector3> { _trCaster.forward }, _effectInfo);
                        }
                        else
                        {
                            CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { _trCaster }, null, null, _effectInfo);
                        }
                    }
                    else
                    {
                        if (_trTarget == null)
                        {
                            // 논타겟팅 스킬
                            CHMMain.Particle.CreateParticle(_trCaster, null, new List<Vector3> { _posSkill }, new List<Vector3> { _dirSkill }, _effectInfo);
                        }
                        else
                        {
                            // 타겟팅 스킬
                            CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { _trTarget }, null, null, _effectInfo);
                        }
                    }
                }
                break;
        }
    }

    void ApplyEffectType(CHUnitBase _casterUnit, CHUnitBase _targetUnit, EffectInfo _effectInfo)
    {
        if (_casterUnit == null || _targetUnit == null || _effectInfo == null) return;

        float skillValue = CalculateSkillDamage(_casterUnit, _targetUnit, _effectInfo);

        // 스킬 시전자 스탯
        float casterAttackPower = _casterUnit.GetCurrentAttackPower();
        float casterDefensePower = _casterUnit.GetCurrentDefensePower();
        // 타겟 스탯
        float targetAttackPower = _targetUnit.GetCurrentAttackPower();
        float targetDefensePower = _targetUnit.GetCurrentDefensePower();

        switch (_effectInfo.eEffectType)
        {
            case Defines.EEffectType.Hp_Up:
                Debug.Log($"HpUp : {skillValue}");
                _targetUnit.ChangeHp(_casterUnit, skillValue, _effectInfo.eDamageState);
                break;
            case Defines.EEffectType.Hp_Down:
                {
                    // 데미지 계산 : 스킬 데미지 + 스킬 시전자 공격력 - 타겟 방어력
                    float totalValue = skillValue + casterAttackPower - targetDefensePower;
                    // 데미지가 -이면 데미지를 줄 수 없다는 뜻
                    if (totalValue < 0)
                    {
                        totalValue = 0f;
                    }
                    Debug.Log($"HpDown : {totalValue}");
                    _targetUnit.ChangeHp(_casterUnit, CHUtil.ReverseValue(totalValue), _effectInfo.eDamageState);
                }
                break;
            case Defines.EEffectType.AttackPower_Up:
                _targetUnit.ChangeAttackPower(_casterUnit, skillValue, _effectInfo.eDamageState);
                break;
            case Defines.EEffectType.AttackPower_Down:
                _targetUnit.ChangeAttackPower(_casterUnit, CHUtil.ReverseValue(skillValue), _effectInfo.eDamageState);
                break;
            case Defines.EEffectType.DefensePower_Up:
                _targetUnit.ChangeDefensePower(_casterUnit, skillValue, _effectInfo.eDamageState);
                break;
            case Defines.EEffectType.DefensePower_Down:
                _targetUnit.ChangeDefensePower(_casterUnit, CHUtil.ReverseValue(skillValue), _effectInfo.eDamageState);
                break;
            default:
                break;
        }
    }

    float CalculateSkillDamage(CHUnitBase _casterUnit, CHUnitBase _targetUnit, EffectInfo _effectInfo)
    {
        if (_casterUnit == null || _targetUnit == null || _effectInfo == null) return 0f;

        // 데미지 타입에 따라 구분
        switch (_effectInfo.eDamageType)
        {
            case Defines.EDamageType.Fixed:
                return _effectInfo.damage;
            case Defines.EDamageType.Percent_Me_MaxHp:
                return _casterUnit.GetCurrentMaxHp() * _effectInfo.damage / 100f;
            case Defines.EDamageType.Percent_Me_RemainHp:
                return _casterUnit.GetCurrentHp() * _effectInfo.damage / 100f;
            case Defines.EDamageType.Percent_Target_MaxHp:
                return _targetUnit.GetCurrentMaxHp() * _effectInfo.damage / 100f;
            case Defines.EDamageType.Percent_Target_RemainHp:
                return _targetUnit.GetCurrentHp() * _effectInfo.damage / 100f;
            default:
                return 0f;
        }
    }

    bool CanUseSkill(CHUnitBase _casterUnit, SkillInfo skillInfo)
    {
        switch (skillInfo.eSkillCost)
        {
            case Defines.ESkillCost.Fixed_HP:
                {
                    if (_casterUnit.GetCurrentHp() >= skillInfo.cost)
                    {
                        _casterUnit.ChangeHp(_casterUnit, CHUtil.ReverseValue(skillInfo.cost), Defines.EDamageState.None);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case Defines.ESkillCost.Percent_MaxHP:
                {
                    var costValue = _casterUnit.GetCurrentMaxHp() * skillInfo.cost / 100f;

                    if (_casterUnit.GetCurrentHp() >= costValue)
                    {
                        _casterUnit.ChangeHp(_casterUnit, CHUtil.ReverseValue(costValue), Defines.EDamageState.None);
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
                        _casterUnit.ChangeHp(_casterUnit, CHUtil.ReverseValue(costValue), Defines.EDamageState.None);
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
                        _casterUnit.ChangeMp(_casterUnit, CHUtil.ReverseValue(skillInfo.cost), Defines.EDamageState.None);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case Defines.ESkillCost.Percent_MaxMP:
                {
                    var costValue = _casterUnit.GetCurrentMaxMp() * skillInfo.cost / 100f;

                    if (_casterUnit.GetCurrentMp() >= costValue)
                    {
                        _casterUnit.ChangeMp(_casterUnit, CHUtil.ReverseValue(costValue), Defines.EDamageState.None);
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
                        _casterUnit.ChangeMp(_casterUnit, CHUtil.ReverseValue(costValue), Defines.EDamageState.None);
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

    async Task CreateDecal(Transform _trCaster, Transform _trTarget, Vector3 _posDecal, Vector3 _dirDecal, EffectInfo _effectInfo)
    {
        GameObject objDecal = null;

        switch (_effectInfo.eCollision)
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

                    objDecal.transform.position = _posDecal;
                    objDecal.transform.forward = _dirDecal;

                    if (_trTarget != null)
                    {
                        objDecal.transform.SetParent(_trTarget.transform);
                    }

                    objDecal.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                    var decalProjector = objDecal.GetComponent<DecalProjector>();
                    if (decalProjector != null)
                    {
                        decalProjector.size = Vector3.one * _effectInfo.sphereRadius * 2f;
                    }
                }
                break;
            case Defines.ECollision.Box:
                break;
            default:
                break;
        }

        await CreateTimeDecal(_trCaster, _trTarget, _posDecal, _dirDecal, objDecal, _effectInfo);
    }

    async Task CreateTimeDecal(Transform _trCaster, Transform _trTarget, Vector3 _posDecal, Vector3 _dirDecal, GameObject _areaDecal, EffectInfo _effectInfo)
    {
        GameObject objDecal = null;

        switch (_effectInfo.eCollision)
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

                    objDecal.transform.position = _posDecal;
                    objDecal.transform.forward = _dirDecal;

                    if (_trTarget != null)
                    {
                        objDecal.transform.SetParent(_trTarget.transform);
                    }

                    objDecal.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                    var decalProjector = objDecal.GetComponent<DecalProjector>();
                    if (decalProjector != null)
                    {
                        float time = 0;
                        while (time <= _effectInfo.startDelay)
                        {
                            var curValue = Mathf.Lerp(0, _effectInfo.sphereRadius * 2f, time / _effectInfo.startDelay);

                            if (decalProjector == null) break;

                            decalProjector.size = Vector3.one * curValue;
                            time += Time.deltaTime;

                            if (_effectInfo.moveToPos)
                            {
                                _trCaster.position += _dirDecal.normalized * _effectInfo.moveSpeed * Time.deltaTime;
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

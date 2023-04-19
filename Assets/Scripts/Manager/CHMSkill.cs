using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Infomation;
using static UnityEngine.GraphicsBuffer;

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
                // 스킬 시전 딜레이 시간 전에 데칼로 스킬 시전 구역 알려줌
                if (effectInfo.onDecal || (Mathf.Approximately(effectInfo.startDelay, 0f) == false))
                {
                    await CreateDecal(effectInfo, _trTarget, _posSkill, _dirSkill);
                }
                else
                {
                    await Task.Delay((int)(effectInfo.startDelay * 1000f));
                }

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

                // 장애물이 있는지 확인
                if (Physics.Raycast(_originPos, targetDir, targetDis, ~_lmTarget) == false)
                {
                    // 타겟이 살아있으면 타겟으로 지정
                    if (target.GetComponent<CHUnitBase>().GetIsDeath() == false)
                    {
                        targetInfoList.Add(new TargetInfo
                        {
                            objTarget = target.gameObject,
                            direction = targetDir,
                            distance = targetDis,
                        });
                    }
                }
            }
        }

        return targetInfoList;
    }

    public TargetInfo GetClosestTargetInfo(Vector3 _originPos, Vector3 _direction, LayerMask _lmTarget, float _range, float _viewAngle = 360f)
    {
        TargetInfo closestTargetInfo = null;
        List<TargetInfo> targetInfoList = GetTargetInfoListInRange(_originPos, _direction, _lmTarget, _range, _viewAngle);

        if (targetInfoList.Count > 0)
        {
            float minDis = Mathf.Infinity;

            foreach (TargetInfo targetInfo in targetInfoList)
            {
                if (targetInfo.distance < minDis)
                {
                    minDis = targetInfo.distance;
                    closestTargetInfo = targetInfo;
                }
            }
        }

        return closestTargetInfo;
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
                    direction = targetDir,
                    distance = targetDis,
                });
            }
        }

        return targetInfoList;
    }

    public TargetInfo GetClosestTargetInfo(Vector3 _originPos, LayerMask _lmTarget, Vector3 _size, Quaternion _quater)
    {
        TargetInfo closestTargetInfo = null;
        List<TargetInfo> targetInfoList = GetTargetInfoListInRange(_originPos, _lmTarget, _size, _quater);

        if (targetInfoList.Count > 0)
        {
            float minDis = Mathf.Infinity;

            foreach (TargetInfo targetInfo in targetInfoList)
            {
                if (targetInfo.distance < minDis)
                {
                    minDis = targetInfo.distance;
                    closestTargetInfo = targetInfo;
                }
            }
        }

        return closestTargetInfo;
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

    public LayerMask GetTargetMask(LayerMask _myLayerMask, Defines.ETargetMask _targetMask)
    {
        LayerMask enemyLayerMask;
        if (LayerMask.LayerToName(_myLayerMask) == "Red")
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
                return _myLayerMask;
            case Defines.ETargetMask.Enemy:
                return enemyLayerMask;
            case Defines.ETargetMask.MyTeam_Enemy:
                return _myLayerMask | enemyLayerMask;
            default:
                return -1;
        }
    }

    //-------------------------------------- private ------------------------------------------//

    void CreateSphereCollision(Transform _trCaster, Transform _trTarget, Vector3 _posSkill, Vector3 _dirSkill, EffectInfo _effectInfo)
    {
        LayerMask targetMask = GetTargetMask(_trCaster.gameObject.layer, _effectInfo.eTargetMask);

        // 스킬 시전 시 맞은 타겟들
        var liTargetInfo = GetTargetInfoListInRange(_posSkill, _dirSkill, targetMask, _effectInfo.sphereRadius, _effectInfo.angle);
        var liTarget = GetTargetTransformList(liTargetInfo);

        // 파티클 위치에 따라 파티클 생성
        switch (_effectInfo.eEffectPos)
        {
            case Defines.EEffectPos.Me:
                {
                    // 맞은 타겟 수 만큼 파티클 중복 여부
                    if (_effectInfo.duplication)
                    {
                        foreach (var target in liTarget)
                        {
                            ApplySkillValue(_trCaster, new List<Transform> { _trTarget }, _effectInfo);
                            CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { _trTarget }, new List<Vector3> { _trCaster.position }, new List<Vector3> { _dirSkill }, _effectInfo);
                        }
                    }
                    else
                    {
                        ApplySkillValue(_trCaster, new List<Transform> { _trTarget }, _effectInfo);
                        CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { _trTarget }, new List<Vector3> { _trCaster.position }, new List<Vector3> { _dirSkill }, _effectInfo);
                    }
                }
                break;
            case Defines.EEffectPos.TargetOne:
                {
                    // 맞은 타겟 수 만큼 파티클 중복 여부
                    if (_effectInfo.duplication)
                    {
                        foreach (var target in liTarget)
                        {
                            ApplySkillValue(_trCaster, new List<Transform> { _trTarget }, _effectInfo);
                            CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { _trTarget }, new List<Vector3> { _trTarget.position }, new List<Vector3> { _dirSkill }, _effectInfo);
                        }
                    }
                    else
                    {
                        ApplySkillValue(_trCaster, new List<Transform> { _trTarget }, _effectInfo);
                        CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { _trTarget }, new List<Vector3> { _trTarget.position }, new List<Vector3> { _dirSkill }, _effectInfo);
                    }
                }
                break;
            case Defines.EEffectPos.TargetAll:
                {
                    List<Vector3> liParticlePos = new List<Vector3>();
                    List<Vector3> liParticleDir = new List<Vector3>();

                    for (int i = 0; i < liTarget.Count; ++i)
                    {
                        liParticlePos.Add(liTarget[i].position);
                        liParticleDir.Add((liTarget[i].position - _trCaster.position).normalized);
                    }

                    ApplySkillValue(_trCaster, liTarget, _effectInfo);

                    if (_trTarget == null)
                    {
                        CHMMain.Particle.CreateParticle(_trCaster, null, liParticlePos, liParticleDir, _effectInfo);
                    }
                    else
                    {
                        CHMMain.Particle.CreateParticle(_trCaster, liTarget, liParticlePos, liParticleDir, _effectInfo);
                    }
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
                    // Collision이 없으면 해당 지점에 파티클만 생성
                    CHMMain.Particle.CreateParticle(_trCaster, new List<Transform> { _trTarget }, new List<Vector3> { _posSkill }, new List<Vector3> { _dirSkill }, _effectInfo);
                }
                break;
        }
    }

    void ApplySkillValue(Transform _trCaster, List<Transform> _liTarget, EffectInfo _effectInfo)
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
                _targetUnit.ChangeHp(skillValue, _effectInfo.eDamageState);
                break;
            case Defines.EEffectType.Hp_Down:
                {
                    // 데미지 계산 : 스킬 데미지 + 스킬 시전자 공격력 - 타겟 방어력
                    var totalValue = skillValue + casterAttackPower - targetDefensePower;
                    Debug.Log($"HpDown : {totalValue} = {skillValue} + {casterAttackPower} - {targetDefensePower}");
                    _targetUnit.ChangeHp(CHUtil.ReverseValue(totalValue), _effectInfo.eDamageState);
                }
                break;
            case Defines.EEffectType.AttackPower_Up:
                _targetUnit.ChangeAttackPower(skillValue, _effectInfo.eDamageState);
                break;
            case Defines.EEffectType.AttackPower_Down:
                _targetUnit.ChangeAttackPower(CHUtil.ReverseValue(skillValue), _effectInfo.eDamageState);
                break;
            case Defines.EEffectType.DefensePower_Up:
                _targetUnit.ChangeDefensePower(skillValue, _effectInfo.eDamageState);
                break;
            case Defines.EEffectType.DefensePower_Down:
                _targetUnit.ChangeDefensePower(CHUtil.ReverseValue(skillValue), _effectInfo.eDamageState);
                break;
            default:
                break;
        }
    }

    float CalculateSkillDamage(CHUnitBase _casterUnit, CHUnitBase _targetUnit, EffectInfo _effectInfo)
    {
        if (_casterUnit == null || _targetUnit == null || _effectInfo == null) return -1f;

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
                return -1f;
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
                        _casterUnit.ChangeHp(CHUtil.ReverseValue(skillInfo.cost), Defines.EDamageState.None);
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
                        _casterUnit.ChangeHp(CHUtil.ReverseValue(costValue), Defines.EDamageState.None);
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
                        _casterUnit.ChangeHp(CHUtil.ReverseValue(costValue), Defines.EDamageState.None);
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
                        _casterUnit.ChangeMp(CHUtil.ReverseValue(skillInfo.cost), Defines.EDamageState.None);
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
                        _casterUnit.ChangeMp(CHUtil.ReverseValue(costValue), Defines.EDamageState.None);
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
                        _casterUnit.ChangeMp(CHUtil.ReverseValue(costValue), Defines.EDamageState.None);
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

    async Task CreateDecal(EffectInfo _effectInfo, Transform _trTarget, Vector3 _posDecal, Vector3 _dirDecal)
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
                        decalProjector.size = Vector3.one * _effectInfo.sphereRadius;
                    }
                }
                break;
            case Defines.ECollision.Box:
                break;
            default:
                break;
        }

        await CreateTimeDecal(_effectInfo, _trTarget, _posDecal, _dirDecal, objDecal);
    }

    async Task CreateTimeDecal(EffectInfo _effectInfo, Transform _trTarget, Vector3 _posDecal, Vector3 _dirDecal, GameObject _areaDecal)
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
                            var curValue = Mathf.Lerp(0, _effectInfo.sphereRadius, time / _effectInfo.startDelay);
                            decalProjector.size = Vector3.one * curValue;
                            time += Time.deltaTime;
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

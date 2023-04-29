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

    public async void CreateSkill(SkillLocationInfo _skillLocationInfo, Defines.ESkillID _skill)
    {
        // ��ų �����ڰ� �׾����� ��ų �ߵ� X
        var isDeath = _skillLocationInfo.trCaster.GetComponent<CHUnitBase>().GetIsDeath();
        if (isDeath) return;

        var skillInfo = CHMMain.Json.GetSkillInfo(_skill);
        if (skillInfo != null)
        {
            var casterUnit = _skillLocationInfo.trCaster.GetComponent<CHUnitBase>();
            if (casterUnit != null)
            {
                if (CanUseSkill(casterUnit, skillInfo) == false) return;
            }

            foreach (var effectInfo in skillInfo.liEffectInfo)
            {
                // �ش� ��ġ�� �������� ����
                if (effectInfo.moveToPos)
                {
                    float distance = Vector3.Distance(_skillLocationInfo.trCaster.position, _skillLocationInfo.posSkill);
                    effectInfo.startDelay = (distance + effectInfo.offsetToTarget) / effectInfo.moveSpeed;
                }
                
                // ��ų ���� ������ �ð� ���� ��Į�� ��ų ���� ���� �˷���
                if (effectInfo.onDecal && (Mathf.Approximately(0f, effectInfo.startDelay) == false))
                {
                    await CreateDecal(_skillLocationInfo, effectInfo, skillInfo.isTargeting);
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

                // ��ų �浹 ���� ����
                CreateSkillCollision(_skillLocationInfo, effectInfo, skillInfo.isTargeting);
            }
        }
    }

    public List<TargetInfo> GetTargetInfoListInRange(Vector3 _originPos, Vector3 _direction, LayerMask _lmTarget, float _range, float _viewAngle = 360f)
    {
        List<TargetInfo> targetInfoList = new List<TargetInfo>();

        // �������� �ִ� Ÿ�ٵ� Ȯ��
        Collider[] targets = Physics.OverlapSphere(_originPos, _range, _lmTarget);

        float minDistance = float.MaxValue;

        foreach (Collider target in targets)
        {
            Transform targetTr = target.transform;
            Vector3 targetDir = (targetTr.position - _originPos).normalized;

            // �þ߰��� �ɸ����� Ȯ��
            if (Vector3.Angle(_direction, targetDir) < _viewAngle / 2)
            {
                float targetDis = Vector3.Distance(_originPos, targetTr.position);

                var unitBase = target.GetComponent<CHUnitBase>();
                // Ÿ���� ��������� Ÿ������ ����
                if (unitBase != null && unitBase.GetIsDeath() == false)
                {
                    // ���� ª�� �Ÿ��� �ִ� Ÿ���� ����Ʈ�� ù��°��
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

    public void ApplySkillValue(Transform _trCaster, List<Transform> _liTarget, EffectInfo _effectInfo)
    {
        // ��ų ������(��������, �� ��) _liTarget ����

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

    void CreateSphereCollision(SkillLocationInfo _skillLocationInfo, EffectInfo _effectInfo, bool _isTargeting)
    {
        SkillLocationInfo skillLocationInfo = _skillLocationInfo.Copy();
        LayerMask targetMask = GetTargetMask(skillLocationInfo.trCaster.gameObject.layer, _effectInfo.eTargetMask);

        List<TargetInfo> liTargetInfo = new List<TargetInfo>();
        List<Transform> liTarget = new List<Transform>();

        // ��ų ���� �� �ش� ��ġ�� �ݸ��� ����
        if (_isTargeting == false)
        {
            // ��Ÿ���� ��ų
            if (_effectInfo.createCasterPosition == false)
            {
                liTargetInfo = GetTargetInfoListInRange(skillLocationInfo.posSkill, skillLocationInfo.dirSkill, targetMask, _effectInfo.sphereRadius, _effectInfo.collisionAngle);
                liTarget = GetTargetTransformList(liTargetInfo);
            }
            else
            {
                // ��ų ������ ����� ��ų ������ ��ġ�� �ݸ��� ����
                liTargetInfo = GetTargetInfoListInRange(skillLocationInfo.posCaster, skillLocationInfo.dirCaster, targetMask, _effectInfo.sphereRadius, _effectInfo.collisionAngle);
                liTarget = GetTargetTransformList(liTargetInfo);

                skillLocationInfo.posSkill = skillLocationInfo.posCaster;
                skillLocationInfo.dirSkill = skillLocationInfo.dirCaster;
            }

            // ��Ÿ���� ��ų�� ���� �ÿ� Ÿ���� ���� ���� ����
            if (liTargetInfo == null || liTargetInfo.Count <= 0)
            {
                if (_effectInfo.createOnEmpty)
                {
                    CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { skillLocationInfo.trTarget },
                        new List<Vector3> { skillLocationInfo.posSkill }, new List<Vector3> { skillLocationInfo.dirSkill }, _effectInfo);
                }

                return;
            }
        }
        else
        {
            // Ÿ���� ��ų
            if (_effectInfo.createCasterPosition == false)
            {
                liTargetInfo = GetTargetInfoListInRange(skillLocationInfo.trTarget.position, skillLocationInfo.trTarget.forward, targetMask, _effectInfo.sphereRadius, _effectInfo.collisionAngle);
                liTarget = GetTargetTransformList(liTargetInfo);
            }
            else
            {
                // ��ų ������ ��ġ�� �ݸ��� ����
                liTargetInfo = GetTargetInfoListInRange(skillLocationInfo.trCaster.position, skillLocationInfo.trCaster.forward, targetMask, _effectInfo.sphereRadius, _effectInfo.collisionAngle);
                liTarget = GetTargetTransformList(liTargetInfo);

                skillLocationInfo.posSkill = skillLocationInfo.posCaster;
                skillLocationInfo.dirSkill = skillLocationInfo.dirCaster;
            }

            if (liTargetInfo == null || liTargetInfo.Count <= 0)
            {
                Debug.Log("Targeting Skill : No Target Error");
                return;
            }
        }

        // ��ƼŬ ��ġ�� ���� ��ƼŬ ����
        switch (_effectInfo.eTarget)
        {
            case Defines.ETarget.Position:
                {
                    if (_effectInfo.duplication)
                    {
                        foreach (var target in liTarget)
                        {
                            CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { skillLocationInfo.trTarget },
                                new List<Vector3> { skillLocationInfo.posSkill }, new List<Vector3> { skillLocationInfo.dirSkill }, _effectInfo);
                        }
                    }
                    else
                    {
                        CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { skillLocationInfo.trTarget },
                            new List<Vector3> { skillLocationInfo.posSkill }, new List<Vector3> { skillLocationInfo.dirSkill }, _effectInfo);
                    }
                }
                break;
            case Defines.ETarget.Target_One:
                {
                    Transform targetOne = liTarget.First();

                    // ���� Ÿ�� �� ��ŭ ��ƼŬ �ߺ� ����
                    if (_effectInfo.duplication)
                    {
                        foreach (var target in liTarget)
                        {
                            CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { targetOne },
                                new List<Vector3> { targetOne.position }, new List<Vector3> { targetOne.forward }, _effectInfo);
                        }
                    }
                    else
                    {
                        CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { targetOne },
                            new List<Vector3> { targetOne.position }, new List<Vector3> { targetOne.forward }, _effectInfo);
                    }
                }
                break;
            case Defines.ETarget.Target_All:
                {
                    List<Vector3> liParticlePos = new List<Vector3>();
                    List<Vector3> liParticleDir = new List<Vector3>();

                    for (int i = 0; i < liTarget.Count; ++i)
                    {
                        liParticlePos.Add(liTarget[i].position);
                        liParticleDir.Add((liTarget[i].position - skillLocationInfo.trCaster.position).normalized);
                    }

                    CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, liTarget, liParticlePos, liParticleDir, _effectInfo);
                }
                break;
            default:
                {
                    CHMMain.Particle.CreateParticle(skillLocationInfo.trCaster, new List<Transform> { skillLocationInfo.trTarget },
                        new List<Vector3> { skillLocationInfo.posSkill }, new List<Vector3> { skillLocationInfo.dirSkill }, _effectInfo);
                }
                break;
        }
    }

    void CreateSkillCollision(SkillLocationInfo _skillLocationInfo, EffectInfo _effectInfo, bool _isTargeting)
    {
        // Collision ��翡 ���� ����
        switch (_effectInfo.eCollision)
        {
            case Defines.ECollision.Sphere:
                {
                    CreateSphereCollision(_skillLocationInfo, _effectInfo, _isTargeting);
                }
                break;
            case Defines.ECollision.Box:
                break;
        }
    }

    void ApplyEffectType(CHUnitBase _casterUnit, CHUnitBase _targetUnit, EffectInfo _effectInfo)
    {
        if (_casterUnit == null || _targetUnit == null || _effectInfo == null) return;

        float skillValue = CalculateSkillDamage(_casterUnit, _targetUnit, _effectInfo);

        // ��ų ������ ����
        float casterAttackPower = _casterUnit.GetCurrentAttackPower();
        float casterDefensePower = _casterUnit.GetCurrentDefensePower();
        // Ÿ�� ����
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
                    // ������ ��� : ��ų ������ + ��ų ������ ���ݷ� - Ÿ�� ����
                    float totalValue = skillValue + casterAttackPower - targetDefensePower;
                    // �������� -�̸� �������� �� �� ���ٴ� ��
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

        // ������ Ÿ�Կ� ���� ����
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

    async Task CreateDecal(SkillLocationInfo _skillLocationInfo, EffectInfo _effectInfo, bool _isTargeting)
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

                    bool createCasterPosition = _effectInfo.createCasterPosition;

                    if (createCasterPosition == false)
                    {
                        objDecal.transform.position = _skillLocationInfo.posSkill;
                        objDecal.transform.forward = _skillLocationInfo.dirSkill;

                        if (_isTargeting)
                        {
                            objDecal.transform.SetParent(_skillLocationInfo.trTarget.transform);
                        }
                    }
                    else
                    {
                        objDecal.transform.position = _skillLocationInfo.trCaster.position;
                        objDecal.transform.forward = _skillLocationInfo.trCaster.forward;

                        if (_isTargeting)
                        {
                            objDecal.transform.SetParent(_skillLocationInfo.trCaster.transform);
                        }
                    }

                    objDecal.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                    var decalProjector = objDecal.GetComponent<DecalProjector>();
                    if (decalProjector != null)
                    {
                        decalProjector.size = Vector3.one * _effectInfo.sphereRadius * 2;
                    }
                }
                break;
            case Defines.ECollision.Box:
                break;
        }

        await CreateTimeDecal(_skillLocationInfo, objDecal, _effectInfo, _isTargeting);
    }

    async Task CreateTimeDecal(SkillLocationInfo _skillLocationInfo, GameObject _areaDecal, EffectInfo _effectInfo, bool _isTargeting)
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

                    if (_effectInfo.createCasterPosition == false)
                    {
                        objDecal.transform.position = _skillLocationInfo.posSkill;
                        objDecal.transform.forward = _skillLocationInfo.dirSkill;

                        if (_isTargeting)
                        {
                            objDecal.transform.SetParent(_skillLocationInfo.trTarget.transform);
                        }
                    }
                    else
                    {
                        objDecal.transform.position = _skillLocationInfo.trCaster.position;
                        objDecal.transform.forward = _skillLocationInfo.trCaster.forward;

                        if (_isTargeting)
                        {
                            objDecal.transform.SetParent(_skillLocationInfo.trCaster.transform);
                        }
                    }

                    objDecal.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                    var decalProjector = objDecal.GetComponent<DecalProjector>();
                    if (decalProjector != null)
                    {
                        float time = 0;
                        while (time <= _effectInfo.startDelay)
                        {
                            var curValue = Mathf.Lerp(0, _effectInfo.sphereRadius * 2, time / _effectInfo.startDelay);

                            if (decalProjector == null) break;

                            decalProjector.size = Vector3.one * curValue;
                            time += Time.deltaTime;

                            if (_effectInfo.moveToPos)
                            {
                                _skillLocationInfo.trCaster.position += _skillLocationInfo.dirSkill.normalized * _effectInfo.moveSpeed * Time.deltaTime;
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Infomation;

public class CHMSkill
{
    GameObject roundAreaDecal = null;
    GameObject roundTimingDecal = null;

    public void CreateAISkill(Transform _trCaster, Transform _trTarget, Defines.ESkillID _skill)
    {
        var skillInfo = CHMMain.Json.GetSkillInfo(_skill);

        // ��ų �����ڰ� �׾����� ��ų �ߵ� X
        var isDeath = _trCaster.GetComponent<CHUnitBase>().GetIsDeath();

        if (skillInfo != null && isDeath == false)
        {
            // Ÿ���� ��ų, ��Ÿ���� ��ų ����
            if (skillInfo.isTargeting)
            {
                CreateTargetingSkill(_trCaster, _trTarget, _skill);
            }
            else
            {
                CreateNoneTargetingSkill(_trCaster, _trTarget.position, _trTarget.position - _trCaster.position, _skill);
            }
        }
    }

    public async void CreateTargetingSkill(Transform _trCaster, Transform _trTarget, Defines.ESkillID _skill)
    {
        var skillInfo = CHMMain.Json.GetSkillInfo(_skill);

        if (skillInfo != null)
        {
            var casterUnit = _trCaster.GetComponent<CHUnitBase>();
            if (casterUnit != null)
            {
                if (CanUseSkill(casterUnit, skillInfo) == false) return;
            }

            foreach (var effectInfo in skillInfo.liEffectInfo)
            {
                // ��ų ���� ������ �ð� ���� ��Į�� ��ų ���� ���� �˷���
                if (effectInfo.onDecal)
                {
                    await CreateTargetingDecal(effectInfo, _trTarget);
                }
                else
                {
                    await Task.Delay((int)(effectInfo.startDelay * 1000f));
                }

                // ��ų �浹 ���� ����
                CreateTargetingSkillCollision(_trCaster, _trTarget, effectInfo);
            }
        }
    }

    public async void CreateNoneTargetingSkill(Transform _trCaster, Vector3 _posSkill, Vector3 _dirSkill, Defines.ESkillID _skill)
    {
        var skillInfo = CHMMain.Json.GetSkillInfo(_skill);

        if (skillInfo != null)
        {
            var casterUnit = _trCaster.GetComponent<CHUnitBase>();
            if (casterUnit != null)
            {
                if (CanUseSkill(casterUnit, skillInfo) == false) return;
            }

            foreach (var effectInfo in skillInfo.liEffectInfo)
            {
                // ��ų ���� ������ �ð� ���� ��Į�� ��ų ���� ���� �˷���
                if (effectInfo.onDecal)
                {
                    await CreateNoneTargetingDecal(effectInfo, _posSkill, _dirSkill);
                }
                else
                {
                    await Task.Delay((int)(effectInfo.startDelay * 1000f));
                }

                // ��ų �浹 ���� ����
                CreateNoneTargetingSkillCollision(_trCaster, _posSkill, _dirSkill, effectInfo);
            }
        }
    }

    public List<TargetInfo> GetTargetInfoListInRange(Vector3 _originPos, Vector3 _direction, LayerMask _lmTarget, float _range, float _viewAngle = 360f)
    {
        List<TargetInfo> targetInfoList = new List<TargetInfo>();

        // �������� �ִ� Ÿ�ٵ� Ȯ��
        Collider[] targets = Physics.OverlapSphere(_originPos, _range, _lmTarget);

        foreach (Collider target in targets)
        {
            Transform targetTr = target.transform;
            Vector3 targetDir = (targetTr.position - _originPos).normalized;

            // �þ߰��� �ɸ����� Ȯ��
            if (Vector3.Angle(_direction, targetDir) < _viewAngle / 2)
            {
                float targetDis = Vector3.Distance(_originPos, targetTr.position);

                // ��ֹ��� �ִ��� Ȯ��
                if (Physics.Raycast(_originPos, targetDir, targetDis, ~_lmTarget) == false)
                {
                    // Ÿ���� ��������� Ÿ������ ����
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

        // �������� �ִ� Ÿ�ٵ� Ȯ��
        Collider[] targets = Physics.OverlapBox(_originPos, _size / 2f, _quater, _lmTarget);

        foreach (Collider target in targets)
        {
            Transform targetTr = target.transform;
            Vector3 targetDir = (targetTr.position - _originPos).normalized;
            float targetDis = Vector3.Distance(_originPos, targetTr.position);

            // ��ֹ��� �ִ��� Ȯ��
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

    public int GetTargetMask(Defines.ETargetMask _targetMask)
    {
        switch (_targetMask)
        {
            case Defines.ETargetMask.Me:
                return LayerMask.GetMask(Defines.ETargetMask.Me.ToString());
            case Defines.ETargetMask.Red:
                return LayerMask.GetMask(Defines.ETargetMask.Red.ToString());
            case Defines.ETargetMask.Blue:
                return LayerMask.GetMask(Defines.ETargetMask.Blue.ToString());
            case Defines.ETargetMask.Me_Red:
                return LayerMask.GetMask(Defines.ETargetMask.Me.ToString()) | LayerMask.GetMask(Defines.ETargetMask.Red.ToString());
            case Defines.ETargetMask.Me_Blue:
                return LayerMask.GetMask(Defines.ETargetMask.Me.ToString()) | LayerMask.GetMask(Defines.ETargetMask.Blue.ToString());
            case Defines.ETargetMask.Red_Blue:
                return LayerMask.GetMask(Defines.ETargetMask.Red.ToString()) | LayerMask.GetMask(Defines.ETargetMask.Blue.ToString());
            case Defines.ETargetMask.Me_Red_Blue:
                return LayerMask.GetMask(Defines.ETargetMask.Me.ToString()) | LayerMask.GetMask(Defines.ETargetMask.Red.ToString()) | LayerMask.GetMask(Defines.ETargetMask.Blue.ToString());
            default:
                return -1;
        }
    }

    //-------------------------------------- private ------------------------------------------//

    void CreateTargetingSkillCollision(Transform _trCaster, Transform _trTarget, EffectInfo _effectInfo)
    {
        // Collision ��翡 ���� ����
        switch (_effectInfo.eCollision)
        {
            case Defines.ECollision.Sphere:
                {
                    CreateTargetingSphereCollision(_trCaster, _trTarget, _effectInfo);
                }
                break;
            case Defines.ECollision.Box:
                break;
            default:
                {
                    // Collision�� ������ �ش� ������ ��ƼŬ�� ����
                    CHMMain.Particle.CreateTargetingParticle(_trCaster, new List<Transform> { _trTarget }, _effectInfo);
                }
                break;
        }
    }

    void CreateTargetingSphereCollision(Transform _trCaster, Transform _trTarget, EffectInfo _effectInfo)
    {
        // ��ƼŬ ���� ���� ��ġ�� ��ƼŬ ����
        switch (_effectInfo.eStandardPos)
        {
            case Defines.EStandardPos.Me:
                {
                    // Ÿ�ٿ��� ��ų ����
                    ApplySkillValue(_trCaster, new List<Transform> { _trCaster }, _effectInfo);

                    CHMMain.Particle.CreateTargetingParticle(_trCaster, new List<Transform> { _trCaster }, _effectInfo);
                }
                break;
            case Defines.EStandardPos.Target_One:
                {
                    // ��ų ���� �� ���� Ÿ�ٵ�
                    var liTargetInfo = GetClosestTargetInfo(_trTarget.position, _trTarget.position - _trCaster.position, GetTargetMask(_effectInfo.eTargetMask), _effectInfo.sphereRadius, _effectInfo.angle);
                    var liTarget = GetTargetTransformList(liTargetInfo);

                    if (liTarget == null)
                    {
                        Debug.Log("No Target");
                        return;
                    }

                    // Ÿ�ٿ��� ��ų ����
                    ApplySkillValue(_trCaster, liTarget, _effectInfo);

                    CHMMain.Particle.CreateTargetingParticle(_trCaster, new List<Transform> { _trTarget }, _effectInfo);
                }
                break;
            case Defines.EStandardPos.Target_All:
                {
                    // ��ų ���� �� ���� Ÿ�ٵ�
                    var liTargetInfo = GetTargetInfoListInRange(_trTarget.position, _trTarget.position - _trCaster.position, GetTargetMask(_effectInfo.eTargetMask), _effectInfo.sphereRadius, _effectInfo.angle);
                    var liTarget = GetTargetTransformList(liTargetInfo);

                    if (liTarget == null)
                    {
                        Debug.Log("No Target");
                        return;
                    }

                    // Ÿ�ٿ��� ��ų ����
                    ApplySkillValue(_trCaster, liTarget, _effectInfo);

                    CHMMain.Particle.CreateTargetingParticle(_trCaster, liTarget, _effectInfo);
                }
                break;
            default:
                break;
        }
    }

    void CreateNoneTargetingSkillCollision(Transform _trCaster, Vector3 _posSkill, Vector3 _dirSkill, EffectInfo _effectInfo)
    {
        // Collision ��翡 ���� ����
        switch (_effectInfo.eCollision)
        {
            case Defines.ECollision.Sphere:
                {
                    CreateNoneTargetingSphereCollision(_trCaster, _posSkill, _dirSkill, _effectInfo);
                }
                break;
            case Defines.ECollision.Box:
                break;
            default:
                {
                    // Collision�� ������ �ش� ������ ��ƼŬ�� ����
                    CHMMain.Particle.CreateNoneTargetingParticle(_posSkill, _dirSkill, _effectInfo);
                }
                break;
        }
    }

    void CreateNoneTargetingSphereCollision(Transform _trCaster, Vector3 _posSkill, Vector3 _dirSkill, EffectInfo _effectInfo)
    {
        List<Transform> liTarget = new List<Transform>();

        // ��ų ���� �� ���� Ÿ�ٵ�
        var liTargetInfo = GetTargetInfoListInRange(_posSkill, _dirSkill, GetTargetMask(_effectInfo.eTargetMask), _effectInfo.sphereRadius, _effectInfo.angle);
        liTarget = GetTargetTransformList(liTargetInfo);

        if (liTarget == null)
        {
            Debug.Log("No Target");
            return;
        }

        // Ÿ�ٿ��� ��ų �� ����
        ApplySkillValue(_trCaster, liTarget, _effectInfo);

        // ��Ÿ���� ��ų�� ��ƼŬ ���� ���� ��ġ�� ������� �ش� ��ġ�� ��ƼŬ ���� 
        CHMMain.Particle.CreateNoneTargetingParticle(_posSkill, _dirSkill, _effectInfo);
    }

    void ApplySkillValue(Transform _trCaster, List<Transform> _liTarget, EffectInfo _effectInfo)
    {
        var casterUnit = _trCaster.GetComponent<CHUnitBase>();

        foreach (var target in _liTarget)
        {
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

        // ��ų ������ ����
        float casterAttackPower = _casterUnit.GetCurrentAttackPower();
        float casterDefensePower = _casterUnit.GetCurrentDefensePower();
        // Ÿ�� ����
        float targetAttackPower = _targetUnit.GetCurrentAttackPower();
        float targetDefensePower = _targetUnit.GetCurrentDefensePower();

        switch (_effectInfo.eEffectType)
        {
            case Defines.EEffectType.HpUp:
                _targetUnit.ChangeHp(skillValue, _effectInfo.eDamageState);
                break;
            case Defines.EEffectType.HpDown:
                {
                    // ������ ��� : ��ų ������ + ��ų ������ ���ݷ� - Ÿ�� ����
                    var totalValue = skillValue + casterAttackPower - targetDefensePower;
                    Debug.Log($"HpDown : {totalValue} = {skillValue} + {casterAttackPower} - {targetDefensePower}");
                    _targetUnit.ChangeHp(CHUtil.ReverseValue(totalValue), _effectInfo.eDamageState);
                }
                break;
            case Defines.EEffectType.AttackPowerUp:
                _targetUnit.ChangeAttackPower(skillValue, _effectInfo.eDamageState);
                break;
            case Defines.EEffectType.AttackPowerDown:
                _targetUnit.ChangeAttackPower(CHUtil.ReverseValue(skillValue), _effectInfo.eDamageState);
                break;
            case Defines.EEffectType.DefensePowerUp:
                _targetUnit.ChangeDefensePower(skillValue, _effectInfo.eDamageState);
                break;
            case Defines.EEffectType.DefensePowerDown:
                _targetUnit.ChangeDefensePower(CHUtil.ReverseValue(skillValue), _effectInfo.eDamageState);
                break;
            default:
                break;
        }
    }

    float CalculateSkillDamage(CHUnitBase _casterUnit, CHUnitBase _targetUnit, EffectInfo _effectInfo)
    {
        if (_casterUnit == null || _targetUnit == null || _effectInfo == null) return -1f;

        // ������ Ÿ�Կ� ���� ����
        switch (_effectInfo.eDamageType)
        {
            case Defines.EDamageType.Fixed:
                return _effectInfo.damage;
            case Defines.EDamageType.PercentMeMaxHp:
                return _casterUnit.GetCurrentMaxHp() * _effectInfo.damage / 100f;
            case Defines.EDamageType.PercentMeRemainHp:
                return _casterUnit.GetCurrentHp() * _effectInfo.damage / 100f;
            case Defines.EDamageType.PercentTargetMaxHp:
                return _targetUnit.GetCurrentMaxHp() * _effectInfo.damage / 100f;
            case Defines.EDamageType.PercentTargetRemainHp:
                return _targetUnit.GetCurrentHp() * _effectInfo.damage / 100f;
            default:
                return -1f;
        }
    }

    bool CanUseSkill(CHUnitBase _casterUnit, SkillInfo skillInfo)
    {
        switch (skillInfo.eSkillCost)
        {
            case Defines.ESkillCost.FixedHP:
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
            case Defines.ESkillCost.PercentMaxHP:
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
            case Defines.ESkillCost.PercentRemainHP:
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
            case Defines.ESkillCost.FixedMP:
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
            case Defines.ESkillCost.PercentMaxMP:
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
            case Defines.ESkillCost.PercentRemainMP:
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

    async Task CreateTargetingDecal(EffectInfo _effectInfo, Transform _trTarget)
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

                    objDecal.transform.position = _trTarget.position;
                    objDecal.transform.forward = _trTarget.forward;

                    objDecal.transform.SetParent(_trTarget.transform);

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

        await CreateTargetingTimeDecal(_effectInfo, _trTarget, objDecal);
    }

    async Task CreateTargetingTimeDecal(EffectInfo _effectInfo, Transform _trTarget, GameObject _areaDecal)
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

                    objDecal.transform.position = _trTarget.position;
                    objDecal.transform.forward = _trTarget.forward;

                    objDecal.transform.SetParent(_trTarget.transform);

                    objDecal.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                    var decalProjector = objDecal.GetComponent<DecalProjector>();
                    if (decalProjector != null)
                    {
                        float time = 0;

                        while (time <= _effectInfo.startDelay)
                        {
                            var curValue = Mathf.Lerp(0, _effectInfo.sphereRadius * 2f, time / _effectInfo.startDelay);
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

    async Task CreateNoneTargetingDecal(EffectInfo _effectInfo, Vector3 _posDecal, Vector3 _dirDecal)
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

        await CreateNoneTargetingTimeDecal(_effectInfo, _posDecal, _dirDecal, objDecal);
    }

    async Task CreateNoneTargetingTimeDecal(EffectInfo _effectInfo, Vector3 _posDecal, Vector3 _dirDecal, GameObject _areaDecal)
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

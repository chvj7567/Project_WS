using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Infomation;
using static UnityEngine.GraphicsBuffer;

public class CHMSkill
{
    GameObject roundAreaDecal = null;
    GameObject roundTimingDecal = null;

    public void CreateAISkill(Transform _trCaster, Transform _trTarget, Defines.ESkillID _skill)
    {
        var skillInfo = CHMMain.Json.GetSkillInfo(_skill);

        if (skillInfo != null)
        {
            // 타겟팅 스킬, 논타겟팅 스킬 구분
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
            foreach (var effectInfo in skillInfo.liEffectInfo)
            {
                // 스킬 시전 딜레이 시간 전에 데칼로 스킬 시전 구역 알려줌
                if (effectInfo.onDecal)
                {
                    await CreateTargetingDecal(effectInfo, _trTarget);
                }
                else
                {
                    await Task.Delay((int)(effectInfo.startDelay * 1000f));
                }

                List<Transform> liTarget = new List<Transform>();

                // 스킬 충돌 범위 생성
                CreateSkillCollision(_trCaster, _trTarget, effectInfo);
                /*switch (effectInfo.eCollision)
                {
                    case Defines.ECollision.Sphere:
                        switch (effectInfo.eStandardPos)
                        {
                            case Defines.EStandardPos.Me:
                                {
                                    CHMMain.Particle.CreateTargetingParticle(_trCaster, new List<Transform> { _trCaster }, effectInfo);
                                }
                                break;
                            case Defines.EStandardPos.Target_One:
                                {
                                    liTarget.Add(_trTarget);

                                    // Test
                                    foreach (var target in liTarget)
                                    {
                                        var unit = target.GetComponent<UnitBase>();
                                        if (unit != null)
                                        {
                                            unit.MinusHp(effectInfo.damage);
                                        }
                                    }
                                    // Test

                                    CHMMain.Particle.CreateTargetingParticle(_trCaster, liTarget, effectInfo);
                                }
                                break;
                            case Defines.EStandardPos.Target_All:
                                {
                                    var liTargetInfo = GetTargetInfoListInRange(_trTarget.position, _trTarget.position - _trCaster.position, GetTargetMask(effectInfo.eTargetMask), effectInfo.sphereRadius, effectInfo.angle);
                                    liTarget = GetTargetTransformList(liTargetInfo);

                                    // Test
                                    foreach (var target in liTarget)
                                    {
                                        var unit = target.GetComponent<UnitBase>();
                                        if (unit != null)
                                        {
                                            unit.MinusHp(effectInfo.damage);
                                        }
                                    }
                                    // Test

                                    CHMMain.Particle.CreateTargetingParticle(_trCaster, liTarget, effectInfo);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case Defines.ECollision.Box:
                        {
                            var liTargetInfo = GetTargetInfoListInRange(_trTarget.position, GetTargetMask(effectInfo.eTargetMask), new Vector3(effectInfo.boxHalfX, effectInfo.boxHalfY, effectInfo.boxHalfZ), _trCaster.rotation);
                            liTarget = GetTargetTransformList(liTargetInfo);
                        }
                        break;
                    default:
                        {
                            liTarget.Add(_trTarget);

                            CHMMain.Particle.CreateTargetingParticle(_trCaster, liTarget, effectInfo);
                        }
                        break;
                }*/
            }
        }
    }

    public async void CreateNoneTargetingSkill(Transform _trCaster, Vector3 _posSkill, Vector3 _dirSkill, Defines.ESkillID _skill)
    {
        var skillInfo = CHMMain.Json.GetSkillInfo(_skill);

        if (skillInfo != null)
        {
            foreach (var effectInfo in skillInfo.liEffectInfo)
            {
                // 스킬 시전 딜레이 시간 전에 데칼로 스킬 시전 구역 알려줌
                if (effectInfo.onDecal)
                {
                    await CreateNoneTargetingDecal(effectInfo, _posSkill, _dirSkill);
                }
                else
                {
                    await Task.Delay((int)(effectInfo.startDelay * 1000f));
                }

                List<Transform> liTarget = new List<Transform>();

                switch (effectInfo.eCollision)
                {
                    case Defines.ECollision.Sphere:
                        {
                            var liTargetInfo = GetTargetInfoListInRange(_posSkill, _dirSkill, GetTargetMask(effectInfo.eTargetMask), effectInfo.sphereRadius, effectInfo.angle);
                            liTarget = GetTargetTransformList(liTargetInfo);

                            // Test
                            foreach (var target in liTarget)
                            {
                                var unit = target.GetComponent<UnitBase>();
                                if (unit != null)
                                {
                                    unit.MinusHp(effectInfo.damage);
                                }
                            }
                            // Test

                            CHMMain.Particle.CreateNoneTargetingParticle(_posSkill, _dirSkill, effectInfo);
                        }
                        break;
                    case Defines.ECollision.Box:
                        break;
                    default:
                        {
                            CHMMain.Particle.CreateNoneTargetingParticle(_posSkill, _dirSkill, effectInfo);
                        }
                        break;
                }
            }
        }
    }

    void CreateSkillCollision(Transform _trCaster, Transform _trTarget, EffectInfo _effectInfo)
    {
        // Collision 모양에 따라 구분
        switch (_effectInfo.eCollision)
        {
            case Defines.ECollision.Sphere:
                {
                    CreateSphereCollision(_trCaster, _trTarget, _effectInfo);
                }
                break;
            case Defines.ECollision.Box:
                break;
            default:
                {
                    // Collision이 없으면 해당 지점에 파티클만 생성
                    CHMMain.Particle.CreateTargetingParticle(_trCaster, new List<Transform> { _trTarget }, _effectInfo);
                }
                break;
        }
    }

    void CreateSphereCollision(Transform _trCaster, Transform _trTarget, EffectInfo _effectInfo)
    {
        List<Transform> liTarget = new List<Transform>();

        // 스킬 시전 시 맞은 타겟들
        var liTargetInfo = GetTargetInfoListInRange(_trTarget.position, _trTarget.position - _trCaster.position, GetTargetMask(_effectInfo.eTargetMask), _effectInfo.sphereRadius, _effectInfo.angle);
        liTarget = GetTargetTransformList(liTargetInfo);

        switch (_effectInfo.eDamageState)
        {
            case Defines.EDamageState.AtOnce:
                {
                    foreach (var target in liTarget)
                    {
                        var unit = target.GetComponent<UnitBase>();
                        if (unit != null)
                        {
                            unit.MinusHp(_effectInfo.damage);
                        }
                    }
                }
                break;
            case Defines.EDamageState.Constantly_1Sec_3Count:
                {
                    foreach (var target in liTarget)
                    {
                        var unit = target.GetComponent<UnitBase>();
                        if (unit != null)
                        {
                            ConstantlyDamage(unit, 1f, 3f, _effectInfo.damage);
                        }
                    }
                }
                break;
            default:
                break;
        }
        

        // 파티클 생성 기준 위치에 따라 구분
        switch (_effectInfo.eStandardPos)
        {
            case Defines.EStandardPos.Me:
                {
                    CHMMain.Particle.CreateTargetingParticle(_trCaster, new List<Transform> { _trCaster }, _effectInfo);
                }
                break;
            case Defines.EStandardPos.Target_One:
                {
                    CHMMain.Particle.CreateTargetingParticle(_trCaster, new List<Transform> { _trTarget }, _effectInfo);
                }
                break;
            case Defines.EStandardPos.Target_All:
                {
                    CHMMain.Particle.CreateTargetingParticle(_trCaster, liTarget, _effectInfo);
                }
                break;
            default:
                break;
        }
    }

    async void ConstantlyDamage(UnitBase _unit, float _time, float _count, float _damage)
    {
        float tickTime = _time / _count;

        for (int i = 0; i < _count; ++i)
        {
            _unit.MinusHp(_damage);

            if (i == _count - 1)
            {
                break;
            }

            await Task.Delay((int)(tickTime * 1000f));
        }
    }

    public async Task CreateTargetingDecal(EffectInfo _effectInfo, Transform _trTarget)
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

    public async Task CreateTargetingTimeDecal(EffectInfo _effectInfo, Transform _trTarget, GameObject _areaDecal)
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

                        Debug.Log(time);

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

    public async Task CreateNoneTargetingDecal(EffectInfo _effectInfo, Vector3 _posDecal, Vector3 _dirDecal)
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

    public async Task CreateNoneTargetingTimeDecal(EffectInfo _effectInfo, Vector3 _posDecal, Vector3 _dirDecal, GameObject _areaDecal)
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

    public void GetNoneTargetingDecal(EffectInfo _effectInfo, Vector3 _posDecal, Vector3 _dirDecal)
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

                            objDecal = CHMMain.Resource.Instantiate(roundAreaDecal);
                        });
                    }
                    else
                    {
                        objDecal = CHMMain.Resource.Instantiate(roundAreaDecal);
                    }

                    objDecal.transform.position = _posDecal;
                    objDecal.transform.forward = _dirDecal;

                    var decalProjector = objDecal.GetComponent<DecalProjector>();
                    if (decalProjector != null)
                    {
                        decalProjector.size = new Vector3(20, 20, 20);
                    }
                }
                break;
            case Defines.ECollision.Box:
                break;
            default:
                break;
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
                    targetInfoList.Add(new TargetInfo
                    {
                        objTarget = target.gameObject,
                        direction = targetDir,
                        distance = targetDis,
                    });
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
            default:
                return -1;
        }
    }
}

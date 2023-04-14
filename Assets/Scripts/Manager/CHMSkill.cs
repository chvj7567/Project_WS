using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Infomation;

public class CHMSkill
{
    public void CreateAISkill(Transform _trCaster, Transform _trTarget, Defines.ESkillID _skill)
    {
        var skillInfo = CHMMain.Json.GetSkillInfo(_skill);

        if (skillInfo != null)
        {
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
            
            foreach (var effect in skillInfo.liEffectInfo)
            {
                // 스킬 시전 후 이펙트 딜레이 시간
                if (effect.startTime > 0)
                {
                    await Task.Delay((int)(effect.startTime * 1000f));
                }

                List<Transform> liTarget = new List<Transform>();

                switch (effect.eCollision)
                {
                    case Defines.ECollision.Sphere:
                        switch (effect.eStandardPos)
                        {
                            case Defines.EStandardPos.Me:
                                {
                                    CHMMain.Particle.CreateTargetingParticle(_trCaster, new List<Transform> { _trCaster }, effect.eStandardPos, effect.eEffect);
                                }
                                break;
                            case Defines.EStandardPos.TargetOne:
                                {
                                    liTarget.Add(_trTarget);

                                    // Test
                                    foreach (var target in liTarget)
                                    {
                                        var unit = target.GetComponent<UnitBase>();
                                        unit.MinusHp(effect.damage);
                                    }
                                    // Test

                                    CHMMain.Particle.CreateTargetingParticle(_trCaster, liTarget, effect.eStandardPos, effect.eEffect);
                                }
                                break;
                            case Defines.EStandardPos.TargetAll:
                                {
                                    var liTargetInfo = GetTargetInfoListInRange(_trTarget.position, _trTarget.position - _trCaster.position, GetTargetMask(effect.eTargetMask), effect.sphereRadius, effect.angle);
                                    liTarget = GetTargetTransformList(liTargetInfo);

                                    // Test
                                    foreach (var target in liTarget)
                                    {
                                        var unit = target.GetComponent<UnitBase>();
                                        unit.MinusHp(effect.damage);
                                    }
                                    // Test

                                    CHMMain.Particle.CreateTargetingParticle(_trCaster, liTarget, effect.eStandardPos, effect.eEffect);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case Defines.ECollision.Box:
                        {
                            var liTargetInfo = GetTargetInfoListInRange(_trTarget.position, GetTargetMask(effect.eTargetMask), new Vector3(effect.boxHalfX, effect.boxHalfY, effect.boxHalfZ), _trCaster.rotation);
                            liTarget = GetTargetTransformList(liTargetInfo);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public async void CreateNoneTargetingSkill(Transform _trCaster, Vector3 _posSkill, Vector3 _dirSkill, Defines.ESkillID _skill)
    {
        var skillInfo = CHMMain.Json.GetSkillInfo(_skill);

        if (skillInfo != null)
        {
            foreach (var effect in skillInfo.liEffectInfo)
            {
                // 스킬 시전 후 이펙트 딜레이 시간
                if (effect.startTime > 0)
                {
                    await Task.Delay((int)(effect.startTime * 1000f));
                }

                List<Transform> liTarget = new List<Transform>();

                switch (effect.eCollision)
                {
                    case Defines.ECollision.Sphere:
                        {
                            var liTargetInfo = GetTargetInfoListInRange(_posSkill, _dirSkill, GetTargetMask(effect.eTargetMask), effect.sphereRadius, effect.angle);
                            liTarget = GetTargetTransformList(liTargetInfo);

                            // Test
                            foreach (var target in liTarget)
                            {
                                var unit = target.GetComponent<UnitBase>();
                                unit.MinusHp(effect.damage);
                            }
                            // Test

                            CHMMain.Particle.CreateNoneTargetingParticle(_posSkill, _dirSkill, effect.eEffect);
                        }
                        break;
                    case Defines.ECollision.Box:
                        break;
                    default:
                        break;
                }
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
                    targetInfoList.Add(new TargetInfo
                    {
                        targetObj = target.gameObject,
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
                    targetObj = target.gameObject,
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
            targetTransformList.Add(targetInfo.targetObj.transform);
        }

        return targetTransformList;
    }

    public List<Transform> GetTargetTransformList(TargetInfo _targetInfo)
    {
        if (_targetInfo == null) return null;

        List<Transform> targetTransformList = new List<Transform>();
        targetTransformList.Add(_targetInfo.targetObj.transform);

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

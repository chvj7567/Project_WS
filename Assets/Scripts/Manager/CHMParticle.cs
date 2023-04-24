using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using static Infomation;
using UniRx;

public class CHMParticle
{
    Dictionary<Defines.EEffect, Infomation.ParticleInfo> dicParticleInfo = new Dictionary<Defines.EEffect, Infomation.ParticleInfo>();

    public void CreateParticle(Transform _trCaster, List<Transform> _liTarget, List<Vector3> _liParticlePos, List<Vector3> _liParticleDir, EffectInfo _effectInfo, bool _autoDestory = true)
    {
        if (_trCaster == null)
        {
            Debug.Log("No Caster");
            return;
        }

        List<GameObject> liParticle = new List<GameObject>();

        // 타겟이 없다는 건 논타겟 스킬이라는 것
        if (_liTarget == null || _liTarget.Count == 0 || _liTarget.First() == null)
        {
            _liTarget = null;

            // 타겟 생성할 위치 수 만큼 파티클 생성 
            for (int i = 0; i < _liParticlePos.Count; ++i)
            {
                var tempParticle = CHMMain.Particle.GetParticleObject(_effectInfo.eEffect, _autoDestory);
                var sphereCollision = tempParticle.GetOrAddComponent<CHSphereCollision>();
                sphereCollision.Init(_effectInfo.sphereRadius, _effectInfo.stayTickTime);

                if (_effectInfo.triggerEnter)
                {
                    sphereCollision.TriggerEnterCallback(sphereCollision.OnEnter.Subscribe(_ =>
                    {
                        var targetMask = CHMMain.Skill.GetTargetMask(_trCaster.gameObject.layer, _effectInfo.eTargetMask);
                        
                        if ((1 << _.gameObject.layer & targetMask.value) != 0)
                        {
                            Debug.Log($"TriggerEnter : {_.name}");
                            CHMMain.Skill.ApplySkillValue(_trCaster, new List<Transform> { _ }, _effectInfo);

                            SetParticleTriggerValue(_, _effectInfo);
                        }
                    }));
                }

                if (_effectInfo.triggerExit)
                {
                    sphereCollision.TriggerExitCallback(sphereCollision.OnExit.Subscribe(_ =>
                    {
                        var targetMask = CHMMain.Skill.GetTargetMask(_trCaster.gameObject.layer, _effectInfo.eTargetMask);

                        if ((1 << _.gameObject.layer & targetMask.value) != 0)
                        {
                            Debug.Log($"TriggerExit : {_.name}");
                            CHMMain.Skill.ApplySkillValue(_trCaster, new List<Transform> { _ }, _effectInfo);
                        }
                    }));
                }

                sphereCollision.TriggerStayCallback(sphereCollision.OnStay.Subscribe(_ =>
                {
                    var targetMask = CHMMain.Skill.GetTargetMask(_trCaster.gameObject.layer, _effectInfo.eTargetMask);

                    if ((1 << _.gameObject.layer & targetMask.value) != 0)
                    {
                        Debug.Log($"TriggerStay : {_.name}");
                        CHMMain.Skill.ApplySkillValue(_trCaster, new List<Transform> { _ }, _effectInfo);
                    }
                }));

                if (tempParticle == null)
                {
                    Debug.Log("No Particle");
                    return;
                }

                liParticle.Add(tempParticle);
            }

            for (int i = 0; i < liParticle.Count; ++i)
            {
                var objParticle = liParticle.ElementAtOrDefault(i);
                var posParticle = _liParticlePos.ElementAtOrDefault(i);
                var dirParticle = _liParticleDir.ElementAtOrDefault(i);

                objParticle.transform.position = posParticle;
                objParticle.transform.forward = dirParticle;

                SetParticleValue(_trCaster, null, posParticle, dirParticle, objParticle, _effectInfo);
            }
        }
        else
        {
            // 타겟의 수 만큼 파티클 생성 
            for (int i = 0; i < _liTarget.Count; ++i)
            {
                var tempParticle = CHMMain.Particle.GetParticleObject(_effectInfo.eEffect, _autoDestory);

                if (tempParticle == null)
                {
                    Debug.Log("No Particle");
                    return;
                }

                liParticle.Add(tempParticle);
            }

            for (int i = 0; i < liParticle.Count; ++i)
            {
                var objParticle = liParticle.ElementAtOrDefault(i);
                var trTarget = _liTarget.ElementAtOrDefault(i);

                objParticle.transform.position = trTarget.position;
                objParticle.transform.forward = trTarget.forward;

                objParticle.transform.SetParent(trTarget);

                SetParticleValue(_trCaster, trTarget, Vector3.zero, Vector3.zero, objParticle, _effectInfo);
            }
        }
    }

    public GameObject GetParticleObject(Defines.EEffect _eEffect, bool _autoDestory = true)
    {
        GameObject objParticle = null;

        if (dicParticleInfo.ContainsKey(_eEffect) == false)
        {
            CHMMain.Resource.InstantiateEffect(_eEffect, (particle) =>
            {
                particle.SetActive(false);
                if (IsPoolableEffect(_eEffect) == true)
                {
                    particle.GetOrAddComponent<CHPoolable>();
                }

                dicParticleInfo.Add(_eEffect, new Infomation.ParticleInfo
                {
                    objParticle = particle,
                    time = GetParticleTime(particle)
                });

                objParticle = CHMMain.Resource.Instantiate(dicParticleInfo[_eEffect].objParticle);

                if (_autoDestory)
                {
                    DestroyParticle(_eEffect, objParticle);
                }
            });
        }
        else
        {
            objParticle = CHMMain.Resource.Instantiate(dicParticleInfo[_eEffect].objParticle);

            if (_autoDestory)
            {
                DestroyParticle(_eEffect, objParticle);
            }
        }

        return objParticle;
    }

    public float GetParticleTime(GameObject _particleObj)
    {
        float time = -1;

        var arrParticle = _particleObj.GetComponentsInChildren<ParticleSystem>();

        foreach (var particle in arrParticle)
        {
            time = Mathf.Max(time, particle.main.duration);
        }

        return time;
    }

    //-------------------------------------- private ------------------------------------------//

    async void DestroyParticle(Defines.EEffect _eEffect, GameObject _objParticle)
    {
        await Task.Delay((int)(dicParticleInfo[_eEffect].time * 1000));

        if (_objParticle) CHMMain.Resource.Destroy(_objParticle);
    }

    bool IsPoolableEffect(Defines.EEffect _eEffect)
    {
        switch (_eEffect)
        {
            // 움직이는 애들은 풀링 사용시 순간이동하는 이슈가 있음
            case Defines.EEffect.FX_Tornado:
                return false;
            default:
                return true;
        }
    }

    void SetParticleValue(Transform _trCaster, Transform _trTarget, Vector3 _posSkill, Vector3 _dirSkill, GameObject _objParticle, EffectInfo _effectInfo)
    {
        // 각 이펙트별로 세부 설정이 필요한 경우
        switch (_effectInfo.eEffect)
        {
            case Defines.EEffect.FX_Circle_meteor:
                {
                    // y축으로 20 이동
                    var posOrigin = _objParticle.transform.localPosition;
                    _objParticle.transform.localPosition = new Vector3(posOrigin.x, posOrigin.y + 20f, posOrigin.z);
                }
                break;
            case Defines.EEffect.FX_Tornado:
                {
                    Move(_objParticle, _dirSkill, 10f, dicParticleInfo[_effectInfo.eEffect].time);
                }
                break;
            default:
                break;
        }
    }

    void SetParticleTriggerValue(Transform _trTarget, EffectInfo _effectInfo)
    {
        // 각 이펙트에 트리거 된 타겟들 처리
        switch (_effectInfo.eEffect)
        {
            case Defines.EEffect.FX_Tornado:
                {
                    Airborne(_trTarget, 2, 0.5f);
                }
                break;
            case Defines.EEffect.FX_Explosion:
                {
                    Airborne(_trTarget, 5, 0.5f);
                }
                break;
            default:
                break;
        }
    }

    async void Move(GameObject _objParticle, Vector3 _direction, float _speed, float _effectTime)
    {
        var posOrigin = _objParticle.transform.position;

        float time = 0;
        while (time <= _effectTime)
        {
            if (_objParticle == null)
            {
                break;
            }
            _objParticle.transform.position = posOrigin + _direction.normalized * _speed * time;
            time += Time.deltaTime;
            await Task.Delay((int)(Time.deltaTime * 1000f));
        }
    }
    async void Airborne(Transform _trTarget, float _airborneHeight, float _airborneTime)
    {
        var unitBase = _trTarget.GetComponent<CHUnitBase>();
        float gravity = -2 * _airborneHeight / Mathf.Pow(_airborneTime, 2);
        float airborneVelocity = -gravity * _airborneTime;
        Vector3 startPos = _trTarget.position;

        float time = 0f;

        // 위로 떠오르는 코드
        while (time <= _airborneTime)
        {
            if (unitBase.GetIsDeath()) break;

            unitBase.SetIsAirborne(true);
            float height = startPos.y + (airborneVelocity * time) + (0.5f * gravity * Mathf.Pow(time, 2));
            _trTarget.position = new Vector3(_trTarget.position.x, height, _trTarget.position.z);
            time += Time.deltaTime;
            await Task.Delay((int)(Time.deltaTime * 1000f));
        }

        unitBase.SetIsAirborne(false);
        float fallSpeed = 9.8f;
        float groundLevel = 0f;
        Vector3 fallVector = Vector3.zero;

        // 아래로 떨어지는 코드
        while (_trTarget.position.y > groundLevel)
        {
            if (unitBase.GetIsAirborne() || unitBase.GetIsDeath()) break;
            fallVector.y -= fallSpeed * Time.deltaTime;
            _trTarget.position += fallVector * Time.deltaTime;
            await Task.Delay((int)(Time.deltaTime * 1000f));
        }

        _trTarget.position = new Vector3(_trTarget.position.x, 0f, _trTarget.position.z);
    }
}
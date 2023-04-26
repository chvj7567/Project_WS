using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using UniRx;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Threading;
using static Infomation;
using Unity.VisualScripting;

public class MyEvent : UnityEvent { }

public class CHMParticle
{
    Dictionary<Defines.EEffect, ParticleInfo> dicParticleInfo = new Dictionary<Defines.EEffect, Infomation.ParticleInfo>();

    MyEvent OnApplicationQuitEvent = new MyEvent();
    CancellationTokenSource cts;

    public void Init()
    {
        cts = new CancellationTokenSource();

        OnApplicationQuitEvent.AddListener(OnApplicationQuitHandler);
    }

    void OnApplicationQuitHandler()
    {
        if (cts != null && !cts.IsCancellationRequested)
        {
            cts.Cancel();
        }
    }

    public void CreateParticle(Transform _trCaster, List<Transform> _liTarget, List<Vector3> _liParticlePos, List<Vector3> _liParticleDir, EffectInfo _effectInfo, bool _autoDestroy = true)
    {
        if (_trCaster == null)
        {
            Debug.Log("No Caster");
            return;
        }

        List<GameObject> liParticle = new List<GameObject>();

        // Ÿ���� ���ٴ� �� ��Ÿ�� ��ų�̶�� ��
        if (_liTarget == null || _liTarget.Count == 0 || _liTarget.First() == null)
        {
            _liTarget = null;

            // Ÿ�� ������ ��ġ �� ��ŭ ��ƼŬ ���� 
            for (int i = 0; i < _liParticlePos.Count; ++i)
            {
                var objParticle = CHMMain.Particle.GetParticleObject(_effectInfo.eEffect, _autoDestroy);

                SetParticleCollision(_trCaster, _effectInfo, objParticle);

                if (objParticle == null)
                {
                    Debug.Log("No Particle");
                    return;
                }

                liParticle.Add(objParticle);
            }

            for (int i = 0; i < liParticle.Count; ++i)
            {
                var objParticle = liParticle.ElementAtOrDefault(i);
                var posParticle = _liParticlePos.ElementAtOrDefault(i);
                var dirParticle = _liParticleDir.ElementAtOrDefault(i);

                posParticle.y = 0f;
                dirParticle.y = 0f;

                objParticle.transform.position = posParticle;
                objParticle.transform.forward = dirParticle;
                objParticle.transform.forward = objParticle.transform.Angle(_effectInfo.effectAngle);

                SetParticleValue(_trCaster, null, posParticle, dirParticle, objParticle, _effectInfo);
            }
        }
        else
        {
            // Ÿ���� �� ��ŭ ��ƼŬ ���� 
            for (int i = 0; i < _liTarget.Count; ++i)
            {
                var objParticle = CHMMain.Particle.GetParticleObject(_effectInfo.eEffect, _autoDestroy);

                SetParticleCollision(_trCaster, _effectInfo, objParticle);

                if (objParticle == null)
                {
                    Debug.Log("No Particle");
                    return;
                }

                liParticle.Add(objParticle);
            }

            for (int i = 0; i < liParticle.Count; ++i)
            {
                var objParticle = liParticle.ElementAtOrDefault(i);
                var trTarget = _liTarget.ElementAtOrDefault(i);

                var posParticle = trTarget.position;
                var dirParticle = _trCaster.forward;

                posParticle.y = 0f;
                dirParticle.y = 0f;

                objParticle.transform.position = posParticle;
                objParticle.transform.forward = dirParticle;
                objParticle.transform.forward = objParticle.transform.Angle(_effectInfo.effectAngle);

                objParticle.transform.SetParent(trTarget);

                SetParticleValue(_trCaster, trTarget, Vector3.zero, Vector3.zero, objParticle, _effectInfo);
            }

            CHMMain.Skill.ApplySkillValue(_trCaster, _liTarget, _effectInfo);
        }
    }

    void SetParticleCollision(Transform _trCaster, EffectInfo _effectInfo, GameObject _objParticle)
    {
        switch (_effectInfo.eEffect)
        {
            default:
                {
                    // �Ϲ������� ��ƼŬ �θ� ������Ʈ���� �ݸ����� ����
                    ApplyShereCollision(_trCaster, _effectInfo, _objParticle);
                }
                break;
        }
    }

    void ApplyShereCollision(Transform _trCaster, EffectInfo _effectInfo, GameObject _objParticle)
    {
        var sphereCollision = _objParticle.GetOrAddComponent<CHSphereCollision>();
        sphereCollision.Init(_effectInfo.sphereRadius, _effectInfo.stayTickTime);
        SetCollisionCenter(sphereCollision, _effectInfo);

        if (_effectInfo.triggerEnter)
        {
            sphereCollision.TriggerEnterCallback(sphereCollision.OnEnter.Subscribe(_ =>
            {
                var targetMask = CHMMain.Skill.GetTargetMask(_trCaster.gameObject.layer, _effectInfo.eTargetMask);

                if ((1 << _.gameObject.layer & targetMask.value) != 0)
                {
                    Debug.Log($"TriggerEnter : {_.name}");
                    CHMMain.Skill.ApplySkillValue(_trCaster, new List<Transform> { _ }, _effectInfo);

                    SetParticleTriggerValue(_trCaster, _, _effectInfo);
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

                    SetParticleTriggerValue(_trCaster, _, _effectInfo);
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

                SetParticleTriggerValue(_trCaster, _, _effectInfo);
            }
        }));
    }

    void SetCollisionCenter(CHSphereCollision _collision, EffectInfo _effectInfo)
    {
        switch (_effectInfo.eEffect)
        {
            case Defines.EEffect.FX_Arrow_impact:
                _collision.SetCollisionCenter(0f, 4f, 0f);
                break;
            default:
                _collision.SetCollisionCenter(0f, 0f, 0f);
                break;
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

        objParticle.SetActive(true);

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
            // �����̴� �ֵ��� Ǯ�� ���� �����̵��ϴ� �̽��� ����
            case Defines.EEffect.FX_Tornado:
                return false;
            default:
                return true;
        }
    }

    void SetParticleValue(Transform _trCaster, Transform _trTarget, Vector3 _posSkill, Vector3 _dirSkill, GameObject _objParticle, EffectInfo _effectInfo)
    {
        // �� ����Ʈ���� ���� ������ �ʿ��� ���
        switch (_effectInfo.eEffect)
        {
            case Defines.EEffect.FX_Circle_meteor:
                {
                    // y������ 20 �̵�
                    var posOrigin = _objParticle.transform.localPosition;
                    _objParticle.transform.localPosition = new Vector3(posOrigin.x, posOrigin.y + 20f, posOrigin.z);
                }
                break;
            case Defines.EEffect.FX_Arrow_impact:
                {
                    // y������ -3 �̵�
                    var posOrigin = _objParticle.transform.localPosition;
                    _objParticle.transform.localPosition = new Vector3(posOrigin.x, posOrigin.y - 3f, posOrigin.z);
                    ParticleMove(_trCaster, _trTarget, 30f, dicParticleInfo[_effectInfo.eEffect].time, _objParticle);
                }
                break;
            case Defines.EEffect.FX_Arrow_impact_sub:
                {
                    // y������ -3 �̵�
                    var posOrigin = _objParticle.transform.localPosition;
                    _objParticle.transform.localPosition = new Vector3(posOrigin.x, posOrigin.y - 3f, posOrigin.z);
                }
                break;
            case Defines.EEffect.FX_Tornado:
                {
                    ParticleMove(_trCaster, _trTarget, 10f, dicParticleInfo[_effectInfo.eEffect].time, _objParticle);
                }
                break;
            default:
                break;
        }
    }

    void SetParticleTriggerValue(Transform _trCaster, Transform _trTarget, EffectInfo _effectInfo)
    {
        // �� ����Ʈ�� Ʈ���� �� Ÿ�ٵ� ó��
        switch (_effectInfo.eEffect)
        {
            case Defines.EEffect.FX_Tornado:
                {
                    TargetAirborne(cts.Token, _trTarget, 2, 0.5f);
                }
                break;
            case Defines.EEffect.FX_Explosion:
                {
                    TargetAirborne(cts.Token, _trTarget, 10, 2f);
                }
                break;
            default:
                break;
        }
    }

    async void ParticleMove(Transform _trCaster, Transform _trTarget, float _speed, float _effectTime, GameObject _objParticle)
    {
        Vector3 posOrigin = _objParticle.transform.localPosition;
        Vector3 direction = _objParticle.transform.forward;

        float time = 0;
        
        if (_trTarget == null)
        {
            while (time <= _effectTime)
            {
                if (_objParticle == null)
                {
                    break;
                }
                _objParticle.transform.localPosition = posOrigin + direction.normalized * _speed * time;
                time += Time.deltaTime;
                await Task.Delay((int)(Time.deltaTime * 1000f));
            }
        }
        else
        {
            while (time <= _effectTime)
            {
                if (_objParticle == null)
                {
                    break;
                }
                _objParticle.transform.localPosition = posOrigin + (_trTarget.position - _trCaster.position).normalized * _speed * time;
                time += Time.deltaTime;
                await Task.Delay((int)(Time.deltaTime * 1000f));
            }
        }
    }

    async void TargetAirborne(CancellationToken _token, Transform _trTarget, float _airborneHeight, float _airborneTime)
    {
        var unitBase = _trTarget.GetComponent<CHUnitBase>();
        float gravity = -2 * _airborneHeight / Mathf.Pow(_airborneTime, 2);
        float airborneVelocity = -gravity * _airborneTime;
        Vector3 startPos = _trTarget.position;

        var agent = _trTarget.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        // Ÿ�� ���� ��� ���� üũ
        unitBase.SetIsAirborne(true);
        unitBase.IsFalling = false;

        float time = 0f;
        // ���� �������� �ڵ�
        while (!_token.IsCancellationRequested && time <= _airborneTime)
        {
            try
            {
                if (_trTarget == null || unitBase.GetIsDeath()) break;
                float height = startPos.y + (airborneVelocity * time) + (0.5f * gravity * Mathf.Pow(time, 2));
                _trTarget.position = new Vector3(_trTarget.position.x, height, _trTarget.position.z);
                time += Time.deltaTime;
                await Task.Delay((int)(Time.deltaTime * 1000f));
            }
            catch (TaskCanceledException)
            {
                Debug.Log("��� �������� ���� ����");
            }
        }

        unitBase.IsFalling = true;

        float fallSpeed = 9.8f;
        float groundLevel = 0f;
        Vector3 fallVector = Vector3.zero;

        // �Ʒ��� �������� �ڵ�
        while (!_token.IsCancellationRequested && _trTarget != null && _trTarget.position.y > groundLevel)
        {
            try
            {
                if (unitBase.IsFalling == false) break;
                fallVector.y -= fallSpeed * Time.deltaTime;
                _trTarget.position += fallVector * Time.deltaTime;
                await Task.Delay((int)(Time.deltaTime * 1000f));
            }
            catch (TaskCanceledException)
            {
                Debug.Log("��� �������� ���� ����");
            }
        }

        if (_trTarget != null && unitBase != null && unitBase.IsFalling)
        {
            unitBase.IsFalling = false;
            unitBase.SetIsAirborne(false);
            _trTarget.position = new Vector3(_trTarget.position.x, 0f, _trTarget.position.z);

            if(agent != null)
            {
                agent.enabled = true;
            }
        }
    }
}
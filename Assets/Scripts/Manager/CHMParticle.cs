using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using UniRx;
using UnityEngine.AI;
using System.Threading;
using Unity.VisualScripting;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CHMParticle
{
    Dictionary<Defines.EEffect, float> dicParticleTime = new Dictionary<Defines.EEffect, float>();

    CancellationTokenSource cts;
    CancellationToken token;

    public void Init()
    {
        cts = new CancellationTokenSource();
        token = cts.Token;

#if UNITY_EDITOR
        EditorApplication.quitting -= OnApplicationQuitHandler;
        EditorApplication.quitting += OnApplicationQuitHandler;
#else
        Application.quitting -= OnApplicationQuitHandler;
        Application.quitting += OnApplicationQuitHandler;
#endif
    }

    public void Clear()
    {
        dicParticleTime.Clear();
        OnApplicationQuitHandler();
    }

    public void OnApplicationQuitHandler()
    {
        if (cts != null && !cts.IsCancellationRequested)
        {
            cts.Cancel();
        }
    }

    public void CreateParticle(Transform _trCaster, List<Transform> _liTarget, List<Vector3> _liParticlePos, List<Vector3> _liParticleDir, SkillData.EffectData _effectData)
    {
        if (_trCaster == null)
        {
            Debug.Log("No Caster");
            return;
        }

        if (_liTarget == null)
        {
            Debug.Log("No Target");
            return;
        }

        if (_liParticlePos == null || _liParticleDir == null)
        {
            Debug.Log("No Skill Info");
            return;
        }

        List<GameObject> liParticle = new List<GameObject>();

        // 타겟의 수 만큼 파티클 생성 
        for (int i = 0; i < _liTarget.Count; ++i)
        {
            var objParticle = GetParticleObject(_effectData.eEffect);

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
            var posParticle = _liParticlePos.ElementAtOrDefault(i);
            var dirParticle = _liParticleDir.ElementAtOrDefault(i);

            posParticle.y = 0f;
            dirParticle.y = 0f;

            objParticle.transform.position = posParticle;

            // 파티클의 경우 z축이 정면이 되도록
            objParticle.transform.forward = dirParticle;
            objParticle.transform.forward = objParticle.transform.Angle(_effectData.effectAngle, Defines.EStandardAxis.Z);

            SetParticlePositionValue(_trCaster, trTarget, objParticle, _effectData);
            SetParticleCollision(_trCaster, trTarget, objParticle, _effectData);
        }
    }

    public bool IsTarget(int _casterLayer, int _targetLayer, Defines.ETargetMask _targetMask)
    {
        var targetMask = CHMMain.Skill.GetTargetMask(_casterLayer, _targetMask);

        return (1 << _targetLayer & targetMask.value) != 0;
    }

    public GameObject GetParticleObject(Defines.EEffect _eEffect)
    {
        GameObject objParticle = null;

        CHMMain.Resource.InstantiateEffect(_eEffect, (_) =>
        {
            if (_ == null) return;

            objParticle = _;

            if (dicParticleTime.ContainsKey(_eEffect) == false)
            {
                dicParticleTime.Add(_eEffect, GetParticleTime(objParticle));
            }

            if (IsAutoDestroy(_eEffect))
            {
                DestroyParticle(_eEffect, objParticle);
            }
        });

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
        await Task.Delay((int)(dicParticleTime[_eEffect] * 1000));

        if (_objParticle) CHMMain.Resource.Destroy(_objParticle);
    }

    async void SetParticleCollision(Transform _trCaster, Transform _trTarget, GameObject _objParticle, SkillData.EffectData _effectData)
    {
        if (_effectData.eCollision != Defines.ECollision.None)
        {
            await Task.Delay((int)(_effectData.triggerStartDelay * 1000f));

            if (cts.IsCancellationRequested) return;

            var sphereCollision = _objParticle.GetOrAddComponent<CHSphereCollision>();
            sphereCollision.Init(_trCaster, _effectData);
            sphereCollision.sphereCollider.enabled = true;

            if (_effectData.triggerEnter)
            {
                sphereCollision.TriggerEnterCallback(sphereCollision.OnEnter.Subscribe(collider =>
                {
                    if (_trCaster == null || _trTarget == null || _objParticle == null) return;

                    if (IsTarget(_trCaster.gameObject.layer, collider.gameObject.layer, _effectData.eTargetMask))
                    {
                        SetParticleTriggerValue(_trCaster, _trTarget, collider.transform, _objParticle, _effectData);
                    }
                }));
            }

            if (_effectData.triggerExit)
            {
                sphereCollision.TriggerExitCallback(sphereCollision.OnExit.Subscribe(collider =>
                {
                    if (_trCaster == null || _trTarget == null || _objParticle == null) return;

                    if (IsTarget(_trCaster.gameObject.layer, collider.gameObject.layer, _effectData.eTargetMask))
                    {
                        SetParticleTriggerValue(_trCaster, _trTarget, collider.transform, _objParticle, _effectData);
                    }
                }));
            }

            sphereCollision.TriggerStayCallback(sphereCollision.OnStay.Subscribe(collider =>
            {
                if (_trCaster == null || _trTarget == null || _objParticle == null) return;

                if (IsTarget(_trCaster.gameObject.layer, collider.gameObject.layer, _effectData.eTargetMask))
                {
                    SetParticleTriggerValue(_trCaster, _trTarget, collider.transform, _objParticle, _effectData);
                }
            }));

            if (_effectData.triggerStayTime >= 0f)
            {
                await Task.Delay((int)(_effectData.triggerStayTime * 1000f));

                if (cts.IsCancellationRequested) return;

                if (sphereCollision != null && sphereCollision.sphereCollider != null) sphereCollision.sphereCollider.enabled = false;
            }
        }
    }

    bool IsAutoDestroy(Defines.EEffect _eEffect)
    {
        switch (_eEffect)
        {
            case Defines.EEffect.FX_Ax:
                return false;
            default:
                return true;
        }
    }

    async void SetParticlePositionValue(Transform _trCaster, Transform _trTarget, GameObject _objParticle, SkillData.EffectData _effectData)
    {
        // 이펙트가 붙어있어야하는 경우 SetParent를 해버리면 해당 타겟은 충돌체에 감지가 안되므로 타겟을 따라다니도록 수정
        if (_effectData.attach)
        {
            await MoveTrasnform(_objParticle.transform, _trTarget, -1f, dicParticleTime[_effectData.eEffect], 0f, _objParticle);
        }

        // 각 이펙트별로 위치 설정이 필요한 경우
        switch (_effectData.eEffect)
        {
            case Defines.EEffect.Slash:
                {
                    var posOrigin = _objParticle.transform.position;
                    _objParticle.transform.position = new Vector3(posOrigin.x, posOrigin.y - 2.2f, posOrigin.z);
                }
                break;
            case Defines.EEffect.FX_Circle_meteor:
            case Defines.EEffect.FX_Arrow_impact2:
                {
                    var posOrigin = _objParticle.transform.position;
                    _objParticle.transform.position = new Vector3(posOrigin.x, posOrigin.y + 23f, posOrigin.z);
                }
                break;
            case Defines.EEffect.FX_Arrow_impact:
                {
                    await MoveDirection(_objParticle.transform.forward, 30f, dicParticleTime[_effectData.eEffect], _objParticle);
                }
                break;
            case Defines.EEffect.FX_Ax:
                {
                    var posOrigin = _objParticle.transform.position;
                    _objParticle.transform.position = new Vector3(posOrigin.x, posOrigin.y + 3f, posOrigin.z);

                    await MoveDirection(_objParticle.transform.forward, 30f, 1f, _objParticle);
                    await MoveTrasnform(_objParticle.transform, _trCaster, 30f, -1f, 3f, _objParticle);

                    CHMMain.Resource.Destroy(_objParticle);
                }
                break;
            case Defines.EEffect.FX_Tornado:
                {
                    await MoveDirection(_objParticle.transform.forward, 10f, dicParticleTime[_effectData.eEffect], _objParticle);
                }
                break;
            case Defines.EEffect.FX_Explosion_Hit:
                {
                    var posOrigin = _objParticle.transform.position;
                    _objParticle.transform.position = new Vector3(posOrigin.x, posOrigin.y - 0.95f, posOrigin.z);
                }
                break;
        }
    }

    void SetParticleTriggerValue(Transform _trCaster, Transform _trTarget, Transform _trTriggerTarget, GameObject _objParticle, SkillData.EffectData _effectData)
    {
        // 각 이펙트에 트리거 된 타겟들 관련 처리
        switch (_effectData.eEffect)
        {
            case Defines.EEffect.FX_Healing:
                {
                    // 적들의 수 만큼만 힐링
                    if (_trCaster.gameObject.layer == _trTriggerTarget.gameObject.layer) return;

                    CHMMain.Skill.ApplySkillValue(_trCaster, new List<Transform> { _trCaster }, _effectData);
                }
                break;
            case Defines.EEffect.FX_Tornado:
                {
                    TargetAirborne(_trTriggerTarget);

                    CHMMain.Skill.ApplySkillValue(_trCaster, new List<Transform> { _trTriggerTarget }, _effectData);
                }
                break;
            case Defines.EEffect.FX_Arrow_impact:
                {
                    _objParticle.SetActive(false);

                    CHMMain.Skill.ApplySkillValue(_trCaster, new List<Transform> { _trTriggerTarget }, _effectData);
                }
                break;
            default:
                {
                    CHMMain.Skill.ApplySkillValue(_trCaster, new List<Transform> { _trTriggerTarget }, _effectData);
                }
                break;
        }
    }

    async Task MoveDirection(Vector3 _direction, float _speed, float _effectTime, GameObject _objParticle)
    {
        if (_direction != null)
        {
            float time = 0;
            while (!token.IsCancellationRequested && time <= _effectTime)
            {
                try
                {
                    if (_objParticle == null)
                    {
                        break;
                    }

                    _objParticle.transform.position += _direction.normalized * _speed * Time.deltaTime;

                    time += Time.deltaTime;
                    await Task.Delay((int)(Time.deltaTime * 1000f));
                }
                catch (TaskCanceledException)
                {

                }
            }
        }
    }

    async Task MoveTrasnform(Transform _trStart, Transform _trEnd, float _speed, float _effectTime, float _offset, GameObject _objParticle)
    {
        if (_trStart != null && _trEnd != null)
        {
            float time = 0;

            // 일정한 속도로 일정 시간 동안 타겟에게 다가감
            if (_speed >= 0 && _effectTime >= 0)
            {
                while (!token.IsCancellationRequested && time <= _effectTime)
                {
                    try
                    {
                        if (_trStart == null || _trEnd == null || _objParticle == null) break;

                        var direction = _trEnd.position - _trStart.position;
                        direction.y = 0f;

                        _objParticle.transform.forward = direction;
                        _objParticle.transform.position += direction.normalized * _speed * Time.deltaTime;

                        var posParticle = _objParticle.transform.position;
                        var posEnd = _trEnd.position;
                        posParticle.y = 0f;
                        posEnd.y = 0f;

                        if (Vector3.Distance(posParticle, posEnd) <= _offset) break;

                        time += Time.deltaTime;
                        await Task.Delay((int)(Time.deltaTime * 1000f));
                    }
                    catch (TaskCanceledException)
                    {

                    }
                }
            }
            // 일정한 속도로 타겟에게 offset 거리가 될 때까지 다가감
            else if (_speed >= 0 && _effectTime < 0)
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (_trStart == null || _trEnd == null || _objParticle == null) break;

                        var direction = _trEnd.position - _trStart.position;
                        direction.y = 0f;

                        _objParticle.transform.forward = direction;
                        _objParticle.transform.position += direction.normalized * _speed * Time.deltaTime;

                        var posParticle = _objParticle.transform.position;
                        var posEnd = _trEnd.position;
                        posParticle.y = 0f;
                        posEnd.y = 0f;

                        if (Vector3.Distance(posParticle, posEnd) <= _offset) break;

                        await Task.Delay((int)(Time.deltaTime * 1000f));
                    }
                    catch (TaskCanceledException)
                    {

                    }
                }
            }
            // 타겟에게 일정 시간동안 정해진 거리를 유지한채 붙어있음
            else if (_speed < 0 && _effectTime >= 0)
            {
                while (!token.IsCancellationRequested && time <= _effectTime)
                {
                    try
                    {
                        if (_trStart == null || _trEnd == null || _objParticle == null) break;

                        var direction = (_trEnd.position - _trStart.position).normalized;
                        direction.y = 0f;

                        var posParticle = _objParticle.transform.position;
                        var posEnd = _trEnd.position;
                        posParticle.y = 0f;
                        posEnd.y = 0f;

                        var distance = Vector3.Distance(posParticle, posEnd);

                        _objParticle.transform.forward = direction;
                        _objParticle.transform.position += direction * (distance - _offset);

                        time += Time.deltaTime;
                        await Task.Delay((int)(Time.deltaTime * 1000f));
                    }
                    catch (TaskCanceledException)
                    {

                    }
                }
            }
        }
    }

    void TargetAirborne(Transform _target)
    {
        var unitBase = _target.GetComponent<CHUnitBase>();
        if (unitBase != null)
        {
            unitBase.SetIsAirborne(true);
        }

        _target.DOJump(_target.transform.position, 5f, 1, 3f).OnComplete(() =>
        {
            unitBase.SetIsAirborne(false);
        });
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using UniRx;
using UnityEngine.AI;
using System.Threading;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CHMParticle
{
    Dictionary<Defines.EEffect, float> dicParticleTime = new Dictionary<Defines.EEffect, float>();

    CancellationTokenSource cts;

    public void Init()
    {
        cts = new CancellationTokenSource();

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
        cts = null;
        OnApplicationQuitHandler();
    }

    void OnApplicationQuitHandler()
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

        // Ÿ���� �� ��ŭ ��ƼŬ ���� 
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

            // ��ƼŬ�� ��� z���� ������ �ǵ���
            objParticle.transform.forward = dirParticle;
            objParticle.transform.forward = objParticle.transform.Angle(_effectData.effectAngle, Defines.EStandardAxis.Z);

            if (_effectData.createCasterPosition == false)
            {
                if (_effectData.attach)
                {
                    objParticle.transform.SetParent(trTarget);
                }
            }
            else
            {
                if (_effectData.attach)
                {
                    objParticle.transform.SetParent(_trCaster);
                }
            }

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

    void SetParticleCollision(Transform _trCaster, Transform _trTarget, GameObject _objParticle, SkillData.EffectData _effectData)
    {
        if (_effectData.eCollision != Defines.ECollision.None)
        {
            switch (_effectData.eEffect)
            {
                default:
                    {
                        // �Ϲ������� ��ƼŬ �θ� ������Ʈ���� �ݸ����� ����
                        ApplyShereCollision(_trCaster, _trTarget, _objParticle, _effectData);
                    }
                    break;
            }
        }
    }

    async void ApplyShereCollision(Transform _trCaster, Transform _trTarget, GameObject _objParticle, SkillData.EffectData _effectData)
    {
        await Task.Delay((int)(_effectData.triggerStartDelay * 1000f));

        var sphereCollision = _objParticle.GetOrAddComponent<CHSphereCollision>();
        sphereCollision.Init(_trCaster, _effectData);
        sphereCollision.sphereCollider.enabled = true;

        if (_effectData.triggerEnter)
        {
            sphereCollision.TriggerEnterCallback(sphereCollision.OnEnter.Subscribe(collider =>
            {
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
                if (IsTarget(_trCaster.gameObject.layer, collider.gameObject.layer, _effectData.eTargetMask))
                {
                    SetParticleTriggerValue(_trCaster, _trTarget, collider.transform, _objParticle, _effectData);
                }
            }));
        }

        sphereCollision.TriggerStayCallback(sphereCollision.OnStay.Subscribe(collider =>
        {
            if (IsTarget(_trCaster.gameObject.layer, collider.gameObject.layer, _effectData.eTargetMask))
            {
                SetParticleTriggerValue(_trCaster, _trTarget, collider.transform, _objParticle, _effectData);
            }
        }));

        if (_effectData.triggerStayTime >= 0f)
        {
            await Task.Delay((int)(_effectData.triggerStayTime * 1000f));

            if (sphereCollision != null && sphereCollision.sphereCollider != null) sphereCollision.sphereCollider.enabled = false;
        }
    }

    bool IsPoolableEffect(Defines.EEffect _eEffect)
    {
        switch (_eEffect)
        {
            // �����̴� �ֵ��� Ǯ�� ���� �񵿱Ⱑ ��ġ�鼭 �ӵ��� ���ϴ� �̽��� ����
            case Defines.EEffect.FX_Tornado:
            case Defines.EEffect.FX_Arrow_impact:
            case Defines.EEffect.FX_Ax:
                return false;
            default:
                return true;
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
        // �� ����Ʈ���� ���� ������ �ʿ��� ���
        switch (_effectData.eEffect)
        {
            case Defines.EEffect.FX_Circle_meteor:
            case Defines.EEffect.FX_Arrow_impact2:
                {
                    // y������ +23 �̵�
                    var posOrigin = _objParticle.transform.position;
                    _objParticle.transform.position = new Vector3(posOrigin.x, posOrigin.y + 23f, posOrigin.z);
                }
                break;
            case Defines.EEffect.FX_Arrow_impact:
                {
                    await MoveParticleDirection(cts.Token, _objParticle.transform.forward, 30f, dicParticleTime[_effectData.eEffect], _objParticle);
                }
                break;
            case Defines.EEffect.FX_Ax:
                {
                    // y������ +3 �̵�
                    var posOrigin = _objParticle.transform.position;
                    _objParticle.transform.position = new Vector3(posOrigin.x, posOrigin.y + 3f, posOrigin.z);

                    await MoveParticleDirection(cts.Token, _objParticle.transform.forward, 30f, 1f, _objParticle);
                    await MoveParticleTrasnform(cts.Token, _objParticle.transform, _trCaster, 30f, 3f, _objParticle);

                    CHMMain.Resource.Destroy(_objParticle);
                }
                break;
            case Defines.EEffect.FX_Tornado:
                {
                    await MoveParticleDirection(cts.Token, _objParticle.transform.forward, 10f, dicParticleTime[_effectData.eEffect], _objParticle);
                }
                break;
        }
    }

    void SetParticleTriggerValue(Transform _trCaster, Transform _trTarget, Transform _trTriggerTarget, GameObject _objParticle, SkillData.EffectData _effectData)
    {
        // �� ����Ʈ�� Ʈ���� �� Ÿ�ٵ� ���� ó��
        switch (_effectData.eEffect)
        {
            case Defines.EEffect.FX_Healing:
                {
                    // ������ �� ��ŭ�� ����
                    if (_trCaster.gameObject.layer == _trTriggerTarget.gameObject.layer) return;

                    CHMMain.Skill.ApplySkillValue(_trCaster, new List<Transform> { _trCaster }, _effectData);
                }
                break;
            case Defines.EEffect.FX_Tornado:
                {
                    TargetAirborne(cts.Token, _trTarget, 10, 0.5f, 25f);

                    CHMMain.Skill.ApplySkillValue(_trCaster, new List<Transform> { _trTriggerTarget }, _effectData);
                }
                break;
            case Defines.EEffect.FX_Electricity_Hit:
                {
                    TargetAirborne(cts.Token, _trTriggerTarget, 10, .5f, 100f);
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

    async Task MoveParticleDirection(CancellationToken _token, Vector3 _direction, float _speed, float _effectTime, GameObject _objParticle)
    {
        float time = 0;
        if (_direction != null)
        {
            while (!_token.IsCancellationRequested && time <= _effectTime)
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

    async Task MoveParticleTrasnform(CancellationToken _token, Transform _trStart, Transform _trEnd, float _speed, float _offset, GameObject _objParticle)
    {
        float time = 0;
        if (_trStart != null && _trEnd != null)
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    if (_objParticle == null) break;

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
                catch(TaskCanceledException)
                {

                }
            }
        }
    }

    async void TargetAirborne(CancellationToken _token, Transform _trTarget, float _airborneHeight, float _airborneTime, float _fallSpeed)
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

        float groundLevel = 0f;
        Vector3 fallVector = Vector3.zero;

        // �Ʒ��� �������� �ڵ�
        while (!_token.IsCancellationRequested && _trTarget != null && _trTarget.position.y > groundLevel)
        {
            try
            {
                if (unitBase.IsFalling == false) break;
                fallVector.y -= _fallSpeed * Time.deltaTime;
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
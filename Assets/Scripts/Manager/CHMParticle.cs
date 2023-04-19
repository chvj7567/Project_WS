using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using static Infomation;
using UnityEngine.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;

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
                particle.GetOrAddComponent<CHPoolable>();

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

    async void SetParticleValue(Transform _trCaster, Transform _trTarget, Vector3 _posSkill, Vector3 _dirSkill, GameObject _objParticle, EffectInfo _effectInfo)
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
                    // 자기 위치에서 파티클 시간 만큼 스킬 위치 방향으로 나아가게 설정
                    var effectTime = dicParticleInfo[_effectInfo.eEffect].time;
                    var posOrigin = _trCaster.position;
                    _objParticle.transform.position = posOrigin;

                    float time = 0;
                    while (time <= effectTime)
                    {
                        _objParticle.transform.position = posOrigin + _dirSkill.normalized * 10f * time;
                        time += Time.deltaTime;
                        await Task.Delay((int)(Time.deltaTime * 1000f));
                    }
                }
                break;
            default:
                break;
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using static Infomation;
using UnityEngine.Rendering.Universal;

public class CHMParticle
{
    Dictionary<Defines.EEffect, Infomation.ParticleInfo> dicParticleInfo = new Dictionary<Defines.EEffect, Infomation.ParticleInfo>();

    public void CreateTargetingParticle(Transform _trCaster, List<Transform> _liTarget, Defines.EStandardPos _eStandardPos, Defines.EEffect _particle, bool _autoDestory = true)
    {
        if (_trCaster == null)
        {
            Debug.Log("No Caster");
            return;
        }

        if (_liTarget == null || _liTarget.Count == 0)
        {
            Debug.Log("Targeting Skill : No Target");
            return;
        }

        List<GameObject> liParticle = new List<GameObject>();

        // 타겟의 수 만큼 파티클 생성
        for (int i = 0; i < _liTarget.Count; ++i)
        {
            var tempParticle = CHMMain.Particle.GetParticleObject(_particle, _autoDestory);

            if (tempParticle == null)
            {
                Debug.Log("No Particle");
                return;
            }

            liParticle.Add(tempParticle);
        }

        switch (_eStandardPos)
        {
            case Defines.EStandardPos.None:
                break;
            case Defines.EStandardPos.Me:
                {
                    // 자신을 지정하는 스킬은 liTarget에도 자기 Transform이 있음
                    var objParticle = liParticle.Last();
                    objParticle.transform.position = _trCaster.position;
                    objParticle.transform.rotation = _trCaster.rotation;

                    objParticle.transform.SetParent(_trCaster.transform);
                }
                break;
            case Defines.EStandardPos.TargetOne:
                {
                    var objParticle = liParticle.Last();
                    // 임시로 첫번째 적을 타겟으로 지정
                    var trTarget = _liTarget.First();
                    
                    objParticle.transform.position = trTarget.position;
                    objParticle.transform.rotation = trTarget.rotation;

                    objParticle.transform.SetParent(trTarget.transform);
                }
                break;
            case Defines.EStandardPos.TargetAll:
                for (int i = 0; i < liParticle.Count; ++i)
                {
                    var objParticle = liParticle.ElementAtOrDefault(i);
                    var trTarget = _liTarget.ElementAtOrDefault(i);

                    if (objParticle == null || trTarget == null || i >= _liTarget.Count)
                    {
                        Debug.Log("Error");
                        return;
                    }

                    objParticle.transform.position = trTarget.position;
                    objParticle.transform.rotation = trTarget.rotation;

                    objParticle.transform.SetParent(trTarget.transform);
                }
                break;
            default:
                break;
        }
    }

    public void CreateNoneTargetingParticle(Vector3 _posParticle, Vector3 _dirParticle, Defines.EEffect _particle, bool _autoDestory = true)
    {
        GameObject objParticle = CHMMain.Particle.GetParticleObject(_particle, _autoDestory);

        if (objParticle == null)
        {
            Debug.Log("No Particle");
            return;
        }

        objParticle.transform.position = _posParticle;
        objParticle.transform.forward = _dirParticle;
    }

    public GameObject GetParticleObject(Defines.EEffect _particle, bool _autoDestory = true)
    {
        GameObject objParticle = null;

        if (dicParticleInfo.ContainsKey(_particle) == false)
        {
            CHMMain.Resource.InstantiateEffect(_particle, (particle) =>
            {
                particle.SetActive(false);
                particle.GetOrAddComponent<CHPoolable>();

                dicParticleInfo.Add(_particle, new Infomation.ParticleInfo
                {
                    objParticle = particle,
                    time = GetParticleTime(particle)
                });

                objParticle = CHMMain.Resource.Instantiate(dicParticleInfo[_particle].objParticle);

                if (_autoDestory)
                {
                    DestroyParticle(_particle, objParticle);
                }
            });
        }
        else
        {
            objParticle = CHMMain.Resource.Instantiate(dicParticleInfo[_particle].objParticle);

            if (_autoDestory)
            {
                DestroyParticle(_particle, objParticle);
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
            time = Mathf.Max(time, particle.duration);
        }

        return time;
    }

    //-------------------------------------- private ------------------------------------------//

    async void DestroyParticle(Defines.EEffect _particle, GameObject _objParticle)
    {
        await Task.Delay((int)(dicParticleInfo[_particle].time * 1000));

        if (_objParticle) CHMMain.Resource.Destroy(_objParticle);
    }
}
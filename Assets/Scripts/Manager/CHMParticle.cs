using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using static Infomation;

public class CHMParticle
{
    Dictionary<Defines.EEffect, Infomation.ParticleInfo> dicParticleInfo = new Dictionary<Defines.EEffect, Infomation.ParticleInfo>();

    public void CreateTargetingParticle(Transform _trCaster, List<Transform> _liTarget, EffectInfo _effectInfo, bool _autoDestory = true)
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

        // Ÿ���� �� ��ŭ ��ƼŬ ����
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

        switch (_effectInfo.eStandardPos)
        {
            case Defines.EStandardPos.Me:
                {
                    // �ڽ��� �����ϴ� ��ų�� liTarget���� �ڱ� Transform�� ����
                    var objParticle = liParticle.Last();
                    objParticle.transform.position = _trCaster.position;
                    objParticle.transform.rotation = _trCaster.rotation;

                    objParticle.transform.SetParent(_trCaster.transform);

                    SetParticleValue(_effectInfo, objParticle);
                }
                break;
            case Defines.EStandardPos.TargetOne:
                {
                    var objParticle = liParticle.Last();
                    // �ӽ÷� ù��° ���� Ÿ������ ����
                    var trTarget = _liTarget.First();
                    
                    objParticle.transform.position = trTarget.position;
                    objParticle.transform.rotation = trTarget.rotation;

                    objParticle.transform.SetParent(trTarget.transform);

                    SetParticleValue(_effectInfo, objParticle);
                }
                break;
            case Defines.EStandardPos.TargetAll:
                for (int i = 0; i < liParticle.Count; ++i)
                {
                    var objParticle = liParticle.ElementAtOrDefault(i);
                    var trTarget = _liTarget.ElementAtOrDefault(i);

                    if (objParticle == null || trTarget == null || i >= _liTarget.Count)
                    {
                        Debug.Log("No Particle Or No Target");
                        return;
                    }

                    objParticle.transform.position = trTarget.position;
                    objParticle.transform.rotation = trTarget.rotation;

                    objParticle.transform.SetParent(trTarget.transform);

                    SetParticleValue(_effectInfo, objParticle);
                }
                break;
            default:
                break;
        }
    }

    public void CreateNoneTargetingParticle(Vector3 _posParticle, Vector3 _dirParticle, EffectInfo _effectInfo, bool _autoDestory = true)
    {
        GameObject objParticle = CHMMain.Particle.GetParticleObject(_effectInfo.eEffect, _autoDestory);

        if (objParticle == null)
        {
            Debug.Log("No Particle");
            return;
        }

        objParticle.transform.position = _posParticle;
        objParticle.transform.forward = _dirParticle;

        SetParticleValue(_effectInfo, objParticle);
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

    void SetParticleValue(EffectInfo _effectInfo, GameObject _objParticle)
    {
        // �� ����Ʈ �����պ��� ���� ������ �ʿ��� ���
        switch (_effectInfo.eEffect)
        {
            case Defines.EEffect.FX_Circle_ring:
                {
                    var psRingMain = _objParticle.GetComponent<ParticleSystem>().main;
                    var psSmokeMain = _objParticle.transform.GetChild(0).GetComponent<ParticleSystem>().main;

                    psRingMain.startLifetime = _effectInfo.startDelay + GetParticleTime(_objParticle);
                    psRingMain.startSize = _effectInfo.sphereRadius * 2f;
                    psSmokeMain.startDelay = 1f;
                    psSmokeMain.startLifetime = GetParticleTime(_objParticle) - 1f;
                    psSmokeMain.startSize = _effectInfo.sphereRadius * 2f;
                }
                break;
            case Defines.EEffect.FX_Circle_meteor:
                {
                    var posOrigin = _objParticle.transform.localPosition;
                    _objParticle.transform.localPosition = new Vector3(posOrigin.x, posOrigin.y + 5f, posOrigin.z);
                }
                break;
            case Defines.EEffect.FX_Circle_hit:
                {
                    var psHitMain = _objParticle.GetComponent<ParticleSystem>().main;

                    psHitMain.startSizeX = _effectInfo.sphereRadius;
                    psHitMain.startSizeY = _effectInfo.sphereRadius;
                }
                break;
            default:
                break;
        }
    }
}
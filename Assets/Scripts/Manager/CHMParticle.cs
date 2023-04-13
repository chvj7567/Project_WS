using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public class CHMParticle
{
    List<GameObject> liParticleObj = new List<GameObject>();
    Dictionary<Defines.EEffect, float> dicParticleTime = new Dictionary<Defines.EEffect, float>();

    public void Init()
    {
        liParticleObj.Clear();

        for (int i = 0; i < (int)Defines.EEffect.Max; ++i)
        {
            CHMMain.Resource.InstantiateEffect((Defines.EEffect)i, (particle) =>
            {
                particle.SetActive(false);
                particle.GetOrAddComponent<CHPoolable>();

                liParticleObj.Add(particle);
                dicParticleTime.Add((Defines.EEffect)i, GetParticleTime(particle));
            });
        }
    }

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

        // Ÿ���� �� ��ŭ ��ƼŬ ����
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
                    // �ڽ��� �����ϴ� ��ų�� liTarget���� �ڱ� Transform�� ����
                    var objParticle = liParticle.Last();
                    objParticle.transform.position = _trCaster.position;
                    objParticle.transform.rotation = _trCaster.rotation;

                    objParticle.transform.SetParent(_trCaster.transform);
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

    public void CreateNoneTargetingParticle(Vector3 _posParticle, Quaternion _rotParticle, Defines.EEffect _particle, bool _autoDestory = true)
    {
        GameObject objParticle = CHMMain.Particle.GetParticleObject(_particle, _autoDestory);

        if (objParticle == null)
        {
            Debug.Log("No Particle");
            return;
        }

        objParticle.transform.position = _posParticle;
        objParticle.transform.rotation = _rotParticle;
    }

    public GameObject GetParticleObject(Defines.EEffect _particle, bool _autoDestory = true)
    {
        var particleObj = CHMMain.Resource.Instantiate(liParticleObj[(int)_particle]);

        if (_autoDestory)
        {
            ParticleDestroy(_particle, particleObj);
        }

        return particleObj;
    }

    public GameObject GetRandomParticleObject(bool _autoDestory = true)
    {
        int randomIndex = UnityEngine.Random.Range(0, liParticleObj.Count);
        var particleObj = CHMMain.Resource.Instantiate(liParticleObj[randomIndex]);
         
        if (_autoDestory)
        {
            ParticleDestroy((Defines.EEffect)randomIndex, particleObj);
        }

        return particleObj;
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

    async void ParticleDestroy(Defines.EEffect _particle, GameObject _particleObj)
    {
        await Task.Delay((int)(dicParticleTime[_particle] * 1000));

        if (_particleObj) CHMMain.Resource.Destroy(_particleObj);
    }
}
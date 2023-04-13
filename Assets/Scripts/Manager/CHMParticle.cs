using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

public class CHMParticle
{
    List<GameObject> liParticleObj = new List<GameObject>();
    Dictionary<Defines.EParticle, float> dicParticleTime = new Dictionary<Defines.EParticle, float>();

    public void Init()
    {
        liParticleObj.Clear();

        for (int i = 0; i < (int)Defines.EParticle.Max; ++i)
        {
            CHMMain.Resource.InstantiateEffect((Defines.EParticle)i, (particle) =>
            {
                particle.SetActive(false);
                particle.GetOrAddComponent<CHPoolable>();

                liParticleObj.Add(particle);
                dicParticleTime.Add((Defines.EParticle)i, GetParticleTime(particle));
            });
        }
    }

    public void CreateParticle(Transform trCaster, List<Transform> liTarget, Defines.EStandardPos eStandardPos, Defines.EParticle _particle, bool isTargeting = false, bool _autoDestory = true)
    {
        // 스킬 시전자나 타겟을 할 목표가 없으면 Return;
        if (trCaster == null || liTarget == null || liTarget.Count == 0)
        {
            Debug.Log("Error");
            return;
        }

        List<GameObject> liParticle = new List<GameObject>();

        // 타겟의 수 만큼 파티클 생성
        for (int i = 0; i < liTarget.Count; ++i)
        {
            var tempParticle = CHMMain.Particle.GetParticleObject(_particle, _autoDestory);

            if (tempParticle == null)
            {
                Debug.Log("Error");
                return;
            }

            liParticle.Add(tempParticle);
        }

        switch (eStandardPos)
        {
            case Defines.EStandardPos.None:
                break;
            case Defines.EStandardPos.Me:
                {
                    var objParticle = liParticle.Last();
                    objParticle.transform.position = trCaster.position;
                    objParticle.transform.rotation = trCaster.rotation;
                    if (isTargeting)
                    {
                        objParticle.transform.SetParent(trCaster);
                    }
                }
                break;
            case Defines.EStandardPos.TargetOne:
                {
                    var objParticle = liParticle.Last();
                    // 임시로 첫번째 적을 타겟으로 지정
                    var trTarget = liTarget.First();
                    
                    objParticle.transform.position = trTarget.position;
                    objParticle.transform.rotation = trTarget.rotation;
                    if (isTargeting)
                    {
                        objParticle.transform.SetParent(trTarget);
                    }
                }
                break;
            case Defines.EStandardPos.TargetAll:
                for (int i = 0; i < liParticle.Count; ++i)
                {
                    var objParticle = liParticle.ElementAtOrDefault(i);
                    var trTarget = liTarget.ElementAtOrDefault(i);

                    if (objParticle == null || trTarget == null || i >= liTarget.Count)
                    {
                        Debug.Log("Error");
                        return;
                    }

                    objParticle.transform.position = trTarget.position;
                    objParticle.transform.rotation = trTarget.rotation;

                    if (isTargeting)
                    {
                        objParticle.transform.SetParent(trTarget);
                    }
                }
                break;
            default:
                break;
        }
    }

    public GameObject GetParticleObject(Defines.EParticle _particle, bool _autoDestory = true)
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
            ParticleDestroy((Defines.EParticle)randomIndex, particleObj);
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

    async void ParticleDestroy(Defines.EParticle _particle, GameObject _particleObj)
    {
        await Task.Delay((int)(dicParticleTime[_particle] * 1000));

        if (_particleObj) CHMMain.Resource.Destroy(_particleObj);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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

    public async void ParticleDestroy(Defines.EParticle _particle, GameObject _particleObj)
    {
        await Task.Delay((int)(dicParticleTime[_particle] * 1000));

        if (_particleObj) CHMMain.Resource.Destroy(_particleObj);
    }
}

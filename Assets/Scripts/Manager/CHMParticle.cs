using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHMParticle
{
    List<GameObject> liParticleObj = new List<GameObject>();

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
            });
        }
    }

    public GameObject GetParticle(Defines.EParticle _particle)
    {
        return CHMMain.Resource.Instantiate(liParticleObj[(int)_particle]);
    }

    public GameObject GetRandomParticle()
    {
        int randomIndex = UnityEngine.Random.Range(0, liParticleObj.Count);
        return CHMMain.Resource.Instantiate(liParticleObj[randomIndex]);
    }

    public float GetParticleTime(ParticleSystem _particle)
    {
        float time = -1;

        var arrParticle = _particle.GetComponentsInChildren<ParticleSystem>();

        foreach (var particle in arrParticle)
        {
            time = Mathf.Max(time, particle.duration);
        }

        return time;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ContBullet : MonoBehaviour
{
    static GameObject hitParticleObj;
    static float hitParticleTime;

    private void Start()
    {
        if (hitParticleObj == null)
        {
            CHMMain.Resource.InstantiateEffect(Defines.EParticle.FX_Fire, (effect) =>
            {
                hitParticleObj = effect;
                hitParticleObj.GetOrAddComponent<CHPoolable>();
                hitParticleTime = hitParticleObj.GetOrAddComponent<ParticleSystem>().GetParticleTime();
            });
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(CHMMain.Resource.Instantiate(hitParticleObj));

        CHMMain.Resource.Destroy(gameObject);
    }

    async void Destroy(GameObject _obj)
    {
        await Task.Delay((int)(hitParticleTime * 1000));
        if (_obj != null)
        {
            CHMMain.Resource.Destroy(_obj);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using UniRx;
using static CHTargetTracker;
using System.Threading.Tasks;

public class ContEnemy : MonoBehaviour
{
    [SerializeField] float attackDistance;
    [SerializeField] float attackDelay = 1f;
    [SerializeField] float timeSinceLastAttack;

    TargetInfo target;
    private void Start()
    {
        var targetTracker = gameObject.GetOrAddComponent<CHTargetTracker>();

        gameObject.UpdateAsObservable().Subscribe(async _ =>
        {
            target = targetTracker.GetTargetInfo();

            if (timeSinceLastAttack < attackDelay)
            {
                timeSinceLastAttack += Time.deltaTime;
            }
            else if (target != null && target.distance <= attackDistance)
            {
                var particle = CHMMain.Particle.GetRandomParticleObject();
                particle.transform.position = target.targetObj.transform.position;
                timeSinceLastAttack = 0f;
            }
        });
    }
}

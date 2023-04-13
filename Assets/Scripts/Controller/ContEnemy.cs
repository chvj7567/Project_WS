using UniRx.Triggers;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class ContEnemy : MonoBehaviour
{
    [SerializeField] float attackDistance;
    [SerializeField] float attackDelay = 1f;
    [SerializeField] float timeSinceLastAttack;

    private void Start()
    {
        var targetTracker = gameObject.GetOrAddComponent<CHTargetTracker>();

        gameObject.UpdateAsObservable().Subscribe(_ =>
        {
            Infomation.TargetInfo mainTarget = targetTracker.GetClosestTargetInfo();

            if (timeSinceLastAttack < attackDelay)
            {
                timeSinceLastAttack += Time.deltaTime;
            }
            else if (mainTarget != null && mainTarget.distance <= attackDistance)
            {
                //List<Transform> liTarget = new List<Transform>();

                foreach (var target in targetTracker.GetTargetInfoListInRange())
                {
                    List<Transform> liTarget = new List<Transform>();
                    liTarget.Add(target.targetObj.transform);
                   // CHMMain.Skill.CreateNoneTargetingSkill(transform, Defines.ESkillID.Explosion);
                }

                //CHMMain.Particle.CreateParticle(transform, liTarget, Defines.EStandardPos.TargetAll, (Defines.EParticle)Random.Range(0, (int)Defines.EParticle.Max), true);

                timeSinceLastAttack = 0f;
            }
        });
    }
}

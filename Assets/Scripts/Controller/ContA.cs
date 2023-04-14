using UniRx.Triggers;
using UnityEngine;
using UniRx;

public class ContA : MonoBehaviour
{
    [SerializeField, ReadOnly] float attackDistance;
    [SerializeField, ReadOnly] float attackDelay;
    [SerializeField, ReadOnly] float timeSinceLastAttack;

    private void Start()
    {
        var unitInfo = gameObject.GetOrAddComponent<UnitA>();
        if (unitInfo != null)
        {
            attackDistance = unitInfo.GetCurrentAttackDistance();
            attackDelay = unitInfo.GetCurrentAttackDelay();
        }

        var targetTracker = gameObject.GetOrAddComponent<CHTargetTracker>();
        if (targetTracker != null)
        {
            gameObject.UpdateAsObservable().Subscribe(_ =>
            {
                Infomation.TargetInfo mainTarget = targetTracker.GetClosestTargetInfo();

                if (timeSinceLastAttack < attackDelay)
                {
                    timeSinceLastAttack += Time.deltaTime;
                }
                else if (mainTarget != null && mainTarget.distance <= attackDistance)
                {
                    CHMMain.Skill.CreateAISkill(transform, mainTarget.objTarget.transform, Defines.ESkillID.Explosion);
                    timeSinceLastAttack = 0f;
                }
            });
        }
    }
}

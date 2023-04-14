using UniRx.Triggers;
using UnityEngine;
using UniRx;

public class ContA : MonoBehaviour
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
                CHMMain.Skill.CreateAISkill(transform, mainTarget.targetObj.transform, Defines.ESkillID.Explosion);
                timeSinceLastAttack = 0f;
            }
        });
    }
}

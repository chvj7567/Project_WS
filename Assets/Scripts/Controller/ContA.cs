using UniRx.Triggers;
using UnityEngine;
using UniRx;

public class ContA : ContBase
{
    private void Start()
    {
        var unitInfo = gameObject.GetOrAddComponent<UnitA>();
        var targetTracker = gameObject.GetOrAddComponent<CHTargetTracker>();
        if (unitInfo!= null && targetTracker != null)
        {
            gameObject.UpdateAsObservable().Subscribe(_ =>
            {
                Infomation.TargetInfo mainTarget = targetTracker.GetClosestTargetInfo();

                // Ÿ���� ���� �ȿ� ������ Ÿ���� ���� �ȿ� �������� ��� ������ �� �ֵ��� ����
                if (mainTarget == null)
                {
                    timeSinceLastAttack = -1f;
                    timeSinceLastSkill1 = -1f;
                    timeSinceLastSkill2 = -1f;
                    timeSinceLastSkill3 = -1f;
                    timeSinceLastSkill4 = -1f;

                    animator.SetBool(attackRange, false);
                    animator.SetBool(sightRange, false);
                }
                // Ÿ���� ���� �ȿ� ������ ��� ���� �� ���� ������ ����
                else
                {
                    // �⺻ ����
                    if (useAttack)
                    {
                        if (timeSinceLastAttack >= 0f && timeSinceLastAttack < unitInfo.GetOriginAttackDelay())
                        {
                            timeSinceLastAttack += Time.deltaTime;
                            animator.SetBool(sightRange, false);
                            animator.SetBool(attackRange, false);
                        }
                        // ���� �����Ÿ� ���� ���
                        else if (mainTarget.distance <= unitInfo.GetOriginAttackDistance())
                        {
                            animator.SetBool(sightRange, false);
                            animator.SetBool(attackRange, true);
                            timeSinceLastAttack = 0.00001f;
                        }
                        else
                        {
                            animator.SetBool(sightRange, true);
                            animator.SetBool(attackRange, false);
                        }
                    }
                    else
                    {
                        // ���� �����Ÿ� ���� ���
                        if (mainTarget.distance <= unitInfo.GetOriginAttackDistance())
                        {
                            animator.SetBool(sightRange, false);
                            animator.SetBool(attackRange, false);
                        }
                        else
                        {
                            animator.SetBool(sightRange, true);
                            animator.SetBool(attackRange, false);
                        }

                        timeSinceLastAttack = -1f;
                    }

                    // 1�� ��ų
                    if (useSkill1)
                    {
                        if (timeSinceLastSkill1 >= 0f && timeSinceLastSkill1 < unitInfo.GetOriginSkill1CoolTime())
                        {
                            timeSinceLastSkill1 += Time.deltaTime;
                        }
                        else if (mainTarget.distance <= unitInfo.GetOriginSkill1Distance())
                        {
                            CHMMain.Skill.CreateAISkill(transform, mainTarget.objTarget.transform, unitInfo.GetOriginSkill1());
                            timeSinceLastSkill1 = 0.00001f;
                        }
                    }
                    else
                    {
                        timeSinceLastSkill1 = -1f;
                    }

                    // 2�� ��ų
                    if (useSkill2)
                    {
                        if (timeSinceLastSkill2 >= 0f && timeSinceLastSkill2 < unitInfo.GetOriginSkill2CoolTime())
                        {
                            timeSinceLastSkill2 += Time.deltaTime;
                        }
                        else if (mainTarget.distance <= unitInfo.GetOriginSkill2Distance())
                        {
                            CHMMain.Skill.CreateAISkill(transform, mainTarget.objTarget.transform, unitInfo.GetOriginSkill2());
                            timeSinceLastSkill2 = 0.00001f;
                        }
                    }
                    else
                    {
                        timeSinceLastSkill2 = -1f;
                    }

                    // 3�� ��ų
                    if (useSkill3)
                    {
                        if (timeSinceLastSkill3 >= 0f && timeSinceLastSkill3 < unitInfo.GetOriginSkill3CoolTime())
                        {
                            timeSinceLastSkill3 += Time.deltaTime;
                        }
                        else if (mainTarget.distance <= unitInfo.GetOriginSkill3Distance())
                        {
                            CHMMain.Skill.CreateAISkill(transform, mainTarget.objTarget.transform, unitInfo.GetOriginSkill3());
                            timeSinceLastSkill3 = 0.00001f;
                        }
                    }
                    else
                    {
                       timeSinceLastSkill3 = -1f;
                    }

                    // 4�� ��ų
                    if (useSkill4)
                    {
                        if (timeSinceLastSkill4 >= 0f && timeSinceLastSkill4 < unitInfo.GetOriginSkill4CoolTime())
                        {
                            timeSinceLastSkill4 += Time.deltaTime;
                        }
                        else if (mainTarget.distance <= unitInfo.GetOriginSkill4Distance())
                        {
                            CHMMain.Skill.CreateAISkill(transform, mainTarget.objTarget.transform, unitInfo.GetOriginSkill4());
                            timeSinceLastSkill4 = 0.00001f;
                        }
                    }
                    else
                    {
                        timeSinceLastSkill4 = -1f;
                    }
                }
            });
        }
    }
}

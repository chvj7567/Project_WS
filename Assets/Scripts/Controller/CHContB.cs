using UniRx.Triggers;
using UnityEngine;
using UniRx;

public class CHContB : CHContBase
{
    private void Start()
    {
        var unitInfo = gameObject.GetOrAddComponent<CHUnitB>();
        var targetTracker = gameObject.GetOrAddComponent<CHTargetTracker>();
        if (unitInfo!= null && targetTracker != null)
        {
            targetTracker.followSpeed = unitInfo.GetOriginMoveSpeed();
            targetTracker.range = unitInfo.GetOriginAttackDistance() * 2f;
            targetTracker.approachDistance = unitInfo.GetOriginAttackDistance();
            targetTracker.viewAngle = unitInfo.GetOriginViewAngle();
            targetTracker.ResetViewAngleOrigin();

            gameObject.UpdateAsObservable().Subscribe(_ =>
            {
                Infomation.TargetInfo mainTarget = targetTracker.GetClosestTargetInfo();

                // 타겟이 범위 안에 없으면 타겟이 범위 안에 들어왔을때 즉시 공격할 수 있도록 설정
                if (mainTarget == null)
                {
                    timeSinceLastAttack = -1f;
                    timeSinceLastSkill1 = -1f;
                    timeSinceLastSkill2 = -1f;
                    timeSinceLastSkill3 = -1f;
                    timeSinceLastSkill4 = -1f;

                    animator.SetBool(attackRange, false);
                }
                // 타겟이 범위 안에 있으면 즉시 공격 후 공격 딜레이 설정
                else
                {
                    // 기본 공격
                    if (useAttack)
                    {
                        if (timeSinceLastAttack >= 0f && timeSinceLastAttack < unitInfo.GetOriginAttackDelay())
                        {
                            timeSinceLastAttack += Time.deltaTime;
                            animator.SetBool(attackRange, false);
                        }
                        // 공격 사정거리 안인 경우
                        else if (mainTarget.distance <= unitInfo.GetOriginAttackDistance())
                        {
                            animator.SetBool(attackRange, true);
                            timeSinceLastAttack = 0.00001f;
                        }
                        else
                        {
                            animator.SetBool(attackRange, false);
                        }
                    }
                    else
                    {
                        // 공격 사정거리 안인 경우
                        if (mainTarget.distance <= unitInfo.GetOriginAttackDistance())
                        {
                            animator.SetBool(attackRange, false);
                        }
                        else
                        {
                            animator.SetBool(attackRange, false);
                        }

                        timeSinceLastAttack = -1f;
                    }

                    // 1번 스킬
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

                    // 2번 스킬
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

                    // 3번 스킬
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

                    // 4번 스킬
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

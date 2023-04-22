using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Rendering;

public class CHContBase : MonoBehaviour
{
    [SerializeField] protected bool useAttack = true;
    [SerializeField] protected bool useSkill1 = true;
    [SerializeField] protected bool useSkill2 = true;
    [SerializeField] protected bool useSkill3 = true;
    [SerializeField] protected bool useSkill4 = true;

    [SerializeField, ReadOnly] protected float timeSinceLastAttack = -1f;
    [SerializeField, ReadOnly] protected float timeSinceLastSkill1 = -1f;
    [SerializeField, ReadOnly] protected float timeSinceLastSkill2 = -1f;
    [SerializeField, ReadOnly] protected float timeSinceLastSkill3 = -1f;
    [SerializeField, ReadOnly] protected float timeSinceLastSkill4 = -1f;

    [SerializeField, ReadOnly] Animator animator;

    [SerializeField, ReadOnly] public int attackRange = Animator.StringToHash("AttackRange");
    [SerializeField, ReadOnly] public int sightRange = Animator.StringToHash("SightRange");
    [SerializeField, ReadOnly] public int isDeath = Animator.StringToHash("IsDeath");

    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        animator = GetComponent<Animator>();
        var unitInfo = gameObject.GetOrAddComponent<CHUnitBase>();
        var targetTracker = gameObject.GetOrAddComponent<CHTargetTracker>();
        if (unitInfo != null && targetTracker != null)
        {
            targetTracker.moveSpeed = unitInfo.GetCurrentMoveSpeed();
            targetTracker.rotateSpeed = unitInfo.GetCurrentRotateSpeed();
            targetTracker.range = unitInfo.GetCurrentAttackDistance() * 2f;
            targetTracker.approachDistance = unitInfo.GetCurrentAttackDistance();
            targetTracker.viewAngle = unitInfo.GetCurrentViewAngle();
            targetTracker.ResetViewAngleOrigin();

            timeSinceLastAttack = -1f;
            timeSinceLastSkill1 = -1f;
            timeSinceLastSkill2 = -1f;
            timeSinceLastSkill3 = -1f;
            timeSinceLastSkill4 = -1f;

            gameObject.UpdateAsObservable().Subscribe(_ =>
            {
                Infomation.TargetInfo mainTarget = targetTracker.GetClosestTargetInfo();

                if (timeSinceLastAttack >= 0f && timeSinceLastAttack < unitInfo.GetOriginAttackDelay())
                {
                    timeSinceLastAttack += Time.deltaTime;
                }
                else
                {
                    timeSinceLastAttack = -1f;
                }

                if (timeSinceLastSkill1 >= 0f && timeSinceLastSkill1 < unitInfo.GetOriginSkill1CoolTime())
                {
                    timeSinceLastSkill1 += Time.deltaTime;
                }
                else
                {
                    timeSinceLastSkill1 = -1f;
                }

                if (timeSinceLastSkill2 >= 0f && timeSinceLastSkill2 < unitInfo.GetOriginSkill2CoolTime())
                {
                    timeSinceLastSkill2 += Time.deltaTime;
                }
                else
                {
                    timeSinceLastSkill2 = -1f;
                }

                if (timeSinceLastSkill3 >= 0f && timeSinceLastSkill3 < unitInfo.GetOriginSkill3CoolTime())
                {
                    timeSinceLastSkill3 += Time.deltaTime;
                }
                else
                {
                    timeSinceLastSkill3 = -1f;
                }

                if (timeSinceLastSkill4 >= 0f && timeSinceLastSkill4 < unitInfo.GetOriginSkill4CoolTime())
                {
                    timeSinceLastSkill4 += Time.deltaTime;
                }
                else
                {
                    timeSinceLastSkill4 = -1f;
                }

                if (mainTarget == null)
                {
                    if (animator) animator.SetBool(attackRange, false);
                }
                // 타겟이 범위 안에 있으면 즉시 공격 후 공격 딜레이 설정
                else
                {
                    var posMainTarget = mainTarget.objTarget.transform.position;
                    var posMy = transform.position;
                    posMainTarget.y = 0f;
                    posMy.y = 0f;
                    var dirMainTarget = posMainTarget - posMy;

                    // 기본 공격
                    if (useAttack)
                    {
                        if (timeSinceLastAttack < 0f && mainTarget.distance <= unitInfo.GetOriginAttackDistance())
                        {
                            if (animator) animator.SetBool(attackRange, true);
                            timeSinceLastAttack = 0.00001f;
                        }
                        else
                        {
                            if (animator) animator.SetBool(attackRange, false);
                        }
                    }
                    else
                    {
                        // 공격 사정거리 안인 경우
                        if (mainTarget.distance <= unitInfo.GetOriginAttackDistance())
                        {
                            if (animator) animator.SetBool(attackRange, false);
                        }
                        else
                        {
                            if (animator) animator.SetBool(attackRange, false);
                        }
                    }

                    // 1번 스킬
                    if (useSkill1)
                    {
                        if (timeSinceLastSkill1 < 0f && mainTarget.distance <= unitInfo.GetOriginSkill1Distance())
                        {
                            CHMMain.Skill.CreateSkill(transform, mainTarget.objTarget.transform, posMainTarget, dirMainTarget, unitInfo.GetOriginSkill1());
                            timeSinceLastSkill1 = 0.0001f;
                        }
                    }

                    // 2번 스킬
                    if (useSkill2)
                    {
                        if (timeSinceLastSkill2 < 0f && mainTarget.distance <= unitInfo.GetOriginSkill2Distance())
                        {
                            CHMMain.Skill.CreateSkill(transform, mainTarget.objTarget.transform, posMainTarget, dirMainTarget, unitInfo.GetOriginSkill2());
                            timeSinceLastSkill2 = 0.0001f;
                        }
                    }

                    // 3번 스킬
                    if (useSkill3)
                    {
                        if (timeSinceLastSkill3 < 0f && mainTarget.distance <= unitInfo.GetOriginSkill3Distance())
                        {
                            CHMMain.Skill.CreateSkill(transform, mainTarget.objTarget.transform, posMainTarget, dirMainTarget, unitInfo.GetOriginSkill3());
                            timeSinceLastSkill3 = 0.0001f;
                        }
                    }

                    // 4번 스킬
                    if (useSkill4)
                    {
                        if (timeSinceLastSkill4 < 0f && mainTarget.distance <= unitInfo.GetOriginSkill4Distance())
                        {
                            CHMMain.Skill.CreateSkill(transform, mainTarget.objTarget.transform, posMainTarget, dirMainTarget, unitInfo.GetOriginSkill4());
                            timeSinceLastSkill4 = 0.0001f;
                        }
                    }
                }
            });
        }
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public float GetTimeSinceLastAttack() { return timeSinceLastAttack; }
    public float GetTimeSinceLastSkill1() { return timeSinceLastSkill1; }
    public float GetTimeSinceLastSkill2() { return timeSinceLastSkill2; }
    public float GetTimeSinceLastSkill3() { return timeSinceLastSkill3; }
    public float GetTimeSinceLastSkill4() { return timeSinceLastSkill4; }

    public void SetTimeSinceLastAttack(float _time) { timeSinceLastAttack = _time; }
    public void SetTimeSinceLastSkill1(float _time) { timeSinceLastSkill1 = _time; }
    public void SetTimeSinceLastSkill2(float _time) { timeSinceLastSkill2 = _time; }
    public void SetTimeSinceLastSkill3(float _time) { timeSinceLastSkill3 = _time; }
    public void SetTimeSinceLastSkill4(float _time) { timeSinceLastSkill4 = _time; }
}

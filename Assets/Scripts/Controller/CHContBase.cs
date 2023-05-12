using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

public class CHContBase : MonoBehaviour
{
    [SerializeField] protected bool useSkill1 = false;
    [SerializeField] protected bool useSkill2 = false;
    [SerializeField] protected bool useSkill3 = false;
    [SerializeField] protected bool useSkill4 = false;

    [SerializeField, ReadOnly] protected bool skill1Lock = false;
    [SerializeField, ReadOnly] protected bool skill2Lock = false;
    [SerializeField, ReadOnly] protected bool skill3Lock = false;
    [SerializeField, ReadOnly] protected bool skill4Lock = false;

    [SerializeField, ReadOnly] protected bool useSkill1Channeling = false;
    [SerializeField, ReadOnly] protected bool useSkill2Channeling = false;
    [SerializeField, ReadOnly] protected bool useSkill3Channeling = false;
    [SerializeField, ReadOnly] protected bool useSkill4Channeling = false;

    [SerializeField, ReadOnly] protected bool skill1Channeling = false;
    [SerializeField, ReadOnly] protected bool skill2Channeling = false;
    [SerializeField, ReadOnly] protected bool skill3Channeling = false;
    [SerializeField, ReadOnly] protected bool skill4Channeling = false;

    [SerializeField, ReadOnly] protected float timeSinceLastSkill1 = -1f;
    [SerializeField, ReadOnly] protected float timeSinceLastSkill2 = -1f;
    [SerializeField, ReadOnly] protected float timeSinceLastSkill3 = -1f;
    [SerializeField, ReadOnly] protected float timeSinceLastSkill4 = -1f;

    [SerializeField, ReadOnly] Animator animator;

    enum Anim
    {
        Idle,
        Attack,
        Run,
        Death
    }

    [SerializeField] List<string> liAnimName = new List<string>();

    [SerializeField, ReadOnly] Dictionary<string, float> dicAnimTime = new Dictionary<string, float>();

    [SerializeField, ReadOnly] public int attackRange = Animator.StringToHash("AttackRange");
    [SerializeField, ReadOnly] public int sightRange = Animator.StringToHash("SightRange");
    [SerializeField, ReadOnly] public int isDeath = Animator.StringToHash("IsDeath");

    CancellationTokenSource cts;
    CancellationToken token;

    private void Start()
    {
        Init();
    }

    private void OnDestroy()
    {
        if (cts != null && !cts.IsCancellationRequested)
        {
            cts.Cancel();
        }
    }

    public virtual void Init()
    {
        cts = new CancellationTokenSource();
        token = cts.Token;

        animator = GetComponent<Animator>();

        if (animator != null)
        {
            RuntimeAnimatorController ac = animator.runtimeAnimatorController;

            foreach (AnimationClip clip in ac.animationClips)
            {
                Debug.Log(clip.name);
                dicAnimTime.Add(clip.name, clip.length);
            }
        }

        var unitInfo = gameObject.GetOrAddComponent<CHUnitBase>();
        var targetTracker = gameObject.GetOrAddComponent<CHTargetTracker>();
        if (unitInfo != null && targetTracker != null)
        {
            if (unitInfo.GetOriginSkill1Data() == null) skill1Lock = true;
            if (unitInfo.GetOriginSkill2Data() == null) skill2Lock = true;
            if (unitInfo.GetOriginSkill3Data() == null) skill3Lock = true;
            if (unitInfo.GetOriginSkill4Data() == null) skill4Lock = true;

            targetTracker.ResetValue(unitInfo);

            timeSinceLastSkill1 = -1f;
            timeSinceLastSkill2 = -1f;
            timeSinceLastSkill3 = -1f;
            timeSinceLastSkill4 = -1f;

            gameObject.UpdateAsObservable().Subscribe(async _ =>
            {
                // 죽거나 땅에 있는 상태가 아닐 때 (에어본 같은 경우)
                if (unitInfo.GetIsDeath() && transform.position.y >= 0.1f)
                {
                    return;
                }

                Infomation.TargetInfo mainTarget = targetTracker.GetClosestTargetInfo();

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
                    Vector3 posMainTarget = mainTarget.objTarget.transform.position;
                    Vector3 posMy = transform.position;
                    Vector3 dirMy = Vector3.zero;

                    switch (targetTracker.standardAxis)
                    {
                        case Defines.EStandardAxis.X:
                            {
                                dirMy = transform.right;
                            }
                            break;
                        case Defines.EStandardAxis.Z:
                            {
                                dirMy = transform.forward;
                            }
                            break;
                    }
                    posMainTarget.y = 0f;
                    posMy.y = 0f;
                    var dirMainTarget = posMainTarget - posMy;

                    // 1번 스킬
                    if ((skill1Lock == false) && useSkill1 && unitInfo.IsNormalState())
                    {
                        if ((skill1Channeling == false) && (timeSinceLastSkill1 < 0f) && (mainTarget.distance <= unitInfo.GetOriginSkill1Distance()))
                        {
                            // 스킬 쿨타임 초기화
                            timeSinceLastSkill1 = 0.0001f;

                            if (animator)
                            {
                                animator.SetTrigger(attackRange);
                                
                                if (unitInfo.GetOriginSkill1Data().isChanneling)
                                {
                                    // 애니메이션 시전 시간동안 채널링
                                    skill1Channeling = true;

                                    // 일단 모든 스킬은 공격 애니메이션으로 통일하지만 추후 활용시 유닛정보에 애니메이션 정보 담아서 활용 가능
                                    await Task.Delay((int)(dicAnimTime[liAnimName[(int)Anim.Attack]] * 1000f));

                                    if (cts.IsCancellationRequested) return;

                                    skill1Channeling = false;
                                }
                            }

                            CHMMain.Skill.CreateSkill(new CHMSkill.SkillLocationInfo
                            {
                                trCaster = transform,
                                posCaster = posMy,
                                dirCaster = dirMy,
                                trTarget = mainTarget.objTarget.transform,
                                posTarget = posMainTarget,
                                dirTarget = posMainTarget - posMy,
                                posSkill = posMainTarget,
                                dirSkill = posMainTarget - posMy,
                            }, unitInfo.GetOriginSkill1());
                        }
                    }

                    // 2번 스킬
                    if ((skill2Lock == false) && useSkill2 && unitInfo.IsNormalState())
                    {
                        if (timeSinceLastSkill2 < 0f && mainTarget.distance <= unitInfo.GetOriginSkill2Distance())
                        {
                            // 스킬 쿨타임 초기화
                            timeSinceLastSkill2 = 0.0001f;

                            if (animator)
                            {
                                animator.SetTrigger(attackRange);

                                if (unitInfo.GetOriginSkill2Data().isChanneling)
                                {
                                    // 애니메이션 시전 시간동안 채널링
                                    skill2Channeling = true;
                                    await Task.Delay((int)(dicAnimTime[liAnimName[(int)Anim.Attack]] * 1000f));

                                    if (cts.IsCancellationRequested) return;

                                    skill2Channeling = false;
                                }
                            }

                            CHMMain.Skill.CreateSkill(new CHMSkill.SkillLocationInfo
                            {
                                trCaster = transform,
                                posCaster = posMy,
                                dirCaster = dirMy,
                                trTarget = mainTarget.objTarget.transform,
                                posTarget = posMainTarget,
                                dirTarget = posMainTarget - posMy,
                                posSkill = posMainTarget,
                                dirSkill = posMainTarget - posMy,
                            }, unitInfo.GetOriginSkill2());
                            timeSinceLastSkill2 = 0.0001f;
                        }
                    }

                    // 3번 스킬
                    if ((skill3Lock == false) && useSkill3 && unitInfo.IsNormalState())
                    {
                        if (timeSinceLastSkill3 < 0f && mainTarget.distance <= unitInfo.GetOriginSkill3Distance())
                        {
                            // 스킬 쿨타임 초기화
                            timeSinceLastSkill3 = 0.0001f;

                            if (animator)
                            {
                                animator.SetTrigger(attackRange);

                                if (unitInfo.GetOriginSkill3Data().isChanneling)
                                {
                                    // 애니메이션 시전 시간동안 채널링
                                    skill3Channeling = true;
                                    await Task.Delay((int)(dicAnimTime[liAnimName[(int)Anim.Attack]] * 1000f));

                                    if (cts.IsCancellationRequested) return;

                                    skill3Channeling = false;
                                }
                            }

                            CHMMain.Skill.CreateSkill(new CHMSkill.SkillLocationInfo
                            {
                                trCaster = transform,
                                posCaster = posMy,
                                dirCaster = dirMy,
                                trTarget = mainTarget.objTarget.transform,
                                posTarget = posMainTarget,
                                dirTarget = posMainTarget - posMy,
                                posSkill = posMainTarget,
                                dirSkill = posMainTarget - posMy,
                            }, unitInfo.GetOriginSkill3());
                            timeSinceLastSkill3 = 0.0001f;
                        }
                    }

                    // 4번 스킬
                    if ((skill4Lock == false) && useSkill4 && unitInfo.IsNormalState())
                    {
                        if (timeSinceLastSkill4 < 0f && mainTarget.distance <= unitInfo.GetOriginSkill4Distance())
                        {
                            // 스킬 쿨타임 초기화
                            timeSinceLastSkill4 = 0.0001f;

                            if (animator)
                            {
                                animator.SetTrigger(attackRange);

                                if (unitInfo.GetOriginSkill4Data().isChanneling)
                                {
                                    // 애니메이션 시전 시간동안 채널링
                                    skill4Channeling = true;
                                    await Task.Delay((int)(dicAnimTime[liAnimName[(int)Anim.Attack]] * 1000f));

                                    if (cts.IsCancellationRequested) return;

                                    skill4Channeling = false;
                                }
                            }

                            CHMMain.Skill.CreateSkill(new CHMSkill.SkillLocationInfo
                            {
                                trCaster = transform,
                                posCaster = posMy,
                                dirCaster = dirMy,
                                trTarget = mainTarget.objTarget.transform,
                                posTarget = posMainTarget,
                                dirTarget = posMainTarget - posMy,
                                posSkill = posMainTarget,
                                dirSkill = posMainTarget - posMy,
                            }, unitInfo.GetOriginSkill4());
                            timeSinceLastSkill4 = 0.0001f;
                        }
                    }
                }
            }).AddTo(this);
        }
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public float GetTimeSinceLastSkill1() { return timeSinceLastSkill1; }
    public float GetTimeSinceLastSkill2() { return timeSinceLastSkill2; }
    public float GetTimeSinceLastSkill3() { return timeSinceLastSkill3; }
    public float GetTimeSinceLastSkill4() { return timeSinceLastSkill4; }

    public void SetTimeSinceLastSkill1(float _time) { timeSinceLastSkill1 = _time; }
    public void SetTimeSinceLastSkill2(float _time) { timeSinceLastSkill2 = _time; }
    public void SetTimeSinceLastSkill3(float _time) { timeSinceLastSkill3 = _time; }
    public void SetTimeSinceLastSkill4(float _time) { timeSinceLastSkill4 = _time; }
}

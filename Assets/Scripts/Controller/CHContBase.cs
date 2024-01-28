using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CHContBase : MonoBehaviour
{
    // ��ų ��� ����
    [SerializeField] bool useSkill1 = true;
    [SerializeField] bool useSkill2 = true;
    [SerializeField] bool useSkill3 = true;
    [SerializeField] bool useSkill4 = true;

    // Ŭ������ ��ų Ȱ��ȭ���� ����(Ȱ��ȭ �� ��ų ��Ÿ�� 0�� ��� Ŭ���Ͽ� ���� ��ų ���(useSkill ���))
    [SerializeField] bool skill1NoCoolClick = false;
    [SerializeField] bool skill2NoCoolClick = false;
    [SerializeField] bool skill3NoCoolClick = false;
    [SerializeField] bool skill4NoCoolClick = false;

    // ��ų ��� ����(��ݵǾ������� �ش� ��ų�� NULL)
    [SerializeField, ReadOnly] bool skill1Lock = false;
    [SerializeField, ReadOnly] bool skill2Lock = false;
    [SerializeField, ReadOnly] bool skill3Lock = false;
    [SerializeField, ReadOnly] bool skill4Lock = false;

    // ��ų ä�θ� ����(�ִϸ��̼� ���� ���)
    [SerializeField, ReadOnly] bool skill1Channeling = false;
    [SerializeField, ReadOnly] bool skill2Channeling = false;
    [SerializeField, ReadOnly] bool skill3Channeling = false;
    [SerializeField, ReadOnly] bool skill4Channeling = false;

    // ��ų �� �� ���� �ð�
    [SerializeField, ReadOnly] float timeSinceLastSkill1 = -1f;
    [SerializeField, ReadOnly] float timeSinceLastSkill2 = -1f;
    [SerializeField, ReadOnly] float timeSinceLastSkill3 = -1f;
    [SerializeField, ReadOnly] float timeSinceLastSkill4 = -1f;

    [SerializeField, ReadOnly] Animator animator;
    [SerializeField, ReadOnly] CHUnitBase unitBase;
    [SerializeField, ReadOnly] CHTargetTracker targetTracker;
    [SerializeField, ReadOnly] NavMeshAgent agent;

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

    public Animator GetAnimator()
    {
        return animator;
    }
    public float GetTimeSinceLastSkill1() { return timeSinceLastSkill1; }
    public float GetTimeSinceLastSkill2() { return timeSinceLastSkill2; }
    public float GetTimeSinceLastSkill3() { return timeSinceLastSkill3; }
    public float GetTimeSinceLastSkill4() { return timeSinceLastSkill4; }

    public void UseSkill1(bool _use)
    {
        useSkill1 = _use;
    }

    public void OpenSkill2()
    {
        useSkill2 = true;
        skill2Lock = false;
    }

    public void OpenSkill3()
    {
        useSkill3 = true;
        skill3Lock = false;
    }

    public void OpenSkill4()
    {
        useSkill4 = true;
        skill4Lock = false;
    }

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

        agent = gameObject.GetOrAddComponent<NavMeshAgent>();
        unitBase = gameObject.GetOrAddComponent<CHUnitBase>();
        targetTracker = gameObject.GetOrAddComponent<CHTargetTracker>();
        if (unitBase != null && targetTracker != null)
        {
            if (unitBase.GetOriginSkill1Data() == null) skill1Lock = true;
            if (unitBase.GetOriginSkill2Data() == null) skill2Lock = true;
            if (unitBase.GetOriginSkill3Data() == null) skill3Lock = true;
            if (unitBase.GetOriginSkill4Data() == null) skill4Lock = true;

            targetTracker.InitValue(unitBase);

            timeSinceLastSkill1 = -1f;
            timeSinceLastSkill2 = -1f;
            timeSinceLastSkill3 = -1f;
            timeSinceLastSkill4 = -1f;

            gameObject.UpdateAsObservable().Subscribe(async _ =>
            {
                // �װų� ���� �ִ� ���°� �ƴ� �� (��� ���� ���)
                if (unitBase.GetIsDeath() && transform.position.y >= 0.1f)
                {
                    return;
                }

                Infomation.TargetInfo mainTarget = targetTracker.GetClosestTargetInfo();

                if (timeSinceLastSkill1 >= 0f && timeSinceLastSkill1 < unitBase.GetCurrentSkill1CoolTime())
                {
                    timeSinceLastSkill1 += Time.deltaTime;
                    if (unitBase.coolTimeGaugeBar)
                    {
                        unitBase.coolTimeGaugeBar.SetGaugeBar(unitBase.GetCurrentSkill1CoolTime(), timeSinceLastSkill1, 0, 0, 0);
                    }
                }
                else
                {
                    timeSinceLastSkill1 = -1f;
                    if (unitBase.coolTimeGaugeBar)
                    {
                        unitBase.coolTimeGaugeBar.SetGaugeBar(unitBase.GetCurrentSkill1CoolTime(), unitBase.GetCurrentSkill1CoolTime(), 0, 0, 0);
                    }
                }

                if (timeSinceLastSkill2 >= 0f && timeSinceLastSkill2 < unitBase.GetCurrentSkill2CoolTime())
                {
                    timeSinceLastSkill2 += Time.deltaTime;
                }
                else
                {
                    timeSinceLastSkill2 = -1f;
                }

                if (timeSinceLastSkill3 >= 0f && timeSinceLastSkill3 < unitBase.GetCurrentSkill3CoolTime())
                {
                    timeSinceLastSkill3 += Time.deltaTime;
                }
                else
                {
                    timeSinceLastSkill3 = -1f;
                }

                if (timeSinceLastSkill4 >= 0f && timeSinceLastSkill4 < unitBase.GetCurrentSkill4CoolTime())
                {
                    timeSinceLastSkill4 += Time.deltaTime;
                }
                else
                {
                    timeSinceLastSkill4 = -1f;
                }

                if (mainTarget == null || mainTarget.objTarget == null)
                {
                    if (animator) animator.SetBool(attackRange, false);
                }
                // Ÿ���� ���� �ȿ� ������ ��� ���� �� ���� ������ ����
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

                    // 1�� ��ų
                    if ((skill1Lock == false) && useSkill1 && unitBase.IsNormalState())
                    {
                        if ((skill1Channeling == false) && (timeSinceLastSkill1 < 0f) && (mainTarget.distance <= unitBase.GetCurrentSkill1Distance()))
                        {
                            if (animator)
                            {
                                animator.SetTrigger(attackRange);
                                
                                if (unitBase.GetOriginSkill1Data().isChanneling)
                                {
                                    // �ִϸ��̼� ���� �ð����� ä�θ�
                                    skill1Channeling = true;

                                    // �ϴ� ��� ��ų�� ���� �ִϸ��̼����� ���������� ���� Ȱ��� ���������� �ִϸ��̼� ���� ��Ƽ� Ȱ�� ����
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
                            }, unitBase.GetOriginSkill1Data().eSkill);

                            if (skill1NoCoolClick == true)
                            {
                                useSkill1 = false;
                            }
                            else
                            {
                                // ��ų ��Ÿ�� �ʱ�ȭ
                                timeSinceLastSkill1 = 0.0001f;
                            }
                        }
                    }

                    // 2�� ��ų
                    if ((skill2Lock == false) && useSkill2 && unitBase.IsNormalState())
                    {
                        if ((skill2Channeling == false) && timeSinceLastSkill2 < 0f && mainTarget.distance <= unitBase.GetCurrentSkill2Distance())
                        {
                            if (animator)
                            {
                                animator.SetTrigger(attackRange);

                                if (unitBase.GetOriginSkill2Data().isChanneling)
                                {
                                    // �ִϸ��̼� ���� �ð����� ä�θ�
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
                            }, unitBase.GetOriginSkill2Data().eSkill);

                            if (skill2NoCoolClick == true)
                            {
                                useSkill2 = false;
                            }
                            else
                            {
                                // ��ų ��Ÿ�� �ʱ�ȭ
                                timeSinceLastSkill2 = 0.0001f;
                            }
                        }
                    }

                    // 3�� ��ų
                    if ((skill3Lock == false) && useSkill3 && unitBase.IsNormalState())
                    {
                        if ((skill3Channeling == false) && timeSinceLastSkill3 < 0f && mainTarget.distance <= unitBase.GetCurrentSkill3Distance())
                        {
                            if (animator)
                            {
                                animator.SetTrigger(attackRange);

                                if (unitBase.GetOriginSkill3Data().isChanneling)
                                {
                                    // �ִϸ��̼� ���� �ð����� ä�θ�
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
                            }, unitBase.GetOriginSkill3Data().eSkill);

                            if (skill3NoCoolClick == true)
                            {
                                useSkill3 = false;
                            }
                            else
                            {
                                // ��ų ��Ÿ�� �ʱ�ȭ
                                timeSinceLastSkill3 = 0.0001f;
                            }
                        }
                    }

                    // 4�� ��ų
                    if ((skill4Lock == false) && useSkill4 && unitBase.IsNormalState())
                    {
                        if ((skill4Channeling == false) && timeSinceLastSkill4 < 0f && mainTarget.distance <= unitBase.GetCurrentSkill4Distance())
                        {
                            if (animator)
                            {
                                animator.SetTrigger(attackRange);

                                if (unitBase.GetOriginSkill4Data().isChanneling)
                                {
                                    // �ִϸ��̼� ���� �ð����� ä�θ�
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
                            }, unitBase.GetOriginSkill4Data().eSkill);

                            if (skill4NoCoolClick == true)
                            {
                                useSkill4 = false;
                            }
                            else
                            {
                                // ��ų ��Ÿ�� �ʱ�ȭ
                                timeSinceLastSkill4 = 0.0001f;
                            }
                        }
                    }
                }
            }).AddTo(this);
        }
    }

    public async void VarianceMoveSpeed(float _changeSpeed, float _time)
    {
        if (agent == null)
            return;

        agent.speed += _changeSpeed;

        await Task.Delay((int)(_time * 1000));

        if (agent == null)
            return;

        agent.speed -= _changeSpeed;
    }
}

using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

public class CHTargetTracker : MonoBehaviour
{
    [Header("Ÿ�� ���� ����")]
    public Defines.EStandardAxis standardAxis; // ���� ������ �� ��
    public LayerMask targetMask; // Ÿ���� �� ���̾�
    public LayerMask ignoreMask; // ������ ���̾�
    public float range; // Ÿ���� ������ ����
    public float rangeMulti = 2; // Ÿ���� ���� �� �þ�� �þ� ���
    public float rangeMultiTime = 3; // Ÿ���� ���� �� �þ߰� �þ�� �ð�(��)
    [Range(0, 360)] public float viewAngle; // Ÿ���� ������ �þ߰�
    public bool viewEditor; // ������ �󿡼� �þ߰� Ȯ�� ����

    [Header("���� ��")]
    [SerializeField, ReadOnly] float orgRangeMulti = -1f;
    [SerializeField, ReadOnly] float orgViewAngle = -1f;

    [Header("��ų �����Ÿ�")]
    [SerializeField, ReadOnly] float skill1Distance = -1f;

    [Header("�þ� Ȯ�� ����")]
    [SerializeField, ReadOnly] bool expensionRange = false;

    [Header("���� ������Ʈ")]
    [SerializeField, ReadOnly] NavMeshAgent agent;
    [SerializeField, ReadOnly] Animator animator;
    [SerializeField, ReadOnly] CHUnitBase unitBase;
    [SerializeField, ReadOnly] CHContBase contBase;
    [SerializeField, ReadOnly] Infomation.TargetInfo closestTarget = new Infomation.TargetInfo();

    public Action actArrived { get; set; }

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
        unitBase = gameObject.GetComponent<CHUnitBase>();
        contBase = gameObject.GetComponent<CHContBase>();
    }

    private void Start()
    {
        InitValue(unitBase);

        // Ÿ�� ����
        gameObject.UpdateAsObservable().Subscribe(_ =>
        {
            // ��Ȱ��ȭ �Ǿ������� Ÿ�� ���� X
            if (gameObject.activeSelf == false)
                return;

            // �׾����� Ÿ�� ���� X
            bool isDead = unitBase == null ? false : unitBase.GetIsDeath();
            if (isDead)
                return;

            // �þ� ���� �ȿ� ���� Ÿ�� �� ���� ����� Ÿ�� ����
            switch (standardAxis)
            {
                case Defines.EStandardAxis.X:
                    {
                        closestTarget = GetClosestTargetInfo(transform.position, transform.right, targetMask, range * rangeMulti, viewAngle);
                    }
                    break;
                case Defines.EStandardAxis.Z:
                    {
                        closestTarget = GetClosestTargetInfo(transform.position, transform.forward, targetMask, range * rangeMulti, viewAngle);
                    }
                    break;
            }

            // ������ Ÿ���� ���� ���
            if (closestTarget.objTarget == null)
            {
                SetExpensionRange(false);
                StopRunAnim();
            }
            // ������ Ÿ���� �ִ� ���
            else
            {
                SetExpensionRange(true);

                // ��ų �����Ÿ� ���� ������ ���ߵ��� ����
                agent.stoppingDistance = skill1Distance;

                // ���� ������ �����̸�(CC �� �� �ɷ��ִ� ��������)
                if (unitBase.IsNormalState())
                {
                    // ��ų �����Ÿ� �ۿ� �ִ� ���
                    if (closestTarget.distance > skill1Distance)
                    {
                        // Ÿ���� ���� �޸��� �ִ� ���̸�
                        if (agent.isOnNavMesh && IsRunAnimPlaying())
                        {
                            // Ÿ�� ��ġ�� �����Ͽ� �i�ư�
                            agent.SetDestination(closestTarget.objTarget.transform.position);
                        }
                        else
                        {
                            // Ÿ���� ���� �ٶ󺸴� �͸� ����
                            LookAtPosition(closestTarget.objTarget.transform.position);
                        }

                        PlayRunAnim();
                    }
                    // ��ų �����Ÿ� �ȿ� �ִ� ���
                    else
                    {
                        LookAtPosition(closestTarget.objTarget.transform.position);
                        StopRunAnim();
                    }
                }
            }
        }).AddTo(this);
    }

    void OnDrawGizmos()
    {
        bool isDead = false;
        if (unitBase) isDead = unitBase.GetIsDeath();

        if (viewEditor && isDead == false)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, range * rangeMulti);

            // �þ߰��� ��輱
            Vector3 left = transform.Angle(-viewAngle * 0.5f, standardAxis);
            Vector3 right = transform.Angle(viewAngle * 0.5f, standardAxis);

            Debug.DrawRay(transform.position, left * range, Color.green);
            Debug.DrawRay(transform.position, right * range, Color.green);
        }
    }

    public void InitValue(CHUnitBase unitBase)
    {
        if (unitBase == null)
            return;

        this.unitBase = unitBase;

        range = this.unitBase.GetCurrentRange();
        rangeMulti = this.unitBase.GetCurrentRangeMulti();
        orgRangeMulti = rangeMulti;
        rangeMulti = 1f;

        viewAngle = this.unitBase.GetCurrentViewAngle();
        orgViewAngle = viewAngle;

        agent.speed = this.unitBase.GetCurrentMoveSpeed();
        agent.angularSpeed = this.unitBase.GetCurrentRotateSpeed();
        skill1Distance = this.unitBase.GetCurrentSkill1Distance();
    }

    void LookAtPosition(Vector3 _pos)
    {
        var posTarget = _pos;
        var posMy = transform.position;

        posTarget.y = 0f;
        posMy.y = 0f;

        switch (standardAxis)
        {
            case Defines.EStandardAxis.X:
                {
                    transform.right = posTarget - posMy;
                }
                break;
            case Defines.EStandardAxis.Z:
                {
                    transform.forward = posTarget - posMy;
                }
                break;
        }
    }

    bool IsRunAnimPlaying()
    {
        if (animator == null)
            return true;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // �ִϸ��̼��� �ؽ� �� ��
        if (stateInfo.IsName("Run"))
        {
            // �ִϸ��̼��� ��� �ð� ��
            if (stateInfo.normalizedTime < 1f)
            {
                return true;
            }
        }

        return false;
    }

    void PlayRunAnim()
    {
        if (contBase && animator)
        {
            animator.SetBool(contBase.sightRange, true);
        }
    }

    void StopRunAnim()
    {
        if (contBase && animator)
        {
            animator.SetBool(contBase.sightRange, false);
        }

        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
    }

    public async void SetExpensionRange(bool active)
    {
        if (active)
        {
            expensionRange = true;
            viewAngle = 360f;
            rangeMulti = orgRangeMulti;
        }
        else
        {
            await Task.Delay((int)(rangeMultiTime * 1000));

            expensionRange = false;
            viewAngle = orgViewAngle;
            rangeMulti = 1f;
        }
    }

    public Infomation.TargetInfo GetClosestTargetInfo()
    {
        return closestTarget;
    }

    public List<Infomation.TargetInfo> GetTargetInfoListInRange(Vector3 _originPos, Vector3 _direction, LayerMask _lmTarget, float _range, float _viewAngle = 360f)
    {
        List<Infomation.TargetInfo> targetInfoList = new List<Infomation.TargetInfo>();

        // �������� �ִ� Ÿ�ٵ� Ȯ��
        Collider[] targets = Physics.OverlapSphere(_originPos, _range, _lmTarget);

        foreach (Collider target in targets)
        {
            Vector3 posTarget = target.transform.position;
            posTarget.y = 0f;
            _originPos.y = 0f;
            Vector3 dirTarget = (posTarget - _originPos).normalized;

            // �þ߰��� �ɸ����� Ȯ��
            if (Vector3.Angle(_direction, dirTarget) <= _viewAngle / 2)
            {
                float targetDis = Vector3.Distance(_originPos, posTarget);

                // ��ֹ��� �ִ��� Ȯ��
                if (Physics.Raycast(_originPos, dirTarget, targetDis, ~(_lmTarget | ignoreMask)) == false)
                {
                    var unitBase = target.GetComponent<CHUnitBase>();
                    // Ÿ���� ��������� Ÿ������ ����
                    if (unitBase != null && unitBase.GetIsDeath() == false)
                    {
                        targetInfoList.Add(new Infomation.TargetInfo
                        {
                            objTarget = target.gameObject,
                            distance = targetDis,
                        });
                    }
                }
            }
        }

        return targetInfoList;
    }

    public Infomation.TargetInfo GetClosestTargetInfo(Vector3 _originPos, Vector3 _direction, LayerMask _lmTarget, float _range, float _viewAngle = 360f)
    {
        Infomation.TargetInfo closestTargetInfo = new Infomation.TargetInfo();
        List<Infomation.TargetInfo> targetInfoList = GetTargetInfoListInRange(_originPos, _direction, _lmTarget, _range, _viewAngle);

        if (targetInfoList.Count > 0)
        {
            float minDis = Mathf.Infinity;

            foreach (Infomation.TargetInfo targetInfo in targetInfoList)
            {
                if (targetInfo.distance < minDis)
                {
                    minDis = targetInfo.distance;
                    closestTargetInfo = targetInfo;
                }
            }
        }

        return closestTargetInfo;
    }
}

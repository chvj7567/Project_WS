using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static Infomation;

public class CHTargetTracker : MonoBehaviour
{
    // Ÿ���� �� ���̾� ����ũ
    public LayerMask targetMask;
    // ������ ���̾� ����ũ
    public LayerMask ignoreMask;
    // Ÿ���� ������ ����
    public float range;
    // Ÿ���� ���� �� �þ�� �þ� ���
    public float rangeMulti;
    // Ÿ���� ������ �þ߰�
    [Range(0, 360)] public float viewAngle;
    // ������ ��ġ
    public Transform trDestination;
    // ������ �󿡼� �þ߰� Ȯ�� ����
    public bool viewEditor;

    [SerializeField, ReadOnly] NavMeshAgent agent;
    [SerializeField, ReadOnly] CHUnitBase unitBase;
    [SerializeField, ReadOnly] Animator animator;
    [SerializeField, ReadOnly] CHContBase contBase;
    [SerializeField, ReadOnly] TargetInfo closestTarget;

    [SerializeField, ReadOnly] float orgRangeMulti = -1f;
    [SerializeField, ReadOnly] float orgViewAngle = -1f;
    [SerializeField, ReadOnly] bool expensionRange = false;
    public void ResetValue(CHUnitBase _unitBase)
    {
        unitBase = _unitBase;

        range = unitBase.GetCurrentRange();
        rangeMulti = unitBase.GetCurrentRangeMulti();
        orgRangeMulti = rangeMulti;
        rangeMulti = 1f;

        viewAngle = unitBase.GetCurrentViewAngle();
        orgViewAngle = viewAngle;

        if (agent)
        {
            agent.speed = unitBase.GetCurrentMoveSpeed();
            agent.angularSpeed = unitBase.GetCurrentRotateSpeed();
            agent.stoppingDistance = unitBase.GetCurrentAttackDistance();
        }
    }

    private void Awake()
    {
        // �þ �þ� ��� ����
        orgRangeMulti = rangeMulti;
        rangeMulti = 1f;

        // �þ߰� ����
        orgViewAngle = viewAngle;

        agent = gameObject.GetOrAddComponent<NavMeshAgent>();
    }

    private void Start()
    {
        gameObject.UpdateAsObservable().Subscribe(_ =>
        {
            bool isDead = false;
            if (unitBase) isDead = unitBase.GetIsDeath();

            // �ڱⰡ ������� ���� Ÿ�� ����
            if (isDead == false)
            {
                // �þ� ���� �ȿ� ���� Ÿ�� �� ���� ����� Ÿ�� ����
                closestTarget = GetClosestTargetInfo(transform.position, transform.forward, targetMask, range * rangeMulti, viewAngle);

                if (closestTarget == null)
                {
                    if (expensionRange == false)
                    {
                        viewAngle = orgViewAngle;
                        rangeMulti = 1f;
                    }
                    else
                    {
                        viewAngle = 360f;
                        rangeMulti = orgRangeMulti;
                    }

                    if (trDestination && unitBase.IsNormalState()) agent.SetDestination(trDestination.position);

                    if (unitBase.IsNormalState())
                    {
                        PlayRunAnim();
                    }
                    else
                    {
                        StopRunAnim();
                    }
                }
                else
                {
                    // Ÿ�� �߰� �� �þ߰��� range�� ����������� ���� ���� ����
                    viewAngle = 360f;
                    // Ÿ�� �߰� �� �þ� �ش� �����ŭ ����
                    rangeMulti = orgRangeMulti;

                    if (unitBase.IsNormalState()) agent.SetDestination(closestTarget.objTarget.transform.position);

                    if (closestTarget.distance > unitBase.GetCurrentAttackDistance() && unitBase.IsNormalState())
                    {
                        PlayRunAnim();
                    }
                    else
                    {
                        var posTarget = closestTarget.objTarget.transform.position;
                        var myTarget = transform.position;
                        posTarget.y = 0f;
                        myTarget.y = 0f;

                        transform.forward = posTarget - myTarget;
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
            Vector3 left = Angle(-viewAngle * 0.5f);
            Vector3 right = Angle(viewAngle * 0.5f);

            Debug.DrawRay(transform.position, left * range, Color.green);
            Debug.DrawRay(transform.position, right * range, Color.green);
        }
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
    }

    Vector3 Angle(float angle)
    {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public async void ExpensionRange()
    {
        expensionRange = true;

        // 1�� ���� ���� ���� Ȯ��
        await Task.Delay(1000);

        expensionRange = false;
    }

    public TargetInfo GetClosestTargetInfo()
    {
        return closestTarget;
    }

    public List<TargetInfo> GetTargetInfoListInRange(Vector3 _originPos, Vector3 _direction, LayerMask _lmTarget, float _range, float _viewAngle = 360f)
    {
        List<TargetInfo> targetInfoList = new List<TargetInfo>();

        // �������� �ִ� Ÿ�ٵ� Ȯ��
        Collider[] targets = Physics.OverlapSphere(_originPos, _range, _lmTarget);

        foreach (Collider target in targets)
        {
            Vector3 posTarget = target.transform.position;
            posTarget.y = 0f;
            _originPos.y = 0f;
            Vector3 dirTarget = (posTarget - _originPos).normalized;

            // �þ߰��� �ɸ����� Ȯ��
            if (Vector3.Angle(_direction, dirTarget) < _viewAngle / 2)
            {
                float targetDis = Vector3.Distance(_originPos, posTarget);

                // ��ֹ��� �ִ��� Ȯ��
                if (Physics.Raycast(_originPos, dirTarget, targetDis, ~(_lmTarget | ignoreMask)) == false)
                {
                    var unitBase = target.GetComponent<CHUnitBase>();
                    // Ÿ���� ��������� Ÿ������ ����
                    if (unitBase != null && unitBase.GetIsDeath() == false)
                    {
                        targetInfoList.Add(new TargetInfo
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

    public TargetInfo GetClosestTargetInfo(Vector3 _originPos, Vector3 _direction, LayerMask _lmTarget, float _range, float _viewAngle = 360f)
    {
        TargetInfo closestTargetInfo = null;
        List<TargetInfo> targetInfoList = GetTargetInfoListInRange(_originPos, _direction, _lmTarget, _range, _viewAngle);

        if (targetInfoList.Count > 0)
        {
            float minDis = Mathf.Infinity;

            foreach (TargetInfo targetInfo in targetInfoList)
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

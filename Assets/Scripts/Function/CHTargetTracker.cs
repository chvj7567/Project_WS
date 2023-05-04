using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

public class CHTargetTracker : MonoBehaviour
{
    // ������ �� ��
    public Defines.EStandardAxis standardAxis;
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
    [SerializeField, ReadOnly] Animator animator;

    [SerializeField, ReadOnly] CHUnitBase unitBase;
    [SerializeField, ReadOnly] CHContBase contBase;

    [SerializeField, ReadOnly] Infomation.TargetInfo closestTarget;

    [SerializeField, ReadOnly] float orgRangeMulti = -1f;
    [SerializeField, ReadOnly] float orgViewAngle = -1f;
    [SerializeField, ReadOnly] bool expensionRange = false;
    public void ResetValue(CHUnitBase _unitBase)
    {
        unitBase = _unitBase;

        range = unitBase.GetOriginRange();
        rangeMulti = unitBase.GetOriginRangeMulti();
        orgRangeMulti = rangeMulti;
        rangeMulti = 1f;

        viewAngle = unitBase.GetOriginViewAngle();
        orgViewAngle = viewAngle;

        agent = gameObject.GetOrAddComponent<NavMeshAgent>();
        agent.speed = unitBase.GetOriginMoveSpeed();
        agent.angularSpeed = unitBase.GetOriginRotateSpeed();
        agent.stoppingDistance = unitBase.GetOriginAttackDistance();
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

                    if (unitBase != null && unitBase.IsNormalState() && trDestination)
                    {
                        if (agent.isOnNavMesh)
                        {
                            agent.SetDestination(trDestination.position);
                        }
                        else
                        {
                            LookAtPosition(trDestination.position);
                        }
                        
                        PlayRunAnim();
                    }
                    else
                    {
                        if (agent.isOnNavMesh)
                        {
                            agent.ResetPath();
                        }

                        StopRunAnim();
                    }
                }
                else
                {
                    // Ÿ�� �߰� �� �þ߰��� range�� ����������� ���� ���� ����
                    viewAngle = 360f;
                    // Ÿ�� �߰� �� �þ� �ش� �����ŭ ����
                    rangeMulti = orgRangeMulti;

                    if (closestTarget.distance > unitBase.GetOriginAttackDistance() && unitBase.IsNormalState())
                    {
                        if (agent.isOnNavMesh)
                        {
                            agent.SetDestination(closestTarget.objTarget.transform.position);
                        }
                        else
                        {
                            LookAtPosition(closestTarget.objTarget.transform.position);
                        }

                        PlayRunAnim();
                    }
                    else
                    {
                        LookAtPosition(closestTarget.objTarget.transform.position);

                        if (agent.isOnNavMesh)
                        {
                            agent.ResetPath();
                        }
                        
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

    public async void ExpensionRange()
    {
        expensionRange = true;

        // 10�� ���� ���� ���� Ȯ��
        await Task.Delay(10000);

        expensionRange = false;
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
        Infomation.TargetInfo closestTargetInfo = null;
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

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
    // 기준이 될 축
    public Defines.EStandardAxis standardAxis;
    // 타겟이 될 레이어 마스크
    public LayerMask targetMask;
    // 무시할 레이어 마스크
    public LayerMask ignoreMask;
    // 타겟을 감지할 범위
    public float range;
    // 타겟을 감지 후 늘어나는 시야 배수
    public float rangeMulti;
    // 타겟을 감지할 시야각
    [Range(0, 360)] public float viewAngle;
    // 가야할 위치
    public List<Transform> destList = new List<Transform>();
    // 에디터 상에서 시야각 확인 여부
    public bool viewEditor;

    [SerializeField, ReadOnly] NavMeshAgent agent;
    [SerializeField, ReadOnly] Animator animator;

    [SerializeField, ReadOnly] CHUnitBase unitBase;
    [SerializeField, ReadOnly] CHContBase contBase;

    [SerializeField, ReadOnly] Infomation.TargetInfo closestTarget = new Infomation.TargetInfo();

    [SerializeField, ReadOnly] float orgStoppingDistance = -1f;
    [SerializeField, ReadOnly] float orgRangeMulti = -1f;
    [SerializeField, ReadOnly] float orgViewAngle = -1f;
    [SerializeField, ReadOnly] bool expensionRange = false;
    [SerializeField, ReadOnly] int curDestinationIndex = 0;

    public void UpdateSkillValue()
    {
        orgStoppingDistance = unitBase.GetCurrentSkill1Distance();
    }

    public void ResetValue(CHUnitBase _unitBase)
    {
        if (_unitBase == null) return;

        unitBase = _unitBase;

        range = unitBase.GetCurrentRange();
        rangeMulti = unitBase.GetCurrentRangeMulti();
        orgRangeMulti = rangeMulti;
        rangeMulti = 1f;

        viewAngle = unitBase.GetCurrentViewAngle();
        orgViewAngle = viewAngle;

        agent.speed = unitBase.GetCurrentMoveSpeed();
        agent.angularSpeed = unitBase.GetCurrentRotateSpeed();
        orgStoppingDistance = unitBase.GetCurrentSkill1Distance();
    }

    private void Awake()
    {
        contBase = gameObject.GetComponent<CHContBase>();
        unitBase = gameObject.GetComponent<CHUnitBase>();
        animator = gameObject.GetComponent<Animator>();
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        gameObject.UpdateAsObservable().Subscribe(_ =>
        {
            bool isDead = false;
            if (unitBase)
            {
                isDead = unitBase.GetIsDeath();
            }

            if (isDead)
            {
                return;
            }

            // 시야 범위 안에 들어온 타겟 중 제일 가까운 타겟 감지
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

            if (closestTarget.objTarget == null)
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

                if (unitBase != null && unitBase.IsNormalState() && destList.Count > 0)
                {
                    var distance = Vector3.Distance(transform.position, destList[curDestinationIndex].position);
                    if (distance < agent.radius * 2)
                    {
                        agent.isStopped = true;
                        agent.velocity = Vector3.zero;
                        if (destList.Count > curDestinationIndex + 1)
                        {
                            ++curDestinationIndex;
                        }
                    }
                    else
                    {
                        agent.isStopped = false;
                    }

                    if (agent.isOnNavMesh && IsRunAnimPlaying())
                    {
                        agent.stoppingDistance = 0f;
                        agent.SetDestination(destList[curDestinationIndex].position);
                    }
                    else
                    {
                        LookAtPosition(destList[curDestinationIndex].position);
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
                // 타겟 발견 시 시야각을 range를 벗어나기전에는 각도 제한 삭제
                viewAngle = 360f;
                // 타겟 발견 시 시야 해당 배수만큼 증가
                rangeMulti = orgRangeMulti;
                agent.stoppingDistance = orgStoppingDistance;

                if (closestTarget.distance > orgStoppingDistance && unitBase.IsNormalState())
                {
                    if (agent.isOnNavMesh && IsRunAnimPlaying())
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

            // 시야각의 경계선
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

    bool IsRunAnimPlaying()
    {
        if (animator == null) return true;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션의 해시 값 비교
        if (stateInfo.IsName("Run"))
        {
            // 애니메이션의 재생 시간 비교
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
    }

    public async void ExpensionRange()
    {
        expensionRange = true;

        // 10초 동안 감지 범위 확장
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

        // 범위내에 있는 타겟들 확인
        Collider[] targets = Physics.OverlapSphere(_originPos, _range, _lmTarget);

        foreach (Collider target in targets)
        {
            Vector3 posTarget = target.transform.position;
            posTarget.y = 0f;
            _originPos.y = 0f;
            Vector3 dirTarget = (posTarget - _originPos).normalized;

            // 시야각에 걸리는지 확인
            if (Vector3.Angle(_direction, dirTarget) <= _viewAngle / 2)
            {
                float targetDis = Vector3.Distance(_originPos, posTarget);

                // 장애물이 있는지 확인
                if (Physics.Raycast(_originPos, dirTarget, targetDis, ~(_lmTarget | ignoreMask)) == false)
                {
                    var unitBase = target.GetComponent<CHUnitBase>();
                    // 타겟이 살아있으면 타겟으로 지정
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

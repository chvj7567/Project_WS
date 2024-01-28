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
    [Header("타겟 감지 설정")]
    public Defines.EStandardAxis standardAxis; // 정면 기준이 될 축
    public LayerMask targetMask; // 타겟이 될 레이어
    public LayerMask ignoreMask; // 무시할 레이어
    public float range; // 타겟을 감지할 범위
    public float rangeMulti = 2; // 타겟을 감지 후 늘어나는 시야 배수
    public float rangeMultiTime = 3; // 타겟을 감지 후 시야가 늘어나는 시간(초)
    [Range(0, 360)] public float viewAngle; // 타겟을 감지할 시야각
    public bool viewEditor; // 에디터 상에서 시야각 확인 여부

    [Header("원본 값")]
    [SerializeField, ReadOnly] float orgRangeMulti = -1f;
    [SerializeField, ReadOnly] float orgViewAngle = -1f;

    [Header("스킬 사정거리")]
    [SerializeField, ReadOnly] float skill1Distance = -1f;

    [Header("시야 확장 여부")]
    [SerializeField, ReadOnly] bool expensionRange = false;

    [Header("관련 컴포넌트")]
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

        // 타겟 감지
        gameObject.UpdateAsObservable().Subscribe(_ =>
        {
            // 비활성화 되어있으면 타겟 감지 X
            if (gameObject.activeSelf == false)
                return;

            // 죽었으면 타겟 감지 X
            bool isDead = unitBase == null ? false : unitBase.GetIsDeath();
            if (isDead)
                return;

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

            // 감지된 타겟이 없는 경우
            if (closestTarget.objTarget == null)
            {
                SetExpensionRange(false);
                StopRunAnim();
            }
            // 감지된 타겟이 있는 경우
            else
            {
                SetExpensionRange(true);

                // 스킬 사정거리 내에 있으면 멈추도록 설정
                agent.stoppingDistance = skill1Distance;

                // 공격 가능한 상태이면(CC 등 안 걸려있는 상태인지)
                if (unitBase.IsNormalState())
                {
                    // 스킬 사정거리 밖에 있는 경우
                    if (closestTarget.distance > skill1Distance)
                    {
                        // 타겟을 향해 달리고 있는 중이면
                        if (agent.isOnNavMesh && IsRunAnimPlaying())
                        {
                            // 타겟 위치를 갱신하여 쫒아감
                            agent.SetDestination(closestTarget.objTarget.transform.position);
                        }
                        else
                        {
                            // 타겟을 향해 바라보는 것만 갱신
                            LookAtPosition(closestTarget.objTarget.transform.position);
                        }

                        PlayRunAnim();
                    }
                    // 스킬 사정거리 안에 있는 경우
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

            // 시야각의 경계선
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

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

public class CHTargetTracker : MonoBehaviour
{
    public Transform trOrigin;
    // 타겟이 될 레이어 마스크
    public List<LayerMask> liTargetMask;
    // 타겟을 감지할 범위
    public float range;
    // 타겟을 감지 후 늘어나는 시야 배수
    public float rangeMulti;
    // 타겟을 감지할 시야각
    [Range(0, 360)] public float viewAngle;
    // 타겟을 바라보는 속도
    public float rotateSpeed;
    // 타겟을 따라가는 속도
    public float followSpeed;
    // 타겟을 따라갈 때 유지할 거리
    public float approachDistance;
    // 에디터 상에서 시야각 확인 여부
    public bool viewEditor;

    [SerializeField] Animator animator;
    [SerializeField] CHUnitBase unitBase;
    [SerializeField] CHContBase contBase;
    [SerializeField] Infomation.TargetInfo closestTarget;

    float orgRangeMulti = -1f;
    float orgViewAngle = -1f;
    LayerMask targetMask;

    public void ResetViewAngleOrigin()
    {
        orgViewAngle = viewAngle;
    }

    public LayerMask GetTargetMask()
    {
        return targetMask;
    }

    private void Awake()
    {
        // 늘어날 시야 배수 저장
        orgRangeMulti = rangeMulti;
        rangeMulti = 1f;

        // 시야각 저장
        orgViewAngle = viewAngle;

        foreach (LayerMask layerMask in liTargetMask)
        {
            targetMask |= layerMask;
        }

        approachDistance = unitBase.GetCurrentAttackDistance();
    }

    private void Start()
    {
        gameObject.UpdateAsObservable().Subscribe(_ =>
        {
            // 자기가 살아있을 때만 타겟 감지
            if (unitBase.GetIsDeath() == false)
            {
                // 시야 범위 안에드 들어온 타겟 중 제일 가까운 타겟 감지
                closestTarget = CHMMain.Skill.GetClosestTargetInfo(transform.position, transform.forward, targetMask, range * rangeMulti, viewAngle);

                if (closestTarget != null)
                {
                    // 타겟 발견 시 시야각을 range를 벗어나기전에는 각도 제한 삭제
                    viewAngle = 360f;
                    // 타겟 발견 시 시야 해당 배수만큼 증가
                    rangeMulti = orgRangeMulti;
                    // 공격 중일 때는 안 움직이도록
                    bool isAttackAnimating = animator.GetCurrentAnimatorStateInfo(0).IsName(Defines.EUnitAni.Attack1.ToString());
                    if (isAttackAnimating == false)
                    {
                        LookAtTarget(closestTarget.direction);

                        // 공격 범위까지만 다가감
                        if (closestTarget.distance > unitBase.GetOriginAttackDistance())
                        {
                            animator.SetBool(contBase.sightRange, true);

                            // 달리기 애니메이션 중 일때 움직이도록
                            bool isRunAnimating = animator.GetCurrentAnimatorStateInfo(0).IsName(Defines.EUnitAni.Run.ToString());
                            if (isRunAnimating)
                            {
                                FollowTarget(closestTarget.direction);
                            }
                        }
                        else
                        {
                            animator.SetBool(contBase.sightRange, false);
                        }
                    }
                }
                else
                {
                    viewAngle = orgViewAngle;
                    rangeMulti = 1f;
                    animator.SetBool(contBase.sightRange, false);
                }
            }
            else
            {
                animator.SetBool(contBase.sightRange, false);
            }
        }).AddTo(this);
    }

    void OnDrawGizmos()
    {
        if (viewEditor && unitBase.GetIsDeath() == false)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, range * rangeMulti);

            // 시야각의 경계선
            Vector3 left = Angle(-viewAngle * 0.5f);
            Vector3 right = Angle(viewAngle * 0.5f);

            Debug.DrawRay(transform.position, left * range, Color.green);
            Debug.DrawRay(transform.position, right * range, Color.green);
        }
    }

    void LookAtTarget(Vector3 _direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    Vector3 Angle(float angle)
    {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    void FollowTarget(Vector3 _direction)
    {
        transform.position += _direction.normalized * followSpeed * Time.deltaTime;
    }

    public Infomation.TargetInfo GetClosestTargetInfo()
    {
        return closestTarget;
    }

    public List<Infomation.TargetInfo> GetTargetInfoListInRange()
    {
        return CHMMain.Skill.GetTargetInfoListInRange(transform.position, transform.forward, targetMask, range, viewAngle);
    }
}

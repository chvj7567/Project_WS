using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class CHTargetTracker : MonoBehaviour
{
    public Transform trOrigin;
    // 타겟이 될 레이어 마스크
    public List<LayerMask> liTargetMask;
    // 타겟을 감지할 범위
    public float range;
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
    
    [SerializeField, ReadOnly] Infomation.TargetInfo closestTarget;
    float viewAngleOrigin;
    LayerMask targetMask;

    private void Awake()
    {
        // 시야각 저장
        viewAngleOrigin = viewAngle;

        foreach (LayerMask layerMask in liTargetMask)
        {
            targetMask |= layerMask;
        }
    }

    private void Start()
    {
        gameObject.UpdateAsObservable().Subscribe(_ =>
        {
            closestTarget = CHMMain.Skill.GetClosestTargetInfo(transform.position, transform.forward, targetMask, range, viewAngle);

            if (closestTarget != null)
            {
                // 타겟 발견 시 시야각을 range를 벗어나기전에는 각도 제한 삭제
                viewAngle = 360f;

                Vector3 direction = closestTarget.objTarget.transform.position - transform.position;

                LookAtTarget(direction);
                FollowTarget(direction);
            }
            else
            {
                viewAngle = viewAngleOrigin;
            }
        });
    }

    void OnDrawGizmos()
    {
        if (viewEditor)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }

    void LookAtTarget(Vector3 _direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    void FollowTarget(Vector3 _direction)
    {
        float distance = Vector3.Distance(transform.position, closestTarget.objTarget.transform.position);

        if (distance > approachDistance)
        {
            transform.position += _direction.normalized * followSpeed * Time.deltaTime;
        }
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

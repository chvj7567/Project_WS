using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class CHTargetTracker : MonoBehaviour
{
    class TargetInfo
    {
        public GameObject targetObj;
        public Vector3 direction;
        public float distance;
    }

    // 타겟이 될 레이어 마스크
    public LayerMask targetMask;
    // 무시 할 레이어 마스크
    public LayerMask ignoreMask;
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
    
    [SerializeField, ReadOnly] TargetInfo target;
    float viewAngleOrigin;

    private void Awake()
    {
        // 시야각 저장
        viewAngleOrigin = viewAngle;
    }

    private void Start()
    {
        
        gameObject.UpdateAsObservable().Subscribe(_ =>
        {
            target = GetClosestTargetInfo();

            if (target != null)
            {
                // 타겟 발견 시 시야각을 range를 벗어나기전에는 각도 제한 삭제
                viewAngle = 360f;

                Vector3 direction = target.targetObj.transform.position - transform.position;

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
        float distance = Vector3.Distance(transform.position, target.targetObj.transform.position);

        if (distance > approachDistance)
        {
            transform.position += _direction.normalized * followSpeed * Time.deltaTime;
        }
    }

    TargetInfo GetClosestTargetInfo()
    {
        TargetInfo tempTarget = null;
        List<TargetInfo> targetInfoList = GetTargetInfoListInRange();

        if (targetInfoList.Count > 0)
        {
            float minDis = Mathf.Infinity;

            foreach (TargetInfo targetInfo in targetInfoList)
            {
                if (targetInfo.distance < minDis)
                {
                    minDis = targetInfo.distance;
                    tempTarget = targetInfo;
                }
            }
        }

        return tempTarget;
    }

    List<TargetInfo> GetTargetInfoListInRange()
    {
        List<TargetInfo> targetInfoList = new List<TargetInfo>();

        // 시야각의 경계선
        Vector3 left = Angle(-viewAngle * 0.5f);
        Vector3 right = Angle(viewAngle * 0.5f);

        if (viewEditor)
        {
            Debug.DrawRay(transform.position, left * range, Color.green);
            Debug.DrawRay(transform.position, right * range, Color.green);
        }

        // 범위내에 있는 타겟들 확인
        Collider[] targets = Physics.OverlapSphere(transform.position, range, targetMask);

        foreach (Collider target in targets)
        {
            Transform targetTr = target.transform;
            Vector3 targetDir = (targetTr.position - transform.position).normalized;

            // 시야각에 걸리는지 확인
            if (Vector3.Angle(transform.forward, targetDir) < viewAngle / 2)
            {
                float targetDis = Vector3.Distance(transform.position, targetTr.position);

                // 장애물이 있는지 확인
                if (Physics.Raycast(transform.position, targetDir, targetDis, targetMask & ignoreMask) == false)
                {
                    if (viewEditor)
                    {
                        Debug.DrawRay(transform.position, targetDir * targetDis, Color.red);
                    }

                    targetInfoList.Add(new TargetInfo
                    {
                        targetObj = target.gameObject,
                        direction = targetDir,
                        distance = targetDis,
                    });
                }
            }
        }

        return targetInfoList;
    }

    Vector3 Angle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

}

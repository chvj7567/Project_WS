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

    // Ÿ���� �� ���̾� ����ũ
    public LayerMask targetMask;
    // ���� �� ���̾� ����ũ
    public LayerMask ignoreMask;
    // Ÿ���� ������ ����
    public float range;
    // Ÿ���� ������ �þ߰�
    [Range(0, 360)] public float viewAngle;
    // Ÿ���� �ٶ󺸴� �ӵ�
    public float rotateSpeed;
    // Ÿ���� ���󰡴� �ӵ�
    public float followSpeed;
    // Ÿ���� ���� �� ������ �Ÿ�
    public float approachDistance;
    // ������ �󿡼� �þ߰� Ȯ�� ����
    public bool viewEditor;
    
    [SerializeField, ReadOnly] TargetInfo target;
    float viewAngleOrigin;

    private void Awake()
    {
        // �þ߰� ����
        viewAngleOrigin = viewAngle;
    }

    private void Start()
    {
        
        gameObject.UpdateAsObservable().Subscribe(_ =>
        {
            target = GetClosestTargetInfo();

            if (target != null)
            {
                // Ÿ�� �߰� �� �þ߰��� range�� ����������� ���� ���� ����
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

        // �þ߰��� ��輱
        Vector3 left = Angle(-viewAngle * 0.5f);
        Vector3 right = Angle(viewAngle * 0.5f);

        if (viewEditor)
        {
            Debug.DrawRay(transform.position, left * range, Color.green);
            Debug.DrawRay(transform.position, right * range, Color.green);
        }

        // �������� �ִ� Ÿ�ٵ� Ȯ��
        Collider[] targets = Physics.OverlapSphere(transform.position, range, targetMask);

        foreach (Collider target in targets)
        {
            Transform targetTr = target.transform;
            Vector3 targetDir = (targetTr.position - transform.position).normalized;

            // �þ߰��� �ɸ����� Ȯ��
            if (Vector3.Angle(transform.forward, targetDir) < viewAngle / 2)
            {
                float targetDis = Vector3.Distance(transform.position, targetTr.position);

                // ��ֹ��� �ִ��� Ȯ��
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

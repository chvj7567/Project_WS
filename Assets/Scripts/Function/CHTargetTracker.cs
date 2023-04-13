using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class CHTargetTracker : MonoBehaviour
{
    public Transform trOrigin;
    // Ÿ���� �� ���̾� ����ũ
    public List<LayerMask> liTargetMask;
    // ���� �� ���̾� ����ũ
    public List<LayerMask> liIgnoreMask;
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
    
    [SerializeField, ReadOnly] TargetInfo closestTarget;
    float viewAngleOrigin;
    LayerMask targetMask;
    LayerMask ignoreMask;

    private void Awake()
    {
        // �þ߰� ����
        viewAngleOrigin = viewAngle;

        foreach (LayerMask layerMask in liTargetMask)
        {
            targetMask |= layerMask;
        }

        foreach (LayerMask layerMask in liIgnoreMask)
        {
            ignoreMask |= layerMask;
        }
    }

    private void Start()
    {
        gameObject.UpdateAsObservable().Subscribe(_ =>
        {
            closestTarget = CHMMain.Skill.GetClosestTargetInfo(transform, targetMask, ignoreMask, range, viewAngle);

            if (closestTarget != null)
            {
                // Ÿ�� �߰� �� �þ߰��� range�� ����������� ���� ���� ����
                viewAngle = 360f;

                Vector3 direction = closestTarget.targetObj.transform.position - transform.position;

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
        float distance = Vector3.Distance(transform.position, closestTarget.targetObj.transform.position);

        if (distance > approachDistance)
        {
            transform.position += _direction.normalized * followSpeed * Time.deltaTime;
        }
    }

    public TargetInfo GetClosestTargetInfo()
    {
        return closestTarget;
    }

    public List<TargetInfo> GetTargetInfoListInRange()
    {
        return CHMMain.Skill.GetTargetInfoListInRange(transform, targetMask, ignoreMask, range, viewAngle);
    }
}

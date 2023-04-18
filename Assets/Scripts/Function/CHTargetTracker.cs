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
    // Ÿ���� �� ���̾� ����ũ
    public List<LayerMask> liTargetMask;
    // Ÿ���� ������ ����
    public float range;
    // Ÿ���� ���� �� �þ�� �þ� ���
    public float rangeMulti;
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
        // �þ �þ� ��� ����
        orgRangeMulti = rangeMulti;
        rangeMulti = 1f;

        // �þ߰� ����
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
            // �ڱⰡ ������� ���� Ÿ�� ����
            if (unitBase.GetIsDeath() == false)
            {
                // �þ� ���� �ȿ��� ���� Ÿ�� �� ���� ����� Ÿ�� ����
                closestTarget = CHMMain.Skill.GetClosestTargetInfo(transform.position, transform.forward, targetMask, range * rangeMulti, viewAngle);

                if (closestTarget != null)
                {
                    // Ÿ�� �߰� �� �þ߰��� range�� ����������� ���� ���� ����
                    viewAngle = 360f;
                    // Ÿ�� �߰� �� �þ� �ش� �����ŭ ����
                    rangeMulti = orgRangeMulti;
                    // ���� ���� ���� �� �����̵���
                    bool isAttackAnimating = animator.GetCurrentAnimatorStateInfo(0).IsName(Defines.EUnitAni.Attack1.ToString());
                    if (isAttackAnimating == false)
                    {
                        LookAtTarget(closestTarget.direction);

                        // ���� ���������� �ٰ���
                        if (closestTarget.distance > unitBase.GetOriginAttackDistance())
                        {
                            animator.SetBool(contBase.sightRange, true);

                            // �޸��� �ִϸ��̼� �� �϶� �����̵���
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

            // �þ߰��� ��輱
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

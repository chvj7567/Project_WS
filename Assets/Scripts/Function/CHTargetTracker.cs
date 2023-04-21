using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
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
    // Ÿ���� �ٶ󺸴� �ӵ�
    public float rotateSpeed;
    // Ÿ���� ���󰡴� �ӵ�
    public float moveSpeed;
    // Ÿ���� ���� �� ������ �Ÿ�
    public float approachDistance;
    // ������ �󿡼� �þ߰� Ȯ�� ����
    public bool viewEditor;

    [SerializeField, ReadOnly] Animator animator;
    [SerializeField, ReadOnly] CHUnitBase unitBase;
    [SerializeField, ReadOnly] CHContBase contBase;
    [SerializeField, ReadOnly] TargetInfo closestTarget;

    float orgRangeMulti = -1f;
    float orgViewAngle = -1f;

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

        animator = GetComponent<Animator>();
        unitBase = GetComponent<CHUnitBase>();
        contBase = GetComponent<CHContBase>();
    }

    private void Start()
    {
        if (unitBase) approachDistance = unitBase.GetCurrentAttackDistance();

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
                    viewAngle = orgViewAngle;
                    rangeMulti = 1f;
                    if (animator && contBase) animator.SetBool(contBase.sightRange, false);
                }
                else
                {
                    // Ÿ�� �߰� �� �þ߰��� range�� ����������� ���� ���� ����
                    viewAngle = 360f;
                    // Ÿ�� �߰� �� �þ� �ش� �����ŭ ����
                    rangeMulti = orgRangeMulti;
                    // ���� ���� ���� �� �����̵���
                    bool isAttackAnimating = false;
                    if (animator) isAttackAnimating = animator.GetCurrentAnimatorStateInfo(0).IsName(Defines.EUnitAni.Attack1.ToString());

                    
                    if (isAttackAnimating == false)
                    {
                        LookAtTarget(closestTarget.direction);

                        bool isAttackDistance = true;
                        if (unitBase) isAttackDistance = closestTarget.distance > unitBase.GetOriginAttackDistance();

                        // ���� ���������� �ٰ���
                        if (isAttackDistance)
                        {
                            if (animator && contBase) animator.SetBool(contBase.sightRange, true);

                            // �޸��� �ִϸ��̼� �� �϶� �����̵���
                            bool isRunAnimating = true;

                            if (animator) isAttackAnimating = animator.GetCurrentAnimatorStateInfo(0).IsName(Defines.EUnitAni.Attack1.ToString());

                            if (isRunAnimating)
                            {
                                FollowTarget(closestTarget.direction);
                            }
                        }
                        else
                        {
                            if (animator) animator.SetBool(contBase.sightRange, false);
                        }
                    }
                }
            }
            else
            {
                if (animator) animator.SetBool(contBase.sightRange, false);
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
        transform.position += _direction.normalized * moveSpeed * Time.deltaTime;
    }

    public Infomation.TargetInfo GetClosestTargetInfo()
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
                            direction = dirTarget,
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

    public List<TargetInfo> GetTargetInfoListInRange(Vector3 _originPos, LayerMask _lmTarget, Vector3 _size, Quaternion _quater)
    {
        List<TargetInfo> targetInfoList = new List<TargetInfo>();

        // �������� �ִ� Ÿ�ٵ� Ȯ��
        Collider[] targets = Physics.OverlapBox(_originPos, _size / 2f, _quater, _lmTarget);

        foreach (Collider target in targets)
        {
            Transform targetTr = target.transform;
            Vector3 targetDir = (targetTr.position - _originPos).normalized;
            float targetDis = Vector3.Distance(_originPos, targetTr.position);

            // ��ֹ��� �ִ��� Ȯ��
            if (Physics.Raycast(_originPos, targetDir, targetDis, ~_lmTarget) == false)
            {
                targetInfoList.Add(new TargetInfo
                {
                    objTarget = target.gameObject,
                    direction = targetDir,
                    distance = targetDis,
                });
            }
        }

        return targetInfoList;
    }

    public TargetInfo GetClosestTargetInfo(Vector3 _originPos, LayerMask _lmTarget, Vector3 _size, Quaternion _quater)
    {
        TargetInfo closestTargetInfo = null;
        List<TargetInfo> targetInfoList = GetTargetInfoListInRange(_originPos, _lmTarget, _size, _quater);

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

    public List<Transform> GetTargetTransformList(List<TargetInfo> _liTargetInfo)
    {
        if (_liTargetInfo == null) return null;

        List<Transform> targetTransformList = new List<Transform>();
        foreach (TargetInfo targetInfo in _liTargetInfo)
        {
            targetTransformList.Add(targetInfo.objTarget.transform);
        }

        return targetTransformList;
    }

    public List<Transform> GetTargetTransformList(TargetInfo _targetInfo)
    {
        if (_targetInfo == null) return null;

        List<Transform> targetTransformList = new List<Transform>();
        targetTransformList.Add(_targetInfo.objTarget.transform);

        return targetTransformList;
    }
}

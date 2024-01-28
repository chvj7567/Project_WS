using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

public class CHTargetMover : MonoBehaviour
{
    public Defines.EStandardAxis standardAxis; // 정면 기준이 될 축

    public List<Transform> destList = new List<Transform>(); // 가야할 위치

    [SerializeField, ReadOnly] NavMeshAgent agent;
    [SerializeField, ReadOnly] Animator animator;
    [SerializeField, ReadOnly] CHUnitBase unitBase;
    [SerializeField, ReadOnly] CHContBase contBase;

    [SerializeField, ReadOnly] int curDestinationIndex = 0;

    Action actMoveDispose;
    IDisposable disposeMove;

    private void Awake()
    {
        contBase = gameObject.GetComponent<CHContBase>();
        unitBase = gameObject.GetComponent<CHUnitBase>();
        animator = gameObject.GetComponent<Animator>();
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        actMoveDispose += () =>
        {
            if (disposeMove == null)
                return;

            disposeMove.Dispose();
        };
    }

    private void OnDisable()
    {
        actMoveDispose.Invoke();
    }

    public void StartRun()
    {
        agent.SetDestination(destList[curDestinationIndex++].position);

        disposeMove = gameObject.UpdateAsObservable().Subscribe(_ =>
        {
            if (agent.remainingDistance < 0.1f)
            {
                agent.ResetPath();
                curDestinationIndex++;

                if (destList.Count > curDestinationIndex)
                {
                    agent.SetDestination(destList[curDestinationIndex].position);
                }
                else
                {
                    actMoveDispose.Invoke();
                }
            }
        }).AddTo(this);
    }

    public void SetDest(Vector3 pos, List<Transform> destList)
    {
        if (destList == null)
            return;

        agent.velocity = Vector3.zero;
        agent.Warp(pos);
        this.destList = destList;
        curDestinationIndex = 0;
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

        agent.ResetPath();
    }
}

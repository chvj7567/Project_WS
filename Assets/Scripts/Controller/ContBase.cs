using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContBase : MonoBehaviour
{
    [SerializeField] protected bool useAttack = true;
    [SerializeField] protected bool useSkill1 = true;
    [SerializeField] protected bool useSkill2 = true;
    [SerializeField] protected bool useSkill3 = true;
    [SerializeField] protected bool useSkill4 = true;

    [SerializeField, ReadOnly] protected float timeSinceLastAttack = -1f;
    [SerializeField, ReadOnly] protected float timeSinceLastSkill1 = -1f;
    [SerializeField, ReadOnly] protected float timeSinceLastSkill2 = -1f;
    [SerializeField, ReadOnly] protected float timeSinceLastSkill3 = -1f;
    [SerializeField, ReadOnly] protected float timeSinceLastSkill4 = -1f;

    [SerializeField] protected Animator animator;

    [SerializeField, ReadOnly] protected int attackRange = Animator.StringToHash("TargetInAttackRange");
    [SerializeField, ReadOnly] protected int sightRange = Animator.StringToHash("TargetInSightRange");
    public float GetTimeSinceLastAttack() { return timeSinceLastAttack; }
    public float GetTimeSinceLastSkill1() { return timeSinceLastSkill1; }
    public float GetTimeSinceLastSkill2() { return timeSinceLastSkill2; }
    public float GetTimeSinceLastSkill3() { return timeSinceLastSkill3; }
    public float GetTimeSinceLastSkill4() { return timeSinceLastSkill4; }

    public void SetTimeSinceLastAttack(float _time) { timeSinceLastAttack = _time; }
    public void SetTimeSinceLastSkill1(float _time) { timeSinceLastSkill1 = _time; }
    public void SetTimeSinceLastSkill2(float _time) { timeSinceLastSkill2 = _time; }
    public void SetTimeSinceLastSkill3(float _time) { timeSinceLastSkill3 = _time; }
    public void SetTimeSinceLastSkill4(float _time) { timeSinceLastSkill4 = _time; }
}

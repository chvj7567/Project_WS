using System;
using System.Collections.Generic;
using UnityEngine;
using static Defines;

[CreateAssetMenu(menuName = "Scriptable Object/Skill Data", order = 1)]
public class SkillData : ScriptableObject
{
    [Header("��ų")]
    [Tooltip("��ų �̸�")] public ESkill eSkill = ESkill.None;
    [Tooltip("��ų ����"), Multiline(5)] public string skillDesc = null;
    [Tooltip("��ų Ÿ���� ����")] public bool isTargeting = false;
    [Tooltip("��ų ä�θ� ����")] public bool isChanneling = false;
    [Tooltip("��ų �����Ÿ�")] public float distance = -1f;
    [Tooltip("��ų ��Ÿ��")] public float coolTime = -1f;
    [Tooltip("��ų ��� Ÿ��")] public ESkillCost eSkillCost = ESkillCost.None;
    [Tooltip("��ų ���")] public float cost = -1f;
    [Tooltip("����Ʈ ���� ����Ʈ")] public List<EffectData> liEffectData = new List<EffectData>();

    public SkillData Clone()
    {
        SkillData clone = ScriptableObject.CreateInstance<SkillData>();
        clone.eSkill = this.eSkill;
        clone.skillDesc = this.skillDesc;
        clone.isTargeting = this.isTargeting;
        clone.distance = this.distance;
        clone.coolTime = this.coolTime;
        clone.eSkillCost = this.eSkillCost;
        clone.cost = this.cost;
        clone.liEffectData = new List<EffectData>();

        foreach (EffectData effectData in this.liEffectData)
        {
            clone.liEffectData.Add(effectData.Clone());
        }

        return clone;
    }

    [Serializable]
    public class EffectData
    {
        [Header("����Ʈ")]
        [Tooltip("����Ʈ �̸�")] public EEffect eEffect = EEffect.None;
        [Tooltip("����Ʈ ����")] public float effectAngle = -1f;
        [Tooltip("����Ʈ ���� ��ġ�� ������ ��ġ���� ����(�ڱ� �ڽ��� ��ų ȿ�� ���� ���)")] public bool createCasterPosition = false;
        [Tooltip("����Ʈ ���� ������")] public float startDelay = -1f;
        [Tooltip("Ÿ���� ���� ��� ����Ʈ ���� ����")] public bool createOnEmpty = false;
        [Tooltip("����Ʈ ������Ʈ�� Ÿ�� �ڽ� ������Ʈ�� ���� ����")] public bool attach = false;
        [Tooltip("����Ʈ �ߺ� ����")] public bool duplication = false;
        [Tooltip("��Į ǥ�� ����")] public bool onDecal = false;

        [Header("��ų ȿ��")]
        [Tooltip("������ ����")] public EStatModifyType eStatModifyType = EStatModifyType.None;
        [Tooltip("������ ����")] public EDamageType1 eDamageType1 = EDamageType1.None;
        [Tooltip("������ Ÿ��")] public EDamageType2 eDamageType2 = EDamageType2.None;
        [Tooltip("������")] public float damage = -1f;

        [Header("��ų ���")]
        [Tooltip("����Ʈ Ÿ��")] public ETarget eTarget = ETarget.None;
        [Tooltip("����Ʈ Ÿ�� ���̾�")] public ETargetMask eTargetMask = ETargetMask.None;

        [Header("��ų �����ڰ� ��ǥ �������� ������ ���")]
        [Tooltip("��ǥ �������� ������ ����")] public bool moveToPos = false;
        [Tooltip("������ �� ���ǵ�")] public float moveSpeed = -1f;
        [Tooltip("��ǥ �������� offset")] public float offsetToTarget = -1f;

        [Header("�浹ü")]
        [Tooltip("�浹ü ���")] public ECollision eCollision = ECollision.None;
        [Tooltip("�浹ü ����")] public float collisionAngle = -1f;
        [Tooltip("���� �浹ü�� ���")] public float sphereRadius = -1f;
        [Tooltip("�ڽ��� �浹ü�� ���")] public Vector3 boxHalf = Vector3.zero;

        [Header("�浹ü ����")]
        [Tooltip("����Ʈ ���� �� �� �� �ڿ� ��������")] public float triggerStartDelay = -1f;
        [Tooltip("�浹ü ���� �� �� �ʰ� ��������")] public float triggerStayTime = -1f;
        [Tooltip("�浹ü�� ����� ��� ��ų ���� ����")] public bool triggerEnter = false;
        [Tooltip("�浹ü�� ��Ҵٰ� �������� ��� ��ų ���� ����")] public bool triggerExit = false;
        [Tooltip("�浹ü�� ����ִ� ��� ��ų ƽ ���� Ÿ��")] public float stayTickTime = -1f;

        public EffectData Clone()
        {
            EffectData clone = new EffectData();
            clone.eEffect = this.eEffect;
            clone.effectAngle = this.effectAngle;
            clone.createCasterPosition = this.createCasterPosition;
            clone.startDelay = this.startDelay;
            clone.createOnEmpty = this.createOnEmpty;
            clone.attach = this.attach;
            clone.duplication = this.duplication;
            clone.onDecal = this.onDecal;
            clone.eStatModifyType = this.eStatModifyType;
            clone.eDamageType1 = this.eDamageType1;
            clone.eDamageType2 = this.eDamageType2;
            clone.damage = this.damage;
            clone.eTarget = this.eTarget;
            clone.eTargetMask = this.eTargetMask;
            clone.moveToPos = this.moveToPos;
            clone.moveSpeed = this.moveSpeed;
            clone.offsetToTarget = this.offsetToTarget;
            clone.eCollision = this.eCollision;
            clone.collisionAngle = this.collisionAngle;
            clone.sphereRadius = this.sphereRadius;
            clone.boxHalf = this.boxHalf;
            clone.triggerStartDelay = this.triggerStartDelay;
            clone.triggerStayTime = this.triggerStayTime;
            clone.triggerEnter = this.triggerEnter;
            clone.triggerExit = this.triggerExit;
            clone.stayTickTime = this.stayTickTime;
            return clone;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using static Defines;

public class Infomation
{
    [Serializable]
    public class StringInfo
    {
        public int stringID = -1;
        public string value = "";
    }

    [Serializable]
    public class SkillInfo
    {
        public ESkillID eSkillID = ESkillID.None; // ��ų ���̵�
        public int skillDesc = -1; // ��ų ����
        public bool isTargeting = false; // Ÿ���� or ��Ÿ����
        public float distance = -1f; // �����Ÿ�
        public float coolTime = -1f; // ��Ÿ��
        public ESkillCost eSkillCost = ESkillCost.None; // ��ų ����� ���� ��������
        public float cost = -1f; // ��ų ���
        public List<EffectInfo> liEffectInfo = new List<EffectInfo>(); // ��ų �� ��ü
    }

    [Serializable]
    public class EffectInfo
    {
        public EEffect eEffect = EEffect.None; // ����Ʈ ����
        public EEffectType eEffectType = EEffectType.None; // ����Ʈ Ÿ��
        public EEffectPos eEffectPos = EEffectPos.None; // ����Ʈ ��ġ
        public EDamageState eDamageState = EDamageState.None; // ������ ����
        public EDamageType eDamageType = EDamageType.None; // ������ Ÿ��
        public ECollision eCollision = ECollision.None; // �浹ü ���
        public ETargetMask eTargetMask = ETargetMask.None; // ��ų ���
        public bool duplication = false; // ��ų �ߺ� ����
        public bool onDecal = false; // ��Į ǥ�� ����
        public bool moveToPos = false; // true�� ��� moveSpeed�� ���� startDelay�� �ڵ� ���
        public float moveSpeed = -1f; // moveToPos�� true�� ��츸 ���, ������ �ӵ�
        public float offsetToTarget = -1f; // moveToPos�� true�� ��츸 ���, Ÿ�ٰ��� �Ÿ��� �󸶳� ����
        public bool triggerEnter = false; // ����Ʈ Trigger�� ����� �� �������� ���� ������ ����
        public bool triggerExit = false; // ����Ʈ Trigger���� ����� �� �������� ���� ������ ����
        public float stayTickTime = -1f; // ����Ʈ Trigger���� �ӹ��� ���� �� �������� ���� �� ƽ Ÿ��(�����̸� ������ �� ����)
        public float startDelay = -1f; // ��ų ���� �� ����Ʈ ������ �ð�, ���� ���� ��� ���� �ð�
        public float angle = -1f; // �浹 ���� ������ �� ����
        public float sphereRadius = -1f; // �� ����� �浹ü�� ��� ���� ������
        public float boxHalfX = -1f; // �ڽ� ����� �浹ü�� ��� X ũ���� ��
        public float boxHalfY = -1f; // �ڽ� ����� �浹ü�� ��� Y ũ���� ��
        public float boxHalfZ = -1f; // �ڽ� ����� �浹ü�� ��� Z ũ���� ��
        public float damage = -1f; // ������
    }

    [Serializable]
    public class UnitInfo
    {
        public EUnitID eUnitID = EUnitID.None;
        public int nameStringID = -1;
        public float maxHp = -1f;
        public float hp = -1f;
        public float hpRegenPerSecond = -1f;
        public float maxMp = -1f;
        public float mp = -1f;
        public float mpRegenPerSecond = -1f;
        public float attackPower = -1f;
        public float defensePower = -1f;
        public float attackDelay = -1f;
        public float attackDistance = -1f;
        public float moveSpeed = -1f;
        public float rotateSpeed = -1f;
        public float viewAngle = -1f;
        public ESkillID eSkill1ID = ESkillID.None;
        public ESkillID eSkill2ID = ESkillID.None;
        public ESkillID eSkill3ID = ESkillID.None;
        public ESkillID eSkill4ID = ESkillID.None;
    }

    public class TargetInfo
    {
        public GameObject objTarget = null;
        public Vector3 direction = Vector3.zero;
        public float distance = -1f;
    }

    public class ParticleInfo
    {
        public GameObject objParticle = null;
        public float time = -1f;
    }
}

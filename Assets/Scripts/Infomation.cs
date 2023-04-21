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
        public ESkillID eSkillID = ESkillID.None;
        public int skillDesc = -1;
        public bool isTargeting = false;
        public float distance = -1f;
        public float coolTime = -1f;
        public ESkillCost eSkillCost = ESkillCost.None;
        public float cost = -1f;
        public List<EffectInfo> liEffectInfo = new List<EffectInfo>(); // 스킬 그 자체
    }

    [Serializable]
    public class EffectInfo
    {
        public EEffect eEffect = EEffect.None;
        public EEffectType eEffectType = EEffectType.None;
        public EEffectPos eEffectPos = EEffectPos.None;
        public EDamageState eDamageState = EDamageState.None;
        public EDamageType eDamageType = EDamageType.None;
        public ECollision eCollision = ECollision.None;
        public ETargetMask eTargetMask = ETargetMask.None;
        public bool duplication = false;
        public bool onDecal = false;
        public bool moveToPos = false; // true인 경우 moveSpeed에 따라 startDelay는 자동 계산
        public float moveSpeed = -1f;
        public float offsetToTarget = -1f;
        public float startDelay = -1f;
        public float angle = -1f;
        public float sphereRadius = -1f;
        public float boxHalfX = -1f;
        public float boxHalfY = -1f;
        public float boxHalfZ = -1f;
        public float damage = -1f;
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

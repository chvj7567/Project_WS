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
        public ESkillID eSkillID = ESkillID.None; // 스킬 아이디
        public int skillDesc = -1; // 스킬 설명
        public bool isTargeting = false; // 타겟팅 or 논타겟팅
        public float distance = -1f; // 사정거리
        public float coolTime = -1f; // 쿨타임
        public ESkillCost eSkillCost = ESkillCost.None; // 스킬 비용을 뭘로 지불할지
        public float cost = -1f; // 스킬 비용
        public List<EffectInfo> liEffectInfo = new List<EffectInfo>(); // 스킬 그 자체
    }

    [Serializable]
    public class EffectInfo
    {
        public EEffect eEffect = EEffect.None; // 이펙트 종류
        public EEffectType eEffectType = EEffectType.None; // 이펙트 타입
        public EEffectPos eEffectPos = EEffectPos.None; // 이펙트 위치
        public EDamageState eDamageState = EDamageState.None; // 데미지 상태
        public EDamageType eDamageType = EDamageType.None; // 데미지 타입
        public ECollision eCollision = ECollision.None; // 충돌체 모양
        public ETargetMask eTargetMask = ETargetMask.None; // 스킬 대상
        public bool duplication = false; // 스킬 중복 여부
        public bool onDecal = false; // 데칼 표시 여부
        public bool moveToPos = false; // true인 경우 moveSpeed에 따라 startDelay는 자동 계산
        public float moveSpeed = -1f; // moveToPos가 true인 경우만 사용, 움직일 속도
        public float offsetToTarget = -1f; // moveToPos가 true인 경우만 사용, 타겟과의 거리를 얼마나 둘지
        public bool triggerEnter = false; // 이펙트 Trigger에 닿았을 때 데미지를 넣을 것인지 여부
        public bool triggerExit = false; // 이펙트 Trigger에서 벗어났을 때 데미지를 넣을 것인지 여부
        public float stayTickTime = -1f; // 이펙트 Trigger에서 머물고 있을 때 데미지를 넣을 때 틱 타입(음수이면 데미지 안 넣음)
        public float startDelay = -1f; // 스킬 시전 후 이펙트 나오는 시간, 여러 개일 경우 누적 시간
        public float angle = -1f; // 충돌 여부 결정할 때 각도
        public float sphereRadius = -1f; // 구 모양의 충돌체일 경우 구의 반지름
        public float boxHalfX = -1f; // 박스 모양의 충돌체일 경우 X 크기의 반
        public float boxHalfY = -1f; // 박스 모양의 충돌체일 경우 Y 크기의 반
        public float boxHalfZ = -1f; // 박스 모양의 충돌체일 경우 Z 크기의 반
        public float damage = -1f; // 데미지
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

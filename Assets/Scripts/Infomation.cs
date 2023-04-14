using System;
using UnityEngine;
using static Defines;

public class Infomation
{
    [Serializable]
    public class TargetInfo
    {
        public GameObject targetObj;
        public Vector3 direction;
        public float distance;
    }

    [Serializable]
    public class EffectInfo
    {
        public EEffect eEffect = EEffect.None;
        public EEffectType eEffectType = EEffectType.None;
        public EDamageState eDamageState = EDamageState.None;
        public EDamageType eDamageType = EDamageType.None;
        public ECollision eCollision = ECollision.None;
        public EStandardPos eStandardPos = EStandardPos.None;
        public ETargetMask eTargetMask = ETargetMask.None;
        public float startTime = -1f;
        public float angle = -1f;
        public float sphereRadius = -1f;
        public float boxHalfX = -1f;
        public float boxHalfY = -1f;
        public float boxHalfZ = -1f;
        public float damage = -1f;
    }
}

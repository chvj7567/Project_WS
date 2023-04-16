using System;

public class Defines
{
    public enum EJsonType
    {
        None = -1,

        String,
        Skill,
        Unit,

        Max
    }

    public enum EMajor
    {
        None = -1,

        GlobalVolume,
        Player,

        Max
    }

    public enum EResourceType
    {
        None = -1,

        Major,
        Character,
        UI,
        Json,
        Effect,
        Decal,

        Max
    }

    public enum ECharacter
    {
        None = -1,

        Slime,
        Max
    }

    public enum EUI
    {
        None = -1,

        EventSystem,
        UICamera,
        UICanvas,
        UIAlarm,

        Max
    }

    public enum ESkillID
    {
        None = -1,

        Curse, // 0
        Devil, // 1
        Electricity, // 2
        Energy, // 3
        Explosion, // 4
        Explosion_Magic, // 5
        Explosion_Magic2, // 6
        Fire, // 7
        Healing, // 8
        Iceflake, // 9
        Poison, // 10
        Slash, // 11
        Defense, // 12

        Max
    }

    public enum ESkillCost
    {
        None = -1,

        FixedHP,
        PercentHP,
        FixedMP,
        PErcentMP,

        Max
    }

    public enum EEffect
    {
        None = -1,

        FX_Curse, // 0
        FX_Devil, // 1
        FX_Electricity, // 2
        FX_Energy, // 3
        FX_Explosion, // 4
        FX_Explosion_Magic, // 5
        FX_Explosion_Magic2, // 6
        FX_Fire, // 7
        FX_Healing, // 8
        FX_Iceflake, // 9
        FX_Poison, // 10
        Slash, // 11

        FX_Circle_ring, // 12
        FX_Circle_meteor, // 13
        FX_Circle_hit, // 14

        FX_Defense, // 15

        Max
    }

    // 스킬대상자 스킬형태
    public enum EEffectType
    {
        None = -1,

        // 대상자가 자기 자신일 때
        Me_HpUp, // 0
        Me_HpDown, // 1
        Me_AttackStatUp, // 2
        Me_AttackStatDown, // 3
        Me_DefenseStatUp, // 4
        Me_DefenseStatDown, // 5
        // 대상자가 적 또는 팀원일 때
        Target_HpUp, // 6
        Target_HpDown, // 7
        Target_AttackStatUp, // 8
        Target_AttackStatDown, // 9
        Target_DefenseStatUp, // 10
        Target_DefenseStatDown, // 11

        Max,
    }

    public enum ECollision
    {
        None = -1,

        Sphere, // 0
        Box, // 1

        Max
    }

    public enum EStandardPos
    {
        None = -1,

        Me, // 0
        Target_One, // 1
        Target_All, // 2

        Max
    }

    public enum ETargetMask
    {
        None = -1,

        Me,
        Red,
        Blue,
        Me_Red,
        Me_Blue,
        Red_Blue,
        Me_Red_Blue,

        Max
    }

    public enum EDamageState
    {
        None = -1,

        AtOnce, // 0 : 한번에
        Constantly_1Sec_3Count, // 1 : 1초동안 3번 지속 데미지

        Max
    }

    public enum EDamageType
    {
        None = -1,

        Fixed, // 고정 데미지
        PercentMaxHp, // 전체 Hp 퍼센트 데미지
        PercentRemainHp, // 남은 Hp 퍼센트 데미지

        Max,
    }

    public enum EGunType
    {
        None = -1,

        Machinegun,
        MachinegunLauncher,
        Pistol,
        Rocketlauncher,
        RocketlauncherModern,
        Shotgun,
        Sniper,
        Uzi,

        Max
    }

    public enum EGunSkin
    {
        None = -1,

        SniperCamo,
        UziGold,

        Max
    }

    public enum ETag
    {
        None = -1,

        Player,
        Weapon,
        Bullet,

        Max
    }

    public enum EUnitID
    {
        None = -1,

        A, // 0
        B, // 1
        C, // 2
        D, // 3
        E, // 4

        Max
    }

    public enum EDecal
    {
        None = -1,

        RoundArea,
        RoundTiming,
        Box,

        Max
    }

    public enum EUnitAni
    {
        None = -1,

        Run,
        Attack1,

        Max
    }
}
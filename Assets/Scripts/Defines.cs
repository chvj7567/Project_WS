using System;

public class Defines
{
    public enum EJsonType
    {
        None = -1,

        Korea,
        Skill,

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
        Devil2, // 2
        Electricity, // 3
        Energy, // 4
        Explosion, // 5
        Explosion_Magic, // 6
        Explosion_Magic2, // 7
        Fire, // 8
        Floor_splash, // 9
        Healing, // 10
        Iceflake, // 11
        Poison, // 12
        Slash, // 13

        Max
    }

    public enum EEffect
    {
        None = -1,

        FX_Curse, // 0
        FX_Devil, // 1
        FX_Devil2, // 2
        FX_Electricity, // 3
        FX_Energy, // 4
        FX_Explosion, // 5
        FX_Explosion_Magic, // 6
        FX_Explosion_Magic2, // 7
        FX_Fire, // 8
        FX_Floor_splash, // 9
        FX_Healing, // 10
        FX_Iceflake, // 11
        FX_Poison, // 12
        Slash, // 13

        Max
    }

    // 스킬대상자 스킬형태
    public enum EEffectType
    {
        None = -1,

        // 대상자가 자기 자신일 때
        MeHpUp, // 0
        MeHpDown, // 1
        MeAttackStatUp, // 2
        MeAttackStatDown, // 3
        MeDefenseStatUp, // 4
        MeDefenseStatDown, // 5
        // 대상자가 적 또는 팀원일 때
        TargetHpUp, // 6
        TargetHpDown, // 7
        TargetAttackStatUp, // 8
        TargetAttackStatDown, // 9
        TargetDefenseStatUp, // 10
        TargetDefenseStatDown, // 11

        Max,
    }

    public enum ECollision
    {
        None = -1,

        Sphere,
        Box,

        Max
    }

    public enum EStandardPos
    {
        None = -1,

        Me,
        TargetOne,
        TargetAll,

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

        AtOnce, // 한번에
        Constantly, // 지속

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
}
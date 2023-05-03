
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
        GaugeBar,

        Max
    }

    public enum EResourceType
    {
        None = -1,

        Major,
        Unit,
        UI,
        Json,
        Effect,
        Decal,

        Max
    }

    public enum EAssetPiece
    {
        None = -1,

        Materials,
        Meshes,
        Shaders,
        Texture,

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

        Explosion, // 0
        Meteor, // 1
        Heal, // 2
        SlashPoison, // 3
        Tornado, // 4
        IceArrow, // 5
        IceArrow2, // 6
        Fire, // 7
        ElectronicExplosion, // 8

        Max
    }

    public enum ESkillCost
    {
        None = -1,

        Fixed_HP, // 0
        Percent_MaxHP, // 1
        Percent_RemainHP, // 2
        Fixed_MP, // 3
        Percent_MaxMP, // 4
        Percent_RemainMP, // 5

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
        
        FX_Tornado, // 16

        FX_Arrow_impact, // 17
        FX_Arrow_impact_sub, // 18
        FX_Arrow_impact2, // 19

        FX_Explosion_Hit, // 20

        FX_Ax, // 21

        FX_Electricity_Hit, // 22

        Max
    }

    // 스킬대상자 스킬형태
    public enum EEffectType
    {
        None = -1,

        Hp_Up, // 0
        Hp_Down, // 1
        AttackPower_Up, // 2
        AttackPower_Down, // 3
        DefensePower_Up, // 4
        DefensePower_Down, // 5

        Max,
    }

    public enum ECollision
    {
        None = -1,

        Sphere, // 0
        Box, // 1

        Max
    }

    public enum ETarget
    {
        None = -1,

        Position, // 0
        Target_One, // 1
        Target_All, // 2

        Max
    }

    public enum ETargetMask
    {
        None = -1,

        Me, // 0
        MyTeam, // 1
        Enemy, // 2
        MyTeam_Enemy, // 3

        Max
    }

    public enum EDamageState
    {
        None = -1,

        AtOnce, // 0 : 한번에
        Continuous_1Sec_3Count, // 1 : 1초동안 3번 지속 데미지
        Continuous_Dot1Sec_10Count, // 1 : 0.1초동안 10번 지속 데미지
        Max
    }

    public enum EDamageType
    {
        None = -1,

        Fixed, // 0 : 고정 데미지
        Percent_Me_MaxHp, // 1 : 나의 전체 Hp 퍼센트 데미지
        Percent_Me_RemainHp, // 2 : 나의 남은 Hp 퍼센트 데미지
        Percent_Target_MaxHp, // 3 : 타겟의 전체 Hp 퍼센트 데미지
        Percent_Target_RemainHp, // 4 : 타겟의 남은 Hp 퍼센트 데미지

        Max
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

    public enum EUnit
    {
        None = -1,

        Crane, // 0
        Dog, // 1
        Dragon, // 2
        Goat, // 3
        Horse, // 4
        Monkey, // 5
        Ox, // 6
        Pig, // 7
        Rabbit, // 8
        Rat, // 9
        Rooster, // 10
        Snake, // 11
        Tigher, // 12

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

    public enum EStat
    {
        None = -1,

        Hp,
        Mp,
        AttackPower,
        DefensePower,

        Max
    }

    [Flags]
    public enum EUnitState
    {
        None = 0,
        IsDead = 1 << 0,
        IsAirborne = 1 << 1
    }

    public enum EStandardAxis
    {
        None = -1,

        X,
        Z,

        Max
    }

    public enum EMaterial
    {
        None = -1,

        Blue,
        Brown,
        Gray,
        Orange,
        Pink,
        Red,
        White,
        Yellow,

        Max
    }
}

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
        Scriptable,

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

    public enum ESkill
    {
        None = -1,

        Explosion,
        Meteor,
        Heal,
        SlashPoison,
        Tornado,
        IceArrow,
        IceArrow2,
        Ax,

        Max
    }

    public enum ESkillCost
    {
        None = -1,

        Fixed_HP,
        Percent_MaxHP,
        Percent_RemainHP,
        Fixed_MP,
        Percent_MaxMP,
        Percent_RemainMP,

        Max
    }

    public enum EEffect
    {
        None = -1,

        FX_Curse,
        FX_Devil,
        FX_Electricity,
        FX_Energy,
        FX_Explosion,
        FX_Explosion_Magic,
        FX_Explosion_Magic2,
        FX_Fire,
        FX_Healing,
        FX_Iceflake,
        FX_Poison,
        Slash,
        FX_Circle_ring,
        FX_Circle_meteor,
        FX_Circle_hit,
        FX_Defense,
        FX_Tornado,
        FX_Arrow_impact,
        FX_Arrow_impact_sub,
        FX_Arrow_impact2,
        FX_Explosion_Hit,
        FX_Ax,
        FX_Electricity_Hit,

        Max
    }

    // 스킬대상자 스킬형태
    public enum EStatModifyType
    {
        None = -1,

        Hp_Up,
        Hp_Down,
        /*Mp_Up,
        Mp_Down,
        AttackPower_Up,
        AttackPower_Down,
        DefensePower_Up,
        DefensePower_Down,*/

        Max,
    }

    public enum ECollision
    {
        None = -1,

        Sphere,
        Box,

        Max
    }

    public enum ETarget
    {
        None = -1,

        Position,
        Target_One,
        Target_All,

        Max
    }

    public enum ETargetMask
    {
        None = -1,

        Me,
        MyTeam,
        Enemy,
        MyTeam_Enemy,

        Max
    }

    public enum EDamageType1
    {
        None = -1,

        AtOnce, // 한번에
        Continuous_1Sec_3Count, // 1초동안 3번 지속 데미지
        Continuous_Dot1Sec_10Count, // 0.1초동안 10번 지속 데미지
        Max
    }

    public enum EDamageType2
    {
        None = -1,

        Fixed, // 고정 데미지
        Percent_Me_MaxHp, // 나의 전체 Hp 퍼센트 데미지
        Percent_Me_RemainHp, // 나의 남은 Hp 퍼센트 데미지
        Percent_Target_MaxHp, // 타겟의 전체 Hp 퍼센트 데미지
        Percent_Target_RemainHp, // 타겟의 남은 Hp 퍼센트 데미지

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

        Player,
        Monster1,

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

    public enum ETeamLayer
    {
        None = -1,

        Red = 6,
        Blue = 7,

        Max
    }
}

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
        FX_Arrow_impact2, // 18

        Max
    }

    // ��ų����� ��ų����
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

    public enum EEffectPos
    {
        None = -1,

        Me_Only, // 0 : ��ų ��ġ �������� ������ �ڱ����� ���� ��ų
        Me_Targeting, // 1 : ��ų ��ġ ���� �� �ڱ����� Ÿ�������� ���� ��ų
        Me_NoneTargeting, // 2 : ��ų ��ġ ���� �� �ڱ����� ��Ÿ�������� ���� ��ų
        Target_One_Targeting, // 3 : ��ų ��ġ ���� �� Ÿ�� �Ѹ����׸� Ÿ�������� ���� ��ų
        Target_One_NoneTargeting, // 4 : ��ų ��ġ ���� �� Ÿ�� �Ѹ����׸� ��Ÿ�������� ���� ��ų
        Target_All_Targeting, // 5 : ��ų ��ġ ���� �� Ÿ�� ��ο��� Ÿ�������� ���� ��ų
        Target_All_NoneTargeting, // 6 : ��ų ��ġ ���� �� Ÿ�� ��ο��� ��Ÿ�������� ���� ��ų

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

        AtOnce, // 0 : �ѹ���
        Continuous_1Sec_3Count, // 1 : 1�ʵ��� 3�� ���� ������

        Max
    }

    public enum EDamageType
    {
        None = -1,

        Fixed, // 0 : ���� ������
        Percent_Me_MaxHp, // 1 : ���� ��ü Hp �ۼ�Ʈ ������
        Percent_Me_RemainHp, // 2 : ���� ���� Hp �ۼ�Ʈ ������
        Percent_Target_MaxHp, // 3 : Ÿ���� ��ü Hp �ۼ�Ʈ ������
        Percent_Target_RemainHp, // 4 : Ÿ���� ���� Hp �ۼ�Ʈ ������

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
}
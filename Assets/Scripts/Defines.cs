public class Defines
{
    public enum EMajor
    {
        GlobalVolume,
        Player,

        Max
    }

    public enum EResourceType
    {
        Major,
        Character,
        UI,
        Json,
        Effect,

        Max
    }

    public enum ECharacter
    {
        Slime,
    }

    public enum EUI
    {
        EventSystem,
        UICamera,
        UICanvas,
        UIAlarm,

        Max
    }

    public enum EParticle
    {
        FX_Curse,
        FX_Devil,
        FX_Devil2,
        FX_Electricity,
        FX_Energy,
        FX_Explosion,
        FX_Explosion_Magic,
        FX_Explosion_Magic2,
        FX_Fire,
        FX_Floor_splash,
        FX_Healing,
        FX_Iceflake,
        FX_Poison,

        Max
    }

    public enum EGunType
    {
        None,
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
        None,
        SniperCamo,
        UziGold,

        Max
    }

    public enum ETag
    {
        None,
        Player,
        Weapon,
        Bullet,

        Max
    }
}
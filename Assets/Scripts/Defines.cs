public class Defines
{
    public enum EMajor
    {
        GlobalVolume,
        Player,
    }

    public enum EResourceType
    {
        Major,
        Character,
        UI,
        Json,
        Effect,
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
    }

    public enum EEffect
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
    }

    public enum EGunSkin
    {
        None,
        SniperCamo,
        UziGold,
    }

    public enum ETag
    {
        None,
        Player,
        Weapon,
        Bullet,
    }
}
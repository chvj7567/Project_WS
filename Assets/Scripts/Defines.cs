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
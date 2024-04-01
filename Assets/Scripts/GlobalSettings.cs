using System.Collections.Generic;

public class GlobalSettings
{
    public static readonly Dictionary<ZombieType, float> Threatness = new()
    {
            { ZombieType.Zombie, 1},
            { ZombieType.ConeheadZombie, 3},
            { ZombieType.BuckheadZombie, 4}
    };
}
public enum ZombieType
{
    Zombie,
    ConeheadZombie,
    BuckheadZombie
}

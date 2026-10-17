using System;

namespace MiniCrawler.Abilities
{
    [Flags]
    public enum AbilityElement
    {
        None = 0,
        Earth = 1 << 0,
        Wind = 1 << 1,
        Lightning = 1 << 2,
        Fire = 1 << 3,
        Ice = 1 << 4,
        Water = 1 << 5,
        Light = 1 << 6,
    }
}

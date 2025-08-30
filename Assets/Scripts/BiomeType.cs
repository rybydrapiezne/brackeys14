using System;

[Flags]
[Serializable]
public enum BiomeType
{
    None = 0,
    Desert = 1 << 0,
    Riverlands = 1 << 1,
    Canyon = 1 << 2,
    Plateau = 1 << 3,
    Steppes = 1 << 4,
    Dunes = 1 << 5,
    WarlordsTerritory = 1 << 6,
    ScorchedLand = 1 << 7,
    Wastelands = 1 << 8 
}
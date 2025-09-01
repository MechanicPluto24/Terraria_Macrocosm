namespace Macrocosm.Common.Enums;

public enum WallSafetyType
{
    /// <summary> Normal walls, player placed, enemies will NOT spawn in front of them. </summary>
    Safe,
    /// <summary> Natural walls, placed at worldgen, enemies will spawn in front of them. </summary>
    Natural,
    /// <summary> Unsafe walls, player placed, enemies will spawn in front of them. </summary>
    Unsafe
}

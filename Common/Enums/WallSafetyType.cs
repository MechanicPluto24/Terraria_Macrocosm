namespace Macrocosm.Common.Enums
{
    public enum WallSafetyType
    {
        /// <summary> Normal walls, player placed, enemies can't spawn in front of them </summary>
        Normal,
        /// <summary> Natural walls, placed at worldgen, enemies CAN spawn in front of them </summary>
        Natural,
        /// <summary> Unsafe walls, player placed, enemies CAN spawn in front of them </summary>
        Unsafe
    }
}

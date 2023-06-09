namespace Macrocosm.Common.DataStructures
{
    /// <summary> Integer range, inspired by Terraria.Utilities.IntRange </summary>
    public struct IntRange
    {
        /// <summary> Range start </summary>
        public int Start;

        /// <summary> Range end, inclusive </summary>
        public int End;

        /// <summary> Lenght of the range, <see cref="End">End</see> included </summary>
        public int Lenght => End + 1 - Start;

        public IntRange(int start, int end)
        {
            Start = start;
            End = end;
        }

        public IntRange(int end)
        {
            Start = 0;
            End = end;
        }

        /// <summary> Whether the value is found in the range </summary>
        public bool Contains(int value)
            => Start <= value && value <= End;

        public static IntRange operator *(IntRange range, float scale)
        {
            return new IntRange((int)(range.Start * scale), (int)(range.End * scale));
        }

        public static IntRange operator *(float scale, IntRange range)
        {
            return range * scale;
        }

        public static IntRange operator /(IntRange range, float scale)
        {
            return new IntRange((int)(range.Start / scale), (int)(range.End / scale));
        }

        public static IntRange operator /(float scale, IntRange range)
        {
            return range / scale;
        }
    }
}

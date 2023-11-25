using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Utilities;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        /// <summary> Generates a random rotation value within the range of -pi to pi. </summary>
        /// <returns> A random rotation value in radians. </returns>
        public static float RandomRotation()
        {
            return Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
        }

        public static T GetRandom<T>(this IList<T> list)
        {
            return list[Main.rand.Next(list.Count)];
        }

        public static int Next(this UnifiedRandom random, Range range)
        {
            return random.Next(range.Start.Value, range.End.Value);
        }

        public static int NextDirection(this UnifiedRandom random, Range range)
        {
            return random.Next(range) * (random.NextBool() ? 1 : -1);
        }
    }
}

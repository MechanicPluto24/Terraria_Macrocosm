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

        public static T GetRandom<T>(this IList<T> list, UnifiedRandom random = null)
        {
            random ??= Main.rand;
            if (list.Count == 0) throw new InvalidOperationException("Sequence contains no elements");
            return list[random.Next(list.Count)];
        }

        public static int Next(this UnifiedRandom random, Range range)
        {
            return random.Next(range.Start.Value, range.End.Value);
        }

        public static int NextDirection(this UnifiedRandom random, Range range)
        {
            return random.Next(range) * (random.NextBool() ? 1 : -1);
        }

        public static float NormalFloat(this UnifiedRandom random, float mean, float stdDev)
        {
            float u1 = 1.0f - random.NextFloat();
            float u2 = 1.0f - random.NextFloat();
            float randStdNormal = MathF.Sqrt(-2.0f * MathF.Log(u1)) * MathF.Sin(2.0f * MathHelper.Pi * u2);
            return mean + stdDev * randStdNormal;
        }
    }
}

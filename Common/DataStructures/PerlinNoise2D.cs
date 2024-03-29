using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Macrocosm.Common.DataStructures
{
    public class PerlinNoise2D
    {
        public int Seed { get; set; }
        public InterpolationType InterpolationType { get; set; }

        public PerlinNoise2D(int seed, InterpolationType interpolationType = InterpolationType.Linear)
        {
            Seed = seed;
            InterpolationType = interpolationType;
        }

        public float GetValue(float x, float y)
        {
            int floorX = IntFloor(x),
                floorY = IntFloor(y);

            (float lerpX, float lerpY) = InterpolationType switch
            {
                InterpolationType.Linear => (x - floorX, y - floorY),
                _ => throw new NotImplementedException()
            };

            return Lerp(
                Lerp(DotGradient(floorX, floorY, x, y), DotGradient(floorX + 1, floorY, x, y), lerpX),
                Lerp(DotGradient(floorX, floorY + 1, x, y), DotGradient(floorX + 1, floorY + 1, x, y), lerpX),
                lerpY
            );
        }

        private (float, float) RandomGradient(int x, int y)
        {
            float random = x * y * Seed * 2f * MathHelper.Pi;
            return (MathF.Cos(random), MathF.Sin(random));
        }

        private float DotGradient(int cornerX, int cornerY, float x, float y)
        {
            (float gradientX, float gradientY) = RandomGradient(cornerX, cornerY);
            (float offsetX, float offsetY) = (x - cornerX, y - cornerY);

            return offsetX * gradientX + offsetY * gradientY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int IntFloor(float x) => x < 0 ? (int)x - 1 : (int)x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Lerp(float a, float b, float t) => a + t * (b - a);
    }

    public enum InterpolationType
    {
        Linear
    }
}

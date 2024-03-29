
using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Subworlds
{
    public readonly struct WorldSize : TagSerializable
    {
        public int Width { get; }
        public int Height { get; }

        public WorldSize(int x, int y)
        {
            Width = x;
            Height = y;
        }

        public static WorldSize Current => new(Main.maxTilesX, Main.maxTilesY);
        public static WorldSize Small => new(4200, 1200);
        public static WorldSize Medium => new(6400, 1800);
        public static WorldSize Large => new(8400, 2400);

        public static WorldSize operator *(WorldSize size, float value)
        {
            return new((int)(size.Width * value), (int)(size.Height * value));
        }

        public static WorldSize operator /(WorldSize size, float value)
        {
            return new((int)(size.Width / value), (int)(size.Height / value));
        }

        public float GetSizeRatio(WorldSize other)
        {
            if (other.Width == 0 || other.Height == 0)
                return 0f;

            int thisArea = Width * Height;
            int otherArea = other.Width * other.Height;

            return (float)thisArea / otherArea;
        }

        public override bool Equals(object obj)
        {
            return obj is WorldSize size &&
                   Width == size.Width &&
                   Height == size.Height;
        }

        public static bool operator ==(WorldSize left, WorldSize right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WorldSize left, WorldSize right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }

        public TagCompound SerializeData()
        {
            TagCompound tag = new()
            {
                [nameof(Width)] = Width,
                [nameof(Height)] = Height
            };
            return tag;
        }

        public static readonly Func<TagCompound, WorldSize> DESERIALIZER = DeserializeData;

        public static WorldSize DeserializeData(TagCompound tag)
        {
            int width = Small.Width;
            int height = Small.Height;

            if (tag.ContainsKey(nameof(Width)))
                width = tag.GetInt(nameof(Width));

            if (tag.ContainsKey(nameof(Height)))
                height = tag.GetInt(nameof(Height));

            return new(width, height);
        }
    }
}

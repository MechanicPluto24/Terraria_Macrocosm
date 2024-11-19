using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.DataStructures
{
    /// <summary> Data structure that stores texture data </summary>
    public record RawTexture(Color[] Data, int Width, int Height)
    {
        public readonly Color[] Data = Data;
        public readonly int Width = Width;
        public readonly int Height = Height;

        public Color this[int index] => Data[index];
        public Color this[int x, int y] => Data[y * Width + x];

        public int Length => Data.Length;

        public static RawTexture FromStream(Stream stream)
        {
            byte[] colorBytes = ImageIO.ReadRaw(stream, out int width, out int height);
            Color[] colors = new Color[width * height];

            for (int i = 0; i < width * height * 4; i += 4)
            {
                colors[i / 4].PackedValue = (uint)(colorBytes[i + 3] << 24 | colorBytes[i + 2] << 16 | colorBytes[i + 1] << 8 | colorBytes[i]);
            }

            return new(colors, width, height);
        }

        public static RawTexture FromAsset(Asset<Texture2D> asset) => FromTexture2D(asset.Value);
        public static RawTexture FromTexture2D(Texture2D texture)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            Utility.InvokeOnMainThread(() => texture.GetData(0, texture.Bounds, data, 0, texture.Width * texture.Height));
            return new(data, texture.Width, texture.Height);
        }
    }
}
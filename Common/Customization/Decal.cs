using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.ModLoader;

namespace Macrocosm.Common.Customization
{
    public readonly struct Decal
    {
        public string Name { get; }
        public string Context { get; }

        public Asset<Texture2D> Texture { get; }
        private readonly string texturePath;
        public Asset<Texture2D> Icon { get; }
        private readonly string iconPath;

        public Decal(string name, string context, string texturePath, string iconPath)
        {
            Name = name;
            Context = context;

            Texture = ModContent.RequestIfExists(texturePath, out Asset<Texture2D> decalTexture) ? decalTexture : Macrocosm.EmptyTex;
            Icon = ModContent.RequestIfExists(iconPath, out Asset<Texture2D> decalIcon) ? decalIcon : Macrocosm.EmptyTex;
        }

        public override bool Equals(object obj)
        {
            return obj is Decal decal && Name == decal.Name;
        }

        public static bool operator ==(Decal left, Decal right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Decal left, Decal right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Context);
        }
    }
}
